using Enjin.SDK.Models;
using FishNet;
using FishNet.Connection;

using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class spawnerManager : NetworkBehaviour
{
    public List<playerSpawn> spawns = new List<playerSpawn>();
    public GameObject playerControllerPrefab;

    public bool hasSpawnedFirstPlayer = false;

    public SceneChangeCount countSceneLoad;
    public bool isSpawningPlayer;

    public override void OnStartClient()
    {
        base.OnStartClient();
        countSceneLoad = FindObjectOfType<SceneChangeCount>();
    }

    void Update()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Game")
        {
            if (countSceneLoad != null && !hasSpawnedFirstPlayer)
            {
                if (countSceneLoad.totalPlayersLoadedScene >= ServerManager.Clients.Count)
                {
                    SpawnPlayer(LocalConnection);
                    hasSpawnedFirstPlayer = true;
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayer(NetworkConnection connection)
    {
        GameObject go = Instantiate(playerControllerPrefab);
        InstanceFinder.ServerManager.Spawn(go, connection);
    }

    [ServerRpc(RequireOwnership = false)]
    public void setPlayerID(int playerID)
    {
        StartCoroutine(SetPlayerSpawn(playerID));
    }

    IEnumerator SetPlayerSpawn(int t_id)
    {
        yield return new WaitForSeconds(2.0f); 

        for (int i = 0; i < spawns.Count; i++)
        {
            if (!spawns[i].isSet)
            {
                spawns[i].setSpawn(t_id);
                break;
            }
        }

        yield return new WaitForSeconds(2.0f);
    }
}
