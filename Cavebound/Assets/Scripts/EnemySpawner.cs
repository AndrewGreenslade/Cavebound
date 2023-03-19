using FishNet.Managing.Server;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : NetworkBehaviour
{
    public GameObject enemyAI;

    public override void OnStartServer()
    {
        base.OnStartServer();
        GameObject AI = Instantiate(enemyAI);
        ServerManager.Spawn(AI);
    }
}
