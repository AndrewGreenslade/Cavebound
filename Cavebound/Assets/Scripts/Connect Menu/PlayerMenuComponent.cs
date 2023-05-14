using FishNet.Object;
using FishNet.Object.Synchronizing;
using TMPro;
using UnityEngine.UI;

public class PlayerMenuComponent : NetworkBehaviour
{ 
    [SyncVar]
    public bool isReady = false;
    public Button MyReadyButton;
    public TextMeshProUGUI playerText;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if(IsOwner)
        {
            MyReadyButton.onClick.AddListener(ReadyPlayerUp);
        }
        else
        {
            MyReadyButton.gameObject.SetActive(false);
        }
    }

    public void ReadyPlayerUp()
    {
        setReady();
        MyReadyButton.gameObject.SetActive(false);
    }

    [ServerRpc]
    public void setReady()
    {
        isReady = true;
    }
}
