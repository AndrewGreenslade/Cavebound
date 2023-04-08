using FishNet.Managing;
using FishNet.Transporting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public NetworkManager networkManager;
    public GameObject ServerButton;
    public GameObject rootMenuCanvas;

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
            ServerButton.SetActive(false);
        }
    }


    public void OnClick_Client()
    {
        if (networkManager == null)
            return;

        if (clientState != LocalConnectionState.Stopped)
        {
            networkManager.ClientManager.StopConnection();
            serverState = LocalConnectionState.Stopped;
        }
        else
        {
            networkManager.ClientManager.StartConnection();
            serverState = LocalConnectionState.Started;
            rootMenuCanvas.SetActive(false);
        }
    }
}
