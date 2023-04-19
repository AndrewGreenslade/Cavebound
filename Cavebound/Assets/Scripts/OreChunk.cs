using Codice.Client.BaseCommands.BranchExplorer;
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
        if(collision.tag == "Player" && !isDespawning && collision.GetComponent<PlayerScript>().isLocalPlayer)
        {
            getInventoryObj(gameObject, collision.gameObject);
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void setToDespawn()
    {
        Despawn();
    }

    public void getInventoryObj(GameObject ore, GameObject player)
    {
        player.GetComponent<Inventory>().addOreToUI(oreName);
        player.GetComponent<Inventory>().AddOresToInventory(oreName);
        setToDespawn();
    }
}