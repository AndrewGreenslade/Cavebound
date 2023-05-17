using UnityEngine;
using static Enjin.SDK.ProjectClient;
using Enjin.SDK;
using Enjin.SDK.ProjectSchema;
using Enjin.SDK.Graphql;
using Enjin.SDK.Models;
using Enjin.SDK.Shared;
using Enjin.SDK.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using System.Threading.Tasks;

public class EnjinManager : MonoBehaviour
{
    ProjectClientBuilder builder = Builder();
    System.Uri goerli = EnjinHosts.GOERLI;

    ProjectClient client = null;
    object mutex = new object();
    PusherEventService eventService;

    [Header("Enjin App Vars")]
    public string uuid;
    public string secret;

    [Header("New Player Vars")]
    public string playerID;
    public string code;
    public string qr;
    public Image qrImage;
    public TMP_InputField nameField;
    public GameObject createButton;
    public TextMeshProUGUI LinkCodeText;
    public TextMeshProUGUI nameDisplay;
    
    [Header("Canvas vars")]
    public GameObject ConnectCanvas;
    public GameObject EnjinCanvas;

    [Header("Existing Player Vars")]
    public string ethAddress;

    [Header("Balances!")]
    Dictionary<string, int> fungibleBalances;
    Dictionary<string, ISet<string>> nonFungibleBalances;


    private void Start()
    {
        builder.BaseUri(goerli);
        client = builder.Build();

        client.AuthClient(uuid, secret).Wait();
        AuthProject req = new AuthProject()
                            .Uuid(uuid)
                            .Secret(secret);

        GraphqlResponse<AccessToken> res = client.AuthProject(req).Result;

        bool result = res.IsSuccess;
        
        Debug.Log("Project Authenticated: " + result);

        AccessToken accessToken = res.Result;

        client.Auth(accessToken.Token);

        bool SecondResult = client.IsAuthenticated;

        Debug.Log("Client Authenticated: " + SecondResult);

        long time = accessToken.ExpiresIn.Value;

        builder.EnableAutomaticReauthentication();

        client.AuthClient(uuid, secret).Wait();

        //for setting up event service
        GetPlatform req2 = new GetPlatform().WithNotificationDrivers();
        GraphqlResponse<Platform> res2 = client.GetPlatform(req2).Result;
        Platform platform = res2.Result;

        eventService = PusherEventService.Builder().Platform(platform).Build();
        Task task = eventService.Start();
        bool result2 = eventService.IsConnected;

        Debug.Log("Event service is started: " + result2.ToString());
    }

    public void createNewPlayer()
    {
        CreatePlayer reqCreate = new CreatePlayer().Id(playerID);

        // Using a authenticated ProjectClient
        GraphqlResponse<AccessToken> resCreate = client.CreatePlayer(reqCreate).Result;
        createButton.SetActive(false);

        GetPlayer req = new GetPlayer().Id(playerID).WithLinkingInfo();

        GraphqlResponse<Player> res = client.GetPlayer(req).Result;

        Player player = res.Result;

        if (player != null)
        {
            if (player.LinkingInfo != null)
            {
                LinkingInfo linkingInfo = player.LinkingInfo;

                code = linkingInfo.Code;
                qr = linkingInfo.Qr;
                StartCoroutine(DownloadImage(qr));
                LinkCodeText.enabled = true;
                LinkCodeText.text = "Link Code: " + code;
            }

            nameDisplay.text = "Welcome: " + playerID;

            EnjinCanvas.SetActive(false);
            ConnectCanvas.SetActive(true);
        }
    }

    public void RetrievePlayerData()
    {
        GetPlayer req = new GetPlayer().Id(playerID).WithWallet().WithWalletBalance().WithLinkingInfo();

        GraphqlResponse<Player> res = client.GetPlayer(req).Result;

        Player player = res.Result;

        if (player != null)
        {
            if (player.LinkingInfo != null)
            {
                LinkingInfo linkingInfo = player.LinkingInfo;

                code = linkingInfo.Code;
                qr = linkingInfo.Qr;
                StartCoroutine(DownloadImage(qr));
                LinkCodeText.enabled = true;
                LinkCodeText.text = "Link Code: " + code;
            }

            ethAddress = player.Wallet.EthAddress;

            eventService.SubscribeToWallet(ethAddress);

            if (player.Wallet.Balances != null)
            {
                foreach (Balance balance in player.Wallet.Balances)
                {
                    string id = balance.Id;
                    string index = balance.Index;
                    int? value = balance.Value;

                    if (index.Equals("0000000000000000"))
                    {
                        AddBalanceImpl(id, value.Value);
                    }
                    else
                    {
                        AddBalanceImpl(id, index);
                    }
                }
            }

            if (player.Wallet.EnjBalance.HasValue)
            {
                float value = player.Wallet.EnjBalance.Value;
                Debug.Log("Player has: " + value + " Enj Balance Left");
            }
            
            nameDisplay.text = "Welcome: " + playerID;

            EnjinCanvas.SetActive(false);
            ConnectCanvas.SetActive(true);
        }
    }


    IEnumerator DownloadImage(string MediaUrl)
    {
        qrImage.enabled = true;

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            qrImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2());
        }
    }

    private void Update()
    {
        updatePlayerID();
    }

    public void updatePlayerID()
    {
        playerID = nameField.text;
    }

    // ---------------------------------------------------
    // adding ballances for Fungible assets
    // ---------------------------------------------------

    public void AddBalance(string id, int value)
    {
        lock (mutex)
        {
            AddBalanceImpl(id, value);
        }
    }

    void AddBalanceImpl(string id, int value)
    {
        if (fungibleBalances.ContainsKey(id))
        {
            fungibleBalances[id] += value;
        }
        else
        {
            fungibleBalances.Add(id, value);
        }
    }

    // ---------------------------------------------------
    // adding ballances for non-Fungible assets
    // ---------------------------------------------------

    public void AddBalance(string id, string index)
    {
        lock (mutex)
        {
            AddBalanceImpl(id, index);
        }
    }

    void AddBalanceImpl(string id, string index)
    {
        ISet<string> indices;

        if (nonFungibleBalances.ContainsKey(id))
        {
            indices = nonFungibleBalances[id];
        }
        else
        {
            indices = new HashSet<string>();
            nonFungibleBalances.Add(id, indices);
        }

        indices.Add(index);
    }

    // ---------------------------------------------------
    // Subtracting ballances for Fungible assets
    // ---------------------------------------------------

    public void SubtractBalance(string id, int value)
    {
        lock (mutex)
        {
            if (!fungibleBalances.ContainsKey(id))
            {
                return;
            }

            int newValue = fungibleBalances[id] - value;

            if (newValue > 0)
            {
                fungibleBalances[id] = newValue;
            }
            else
            {
                fungibleBalances.Remove(id);
            }
        }
    }

    // ---------------------------------------------------
    // Subtracting ballances for non-Fungible assets
    // ---------------------------------------------------

    public void SubtractBalance(string id, string index)
    {
        lock (mutex)
        {
            if (nonFungibleBalances.ContainsKey(id))
            {
                nonFungibleBalances[id].Remove(index);
            }
        }
    }

    private void OnDestroy()
    {
        Task task = eventService.Shutdown();
        task.Wait();
    }
}
