using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreChunk : NetworkBehaviour
{
    public string oreName;
    private bool isDespawning = false;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && !isDespawning)
        {
            isDespawning = true;
            getInventoryObj(GetComponent<NetworkObject>(), collision.GetComponent<NetworkObject>());
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void getInventoryObj(NetworkObject ore, NetworkObject player)
    {
        player.GetComponent<Inventory>().AddOresToInventory(ore);
    }
}
