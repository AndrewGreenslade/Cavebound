using FishNet.Object;
using UnityEngine;
using TMPro;
using FishNet.Connection;
using FishNet.Object.Synchronizing;
using UnityEngine.Networking;
using static UnityEditor.Progress;

public class emptyPlayer : NetworkBehaviour
{
    private bool shouldSpawnPlayer = false;
    public GameObject player;

    [SyncVar]
    public int playerID;

    [SyncVar]
    public NetworkObject SpawnedPlayer;
    
    [SyncVar]
    public bool spawnSet = false;

    [SyncVar]
    public Transform spawn;

    public static emptyPlayer instance;

    [SyncVar]
    public string userName;
    
    spawnerManager manager;

    public override void OnStartServer()
    {
        base.OnStartServer();

        playerID = GetComponent<NetworkObject>().OwnerId;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (IsOwner)
        {
            instance = this;

            //userName = FindObjectOfType<EnjinManager>().playerID;

            FindObjectOfType<MapGenUI>().clearMap();

            shouldSpawnPlayer = true;
        }
    }

    void Update()
    {
        if (IsOwner)
        {
            if (manager == null)
            {
                manager = FindObjectOfType<spawnerManager>();

                if (!spawnSet && manager != null)
                {
                    manager.setPlayerID(playerID);
                }
            }
            else if (!spawnSet && shouldSpawnPlayer)
            {
                foreach (var item in manager.spawns)
                {
                    if (item.playerID == playerID)
                    {
                        shouldSpawnPlayer = false;
                        setSpawnVars(item.pos);
                        SpawnPlayer();
                        break;
                    }
                }
            }

            if (SpawnedPlayer != null)
            {
                PlayerScript ps = SpawnedPlayer.GetComponent<PlayerScript>();

                if (ps.health <= 0)
                {
                    Debug.Log("Despawning player");
                    Despawn(SpawnedPlayer.gameObject);
                    SpawnPlayer();
                }
            }
        }
    }

    [ServerRpc]
    private void SpawnPlayer()
    {
        GameObject localPlayer = Instantiate(player, spawn.position, Quaternion.identity);
        SpawnedPlayer = localPlayer.GetComponent<NetworkObject>();
        ServerManager.Spawn(localPlayer, ClientManager.Clients[playerID]);
    }

    [ServerRpc]
    private void setSpawnVars(Transform t_spawnPos)
    {
        spawnSet = true;
        spawn = t_spawnPos;
    }
}
