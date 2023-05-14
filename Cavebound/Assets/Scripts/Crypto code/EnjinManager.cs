using UnityEngine;
using static Enjin.SDK.ProjectClient;
using Enjin.SDK;
using Enjin.SDK.ProjectSchema;
using Enjin.SDK.Graphql;
using Enjin.SDK.Models;
using Enjin.SDK.Shared;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class EnjinManager : MonoBehaviour
{
    ProjectClientBuilder builder = Builder();
    System.Uri goerli = EnjinHosts.GOERLI;

    ProjectClient client = null;

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
    
    [Header("Canvar vars")]
    public GameObject ConnectCanvas;
    public GameObject EnjinCanvas;

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

}
