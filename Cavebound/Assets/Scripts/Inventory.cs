using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet;
using FishNet.Connection;
using static UnityEditor.Progress;

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
    private List<oreHud> oreHudList = new List<oreHud>();

    public GameObject oreUIPrefab;
    public Transform oreUIPanel;

    public override void OnStartClient()
    {
        base.OnStartClient();

        oresRetrieved = emptyPlayer.instance.GetComponent<StoredInventory>().oresRetrieved;

        if (GameObject.FindGameObjectWithTag("OreUI").transform.childCount <= 0)
        {
            oreUIPanel = GameObject.FindGameObjectWithTag("OreUI").transform;

            foreach (OreRecord item in oresRetrieved)
            {
                GameObject ui = Instantiate(oreUIPrefab, oreUIPanel);
                oreHud hud = ui.GetComponent<oreHud>();

                oreHudList.Add(hud);
                hud.setVars(item.prefab.OreName, item.prefab.droppedOre.GetComponent<SpriteRenderer>().color);
            }
        }
    }

    public void AddOresToInventory(NetworkConnection conn, NetworkObject oreObject)
    {
        foreach(OreRecord item in oresRetrieved)
        {
            if(item.prefab.OreName == oreObject.GetComponent<OreChunk>().oreName)
            {
                addOreToUI(conn, item.prefab.OreName);
                item.amount++;
                oreObject.Despawn();
            }
        }
    }
    
    [TargetRpc]
    private void addOreToUI(NetworkConnection conn, string itemName)
    {
        foreach (var hudItem in oreHudList)
        {
            if (hudItem.oreName == itemName)
            {
                hudItem.AddOreToAmount();
                break;
            }
        }
    }
}
