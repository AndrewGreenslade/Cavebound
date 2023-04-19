using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class OreChunk : NetworkBehaviour
{
    [SyncVar]
    public string oreName;
    [SyncVar]
    private bool isDespawning = false;
    private float despawnTimer = 0;
    public float TodespawnTime = 10.0f;

    public void Update()
    {
        if (IsServer)
        {
            despawnTimer += Time.deltaTime;

            if (despawnTimer > TodespawnTime)
            {
                despawnTimer = 0;
                Despawn();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && !isDespawning)
        {
            setToDespawn();
            getInventoryObj(collision.GetComponent<NetworkObject>().LocalConnection ,GetComponent<NetworkObject>(), collision.GetComponent<NetworkObject>());
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void setToDespawn()
    {
        isDespawning = true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void getInventoryObj(NetworkConnection conn, NetworkObject ore, NetworkObject player)
    {
        player.GetComponent<Inventory>().AddOresToInventory(conn,ore);
    }
}