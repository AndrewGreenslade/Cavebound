using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoredInventory : NetworkBehaviour
{
    public List<OreRecord> oresRetrieved = new List<OreRecord>();

    [ServerRpc]
    public void StoreOreinInventory(NetworkConnection conn, string oreName, int amount)
    {
        oresRetrieved.Find(x => x.prefab.OreName == oreName).amount += amount;
        removeores(conn,oreName,amount);
    }

    [TargetRpc]
    void removeores(NetworkConnection conn ,string oreName, int amount)
    {
        Inventory playerInv = GetComponent<emptyPlayer>().SpawnedPlayer.GetComponent<Inventory>();
        playerInv.oresRetrieved.Find(x => x.prefab.OreName == oreName).amount -= amount;
    }
}
