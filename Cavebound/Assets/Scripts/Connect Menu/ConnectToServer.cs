using UnityEngine;
using FishNet.Managing;

public class ConnectToServer : MonoBehaviour
{
    private NetworkManager _networkManager;

    public GameObject ToDespawn;
    public GameObject ToEnable;

    private void Start()
    {
        _networkManager = FindObjectOfType<NetworkManager>();
    }

    public void OnClick_Server()
    {
        if (_networkManager == null)
            return;

        _networkManager.ServerManager.StartConnection();

        DeselectButtons();
        EnablePlayerMenu();
    }


    public void OnClick_Client()
    {
        if (_networkManager == null)
            return;

        _networkManager.ClientManager.StartConnection();

        DeselectButtons();
        EnablePlayerMenu();
    }

    private void DeselectButtons()
    {
        ToDespawn.SetActive(false);
    }

    private void EnablePlayerMenu()
    {
        ToEnable.SetActive(true);
    }
}
