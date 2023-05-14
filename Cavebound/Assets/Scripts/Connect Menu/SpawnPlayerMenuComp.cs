using FishNet.Object;
using UnityEngine;

public class SpawnPlayerMenuComp : NetworkBehaviour
{
    public Transform panel;
    public GameObject playerMenuComp;
    int myID = -999;

    public override void OnStartClient()
    {
        base.OnStartClient();
        myID = GetComponent<NetworkObject>().OwnerId;
        spawnComp(myID);
    }

    [ServerRpc]
    public void spawnComp(int t_playerID)
    {
        panel = GameObject.FindGameObjectWithTag("PlayerPanel").transform;
        GameObject menuComp = Instantiate(playerMenuComp, panel);
        menuComp.GetComponent<PlayerMenuComponent>().playerText.text = "Player: " + t_playerID.ToString();
        ServerManager.Spawn(menuComp, LocalConnection);
    }
}