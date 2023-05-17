using FishNet.Managing;
using FishNet.Transporting;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public NetworkManager networkManager;
    public GameObject toDisable;
    public GameObject toEnable;
    public GameObject hostButton;
    public EnjinManager enjinManager;

    private LocalConnectionState clientState = LocalConnectionState.Stopped;
   
    private LocalConnectionState serverState = LocalConnectionState.Stopped;

    public void OnClick_Server()
    {
        if (networkManager == null)
            return;

        if (serverState != LocalConnectionState.Stopped)
        {
            networkManager.ServerManager.StopConnection(true);
            serverState = LocalConnectionState.Stopped;
        }
        else {
            networkManager.ServerManager.StartConnection();
            serverState = LocalConnectionState.Started;
            hostButton.SetActive(false);
        }
    }


    public void OnClick_Client()
    {
        if (networkManager == null)
            return;

        if (clientState != LocalConnectionState.Stopped)
        {
            networkManager.ClientManager.StopConnection();
            clientState = LocalConnectionState.Stopped;
        }
        else
        {
            networkManager.ClientManager.StartConnection();
            clientState = LocalConnectionState.Started;
            toDisable.SetActive(false);
            toEnable.SetActive(true);
            enjinManager.nameDisplay.gameObject.SetActive(false);
            enjinManager.qrImage.gameObject.SetActive(false);
            enjinManager.LinkCodeText.gameObject.SetActive(false);

        }
    }
}
