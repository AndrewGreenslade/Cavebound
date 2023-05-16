using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;

public class Inventory : NetworkBehaviour
{
    public List<GameObject> oresAssetsRecords = new List<GameObject>();
    public List<OreRecord> oresRetrieved = new List<OreRecord>();
    
    private List<oreHud> oreHudList = new List<oreHud>();

    public GameObject oreUIPrefab;
    public Transform oreUIPanel;
    
    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!IsOwner)
        {
            return;
        }

        if (!IsServer)
        {
            spawnOres();
        }

        if (GameObject.FindGameObjectWithTag("OreUI").transform.childCount <= 0)
        {
            //gets nesesary components
            oreUIPanel = GameObject.FindGameObjectWithTag("OreUI").transform;

            //sets up hud items
            foreach (OreRecord item in oresRetrieved)
            {
                GameObject ui = Instantiate(oreUIPrefab, oreUIPanel);
                oreHud hud = ui.GetComponent<oreHud>();

                oreHudList.Add(hud);
                hud.setVars(item.prefab.OreName, item.prefab.droppedOre.GetComponent<SpriteRenderer>().color);
            }
        }
        else
        {
            //gets nesesary components
            oreUIPanel = GameObject.FindGameObjectWithTag("OreUI").transform;
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        spawnOres();
    }

    public void spawnOres()
    {
        foreach (var item in oresAssetsRecords)
        {
            oresRetrieved.Add(item.GetComponent<OreRecord>());
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddOresToInventory(string name)
    {
        foreach(OreRecord item in oresRetrieved)
        {
            if(item.prefab.OreName == name)
            {
                item.amount++;
            }
        }
    }

    [Client]
    public void addOreToUI(string itemName)
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

    [TargetRpc]
    public void resetOres(NetworkConnection conn = null)
    {
        foreach (OreRecord Item in oresRetrieved)
        {
            Item.amount = 0;
        }
    }

    public int GetAndResetOreAmount(string oreName)
    {
        OreRecord record = oresRetrieved.Find(x => x.prefab.OreName == oreName);

        int RecordAmount = record.amount;

        record.amount = 0;

        return RecordAmount;
    }
}
