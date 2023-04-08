using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class emptyPlayer : NetworkBehaviour
{
    public GameObject player;
    public int playerID;
    public GameObject SpawnedPlayer;
    public Transform spawn;
    public static GameObject instance;

    public override void OnStartClient()
    {
        base.OnStartClient();

        instance = this.gameObject;
        playerID = GetComponent<NetworkObject>().OwnerId;

        spawnerManager manager = FindObjectOfType<spawnerManager>();

        manager.setPlayerID(playerID);

        foreach (var item in manager.spawns)
        {
            if(item.playerID == playerID)
            {
                spawn = item.pos;
                break;
            }
        }

        spawnMyPlayer();
    }

    public void spawnMyPlayer()
    {
        GameObject localPlayer = Instantiate(player, spawn.position, Quaternion.identity);
        SpawnedPlayer = localPlayer;
        ServerManager.Spawn(localPlayer, GetComponent<NetworkObject>().LocalConnection);
    }
}
