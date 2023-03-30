using FishNet.Object;
using FishNet.Object.Synchronizing;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

public class spawnerManager : NetworkBehaviour
{
    public List<playerSpawn> spawns = new List<playerSpawn>();

    public override void OnStartServer()
    {
        base.OnStartServer();

        for (int i = 0; i < spawns.Count; i++)
        {
            spawns[i].playerID = -999;
        }
    }

    [ServerRpc(RequireOwnership = false, RunLocally = true)]
    public void setPlayerID(int playerID)
    {
        for (int i = 0; i < spawns.Count; i++)
        {
            if (spawns[i].playerID < 0)
            {
                spawns[i].playerID = playerID;
                break;
            }
        }
    } 
}
