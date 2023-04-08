using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StoredInventory : NetworkBehaviour
{
    public List<OreRecord> oresRetrieved = new List<OreRecord>();

    [MenuItem("Andrews Custom Functions/Find Ores in Project")]
    static void FindOresInFolder()
    {
        // Find all assets that start with ore_ and are of type "Ore"'
        string[] guids1 = AssetDatabase.FindAssets("ore_ t:Ore", new[] { "Assets/OreAssets" });

        foreach (string guid1 in guids1)
        {
            Debug.Log(AssetDatabase.GUIDToAssetPath(guid1));
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        AddOresToList();
    }

    public void AddOresToList()
    {
        string[] guids1 = AssetDatabase.FindAssets("ore_ t:Ore", new[] { "Assets/OreAssets" });

        foreach (string guid1 in guids1)
        {
            Debug.Log("ore Asset Found: " + AssetDatabase.GUIDToAssetPath(guid1));

            string assetPath = AssetDatabase.GUIDToAssetPath(guid1);

            if (assetPath != null)
            {
                Ore asset = AssetDatabase.LoadAssetAtPath<Ore>(assetPath);

                if (asset != null)
                {
                    oresRetrieved.Add(new OreRecord { prefab = asset, amount = 0 });
                }
            }
        }
    }

    [ServerRpc]
    public void StoreOreinInventory(string oreName, int amount)
    {
        Inventory playerInv = GetComponent<emptyPlayer>().SpawnedPlayer.GetComponent<Inventory>();

        playerInv.oresRetrieved.Find(x => x.prefab.OreName == oreName).amount -= amount;
        oresRetrieved.Find(x => x.prefab.OreName == oreName).amount += amount;
    }
}
