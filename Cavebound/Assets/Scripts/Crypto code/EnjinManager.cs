using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enjin.SDK.ProjectClient;
using Enjin.SDK;
using Enjin.SDK.ProjectSchema;
using Enjin.SDK.Graphql;
using Enjin.SDK.Models;
using Enjin.SDK.Shared;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class EnjinManager : MonoBehaviour
{
    ProjectClientBuilder builder = Builder();
    System.Uri goerli = EnjinHosts.GOERLI;

    public GameObject ConnectCanvas;
    public GameObject EnjinCanvas;
    public GameObject createButton;

    ProjectClient client = null;
    public TMP_InputField nameField;
    public TextMeshProUGUI nameDisplay;
    public Image qrImage;
    public TextMeshProUGUI LinkCodeText;

    string uuid = "e05dfff6-6836-45cb-af1c-660f72d19651";
    string secret = "ljO9M2hjpFPMj9bFF0LFw4SVuwda57TlnLeYS0YW";
    public string playerID;
    public string code;
    public string qr;
    public string ethAddress;

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
    }

    private void Update()
    {
        updatePlayerID();
    }

    public void updatePlayerID()
    {
        playerID = nameField.text;
    }

    public void getPlayerRemote()
    {
        if (playerID != null)
        {
            GetPlayer req = new GetPlayer().Id(playerID).WithWallet();

            GraphqlResponse<Player> res = client.GetPlayer(req).Result;

            Player player = res.Result;

            if (player != null)
            {
                Wallet wallet = player.Wallet;

                ethAddress = wallet.EthAddress;

                nameDisplay.text = "Welcome: " + playerID;

                EnjinCanvas.SetActive(false);
                ConnectCanvas.SetActive(true);
            }
        }
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
                LinkCodeText.enabled= true;
                LinkCodeText.text = "Link Code: " + code; 
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
        else {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            qrImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2());
        } 
    }
}
