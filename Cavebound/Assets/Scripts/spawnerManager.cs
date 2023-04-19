using FishNet.Managing.Client;
using FishNet.Managing.Server;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections.Generic;
using UnityEngine;

public class spawnerManager : MonoBehaviour
{
    public List<playerSpawn> spawns = new List<playerSpawn>();

    public void setPlayerID(int playerID)
    {
        for (int i = 0; i < spawns.Count; i++)
        {
            if (!spawns[i].isSet)
            {
                //spawns[i].GetComponent<NetworkBehaviour>().GiveOwnership(NetworkManager.ClientManager.Connection);
                spawns[i].setSpawn(playerID);
                break;
            }
        }
    } 
}
