using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;

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
    private List<StoredOreHud> oreStoredHudList = new List<StoredOreHud>();

    public GameObject oreUIPrefab;
    public Transform oreUIPanel;

    public GameObject oreStoredUIPrefab;
    public Transform oreStoredUIPanel;
    
    public override void OnStartClient()
    {
        base.OnStartClient();

        oresRetrieved = emptyPlayer.instance.GetComponent<StoredInventory>().oresRetrieved;

        if (GameObject.FindGameObjectWithTag("OreUI").transform.childCount <= 0)
        {
            oreUIPanel = GameObject.FindGameObjectWithTag("OreUI").transform;
            oreStoredUIPanel = GameObject.FindGameObjectWithTag("StoredOreUI").transform;
            GameObject paretnStoredUI = oreStoredUIPanel.parent.gameObject;

            foreach (OreRecord item in oresRetrieved)
            {
                GameObject ui = Instantiate(oreUIPrefab, oreUIPanel);
                oreHud hud = ui.GetComponent<oreHud>();

                oreHudList.Add(hud);
                hud.setVars(item.prefab.OreName, item.prefab.droppedOre.GetComponent<SpriteRenderer>().color);

                GameObject ui2 = Instantiate(oreStoredUIPrefab, oreStoredUIPanel);
                StoredOreHud hud2 = ui2.GetComponent<StoredOreHud>();

                oreStoredHudList.Add(hud2);
                hud2.setVars(item.prefab.OreName, item.prefab.droppedOre.GetComponent<SpriteRenderer>().color);
            }

            //set background element to be active
            foreach (Transform item in paretnStoredUI.transform)
            {
                if (!item.gameObject.activeSelf)
                {
                    item.gameObject.SetActive(true);
                }
            }

            //then disable parent element
            paretnStoredUI.SetActive(false);
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
