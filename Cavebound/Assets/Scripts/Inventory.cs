using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet;

/// <summary>
/// This should only show on host, so player has no access to his Inventory on his client
/// </summary>
[System.Serializable]
public class OreRecord
{ 
    public Ore prefab;
    public int amount;
}

public class Inventory : NetworkBehaviour
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

    public override void OnStartServer()
    {
        base.OnStartServer();
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
                    oresRetrieved.Add(new OreRecord {prefab = asset,amount = 0});
                }
            }
        }
    }

    public void AddOresToInventory(NetworkObject oreObject)
    {
        foreach(OreRecord item in oresRetrieved)
        {
            if(item.prefab.OreName == oreObject.GetComponent<OreChunk>().oreName)
            {
                item.amount++;
                oreObject.Despawn();
            }
        }
    }
}
