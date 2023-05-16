using FishNet.Object;
using FishNet.Object.Synchronizing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMenuComponent : NetworkBehaviour
{ 
    [SyncVar]
    public bool isReady = false;
    public Button MyReadyButton;
    public TextMeshProUGUI playerText;
    
    [SyncVar]
    public int playerID = -999;

    public override void OnStartClient()
    {
        base.OnStartClient();

        MyReadyButton.onClick.AddListener(ReadyPlayerUp);

        Transform panel = GameObject.FindGameObjectWithTag("PlayerPanel").transform;

        transform.SetParent(panel);

        transform.localScale = Vector3.one;

        playerText.text = "Player: " + playerID.ToString();
    }

    public void ReadyPlayerUp()
    {
        setReady();
        MyReadyButton.gameObject.SetActive(false);
    }

    [ServerRpc(RequireOwnership = false)]
    public void setReady()
    {
        isReady = true;
    }
}
