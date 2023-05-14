using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoredInventory : NetworkBehaviour
{
    public List<GameObject> oresAssetsRecords = new List<GameObject>();
    public List<OreRecord> oresRetrieved = new List<OreRecord>();
    private List<StoredOreHud> oreStoredHudList = new List<StoredOreHud>();

    public GameObject oreStoredUIPrefab;
    public Transform oreStoredUIPanel;

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

        oreStoredUIPanel = GameObject.FindGameObjectWithTag("StoredOreUI").transform;
        GameObject paretnStoredUI = oreStoredUIPanel.parent.gameObject;
       
        //sets up hud items
        foreach (OreRecord item in oresRetrieved)
        {
            GameObject ui = Instantiate(oreStoredUIPrefab, oreStoredUIPanel);
            StoredOreHud hud = ui.GetComponent<StoredOreHud>();

            oreStoredHudList.Add(hud);
            hud.setVars(item.prefab.OreName, item.prefab.droppedOre.GetComponent<SpriteRenderer>().color);
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

    [ServerRpc]
    public void AddOreToAmount(string oreName, int amount)
    {
        OreRecord record = oresRetrieved.Find(x => x.prefab.OreName == oreName);

        record.amount += amount;
    }
}
