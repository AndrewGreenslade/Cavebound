using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : NetworkBehaviour
{
    public GameObject enemyAI;
    public float enemyCount = 10;
    public bool EnemysSpawned = false;

    private void Update()
    {
        if(!IsServer)
        {
            return;
        }

        if (NetworkManager.ClientManager.Clients.Count > 0 && !EnemysSpawned)
        {
            for (int i = 0; i < enemyCount; i++)
            {
                GameObject AI = Instantiate(enemyAI);
                ServerManager.Spawn(AI);
            }

            EnemysSpawned=true;
        }
    }
}
