using FishNet.Object;
using UnityEngine;
using TMPro;
using FishNet.Connection;
using FishNet.Object.Synchronizing;
using UnityEngine.SceneManagement;

public class emptyPlayer : NetworkBehaviour
{
    [SyncVar]
    public bool shouldSpawnPlayer = false;
    
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

        shouldSpawnPlayer = true;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (IsOwner)
        {
            instance = this;

            //userName = FindObjectOfType<EnjinManager>().playerID;
        }
    }

    void Update()
    {
        if (IsServer)
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
                        spawnSet = true;
                        spawn = item.pos;
                        SpawnPlayer(Owner);
                        shouldSpawnPlayer = false;
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
                    SpawnPlayer(Owner);
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayer(NetworkConnection conn)
    {
        GameObject localPlayer = Instantiate(player, spawn.position, Quaternion.identity);
        ServerManager.Spawn(localPlayer, conn);
        SpawnedPlayer = localPlayer.GetComponent<NetworkObject>();
    }
}
