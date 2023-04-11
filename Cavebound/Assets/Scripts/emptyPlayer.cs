using FishNet.Object;
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

        if (IsOwner)
        {
            FindObjectOfType<MapGenUI>().clearMap();
        }
    }

    public void spawnMyPlayer()
    {
        GameObject localPlayer = Instantiate(player, spawn.position, Quaternion.identity);
        SpawnedPlayer = localPlayer;
        ServerManager.Spawn(localPlayer, GetComponent<NetworkObject>().LocalConnection);
    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (PlayerScript.instance != null)
        {
            if (PlayerScript.instance.health <= 0)
            {
                Destroy(PlayerScript.instance.SpawneHud);
                ServerManager.Despawn(SpawnedPlayer);
                spawnMyPlayer();
            }
        }
    }
}
