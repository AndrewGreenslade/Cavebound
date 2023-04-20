using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoredInventory : NetworkBehaviour
{
    public List<GameObject> oresAssetsRecords = new List<GameObject>();
    public List<OreRecord> oresRetrieved = new List<OreRecord>();

    private void Start()
    {
        spawnOres();
    }

    public void spawnOres()
    {
        foreach (var item in oresAssetsRecords)
        {
            oresRetrieved.Add(item.GetComponent<OreRecord>());
        }
    }

    public void StoreOreinInventory(string oreName, int amount)
    {
        oresRetrieved.Find(x => x.prefab.OreName == oreName).amount += amount;
    }

}
