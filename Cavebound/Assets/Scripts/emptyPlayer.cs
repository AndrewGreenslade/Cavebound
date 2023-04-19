using FishNet.Object;
using UnityEngine;
using TMPro;
using FishNet.Connection;

public class emptyPlayer : NetworkBehaviour
{
    public GameObject player;
    public int playerID;
    public GameObject SpawnedPlayer;

    public bool spawnSet = false;
    public Transform spawn;
    public static emptyPlayer instance;
    public string userName;
    spawnerManager manager;

    public override void OnStartClient()
    {
        base.OnStartClient();

        playerID = GetComponent<NetworkObject>().OwnerId;

        if (IsOwner)
        {
            instance = this;

            userName = FindObjectOfType<EnjinManager>().playerID;

            FindObjectOfType<MapGenUI>().clearMap();
        }
    }

    [ServerRpc]
    public void spawnMyPlayer(string name, Vector3 pos, NetworkConnection conn)
    {
        GameObject localPlayer = Instantiate(player, pos, Quaternion.identity);
        localPlayer.transform.GetComponentInChildren<TextMeshProUGUI>().text = name;
        ServerManager.Spawn(localPlayer,conn );
    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (manager == null)
        {
            manager = FindObjectOfType<spawnerManager>();
            
            if (!spawnSet && manager != null)
            {
                manager.setPlayerID(playerID);
            }
        }
        else if(!spawnSet)
        {
            foreach (var item in manager.spawns)
            {
                if (item.playerID == playerID)
                {
                    spawn = item.pos;
                    spawnSet = true;
                    spawnMyPlayer(userName, spawn.position, GetComponent<NetworkObject>().LocalConnection);
                    break;
                }
            }
        }


        if (SpawnedPlayer == null)
        {
            foreach (var p in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (p.GetComponent<PlayerScript>().isLocalPlayer)
                {
                    SpawnedPlayer = p;
                }
            }
        }

        if (PlayerScript.instance != null)
        {
            if (PlayerScript.instance.health <= 0)
            {
                foreach (OreRecord item in PlayerScript.instance.GetComponent<Inventory>().oresRetrieved)
                {
                    item.amount = 0;
                }

                foreach (Transform item in PlayerScript.instance.GetComponent<Inventory>().oreUIPanel.transform)
                {
                    item.GetComponent<oreHud>().Reset();
                }

                Destroy(PlayerScript.instance.SpawnedHud);
                ServerManager.Despawn(SpawnedPlayer);
                spawnMyPlayer(userName, spawn.position, GetComponent<NetworkObject>().LocalConnection);
            }
        }
    }
}
