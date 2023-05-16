using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class playerMenuSpawner : NetworkBehaviour
{
    public Transform panel;
    public GameObject playerMenuComp;
    public GameObject playerMenuCompForClientOnly;

    public int ownerID = -999;

    [SyncVar]
    public NetworkObject obj;

    public bool hasEnabledButton = false;

    public override void OnStartClient()
    {
        base.OnStartClient();

        ownerID = GetComponent<NetworkObject>().OwnerId;

        if (!IsServer)
        {
            if (IsOwner)
            {
                spawnCompOnServer(ownerID);
            }
        }
        else if (IsOwner && IsServer)
        {
            spawnComp(ownerID);
        }
    }

    private void Update()
    {
        if (obj != null && hasEnabledButton == false && IsOwner && !IsServer)
        {
            enableButtonongameobject();
            hasEnabledButton = true;
        }
    }

    public void spawnComp(int t_playerID)
    { 
        panel = GameObject.FindGameObjectWithTag("PlayerPanel").transform;

        GameObject menuComp = Instantiate(playerMenuComp, panel);

        menuComp.GetComponent<PlayerMenuComponent>().playerID = t_playerID;
        
        obj = menuComp.GetComponent<NetworkObject>();

        ServerManager.Spawn(menuComp, LocalConnection);

        GameObject buttonGO = obj.transform.FindChild("ReadyButton").gameObject;
        buttonGO.SetActive(true);
    }


    [ServerRpc]
    public void spawnCompOnServer(int t_playerID)
    {
        panel = GameObject.FindGameObjectWithTag("PlayerPanel").transform;

        GameObject menuComp = Instantiate(playerMenuComp);

        menuComp.GetComponent<PlayerMenuComponent>().playerID = t_playerID;
       
        base.Spawn(menuComp, LocalConnection);

        obj = menuComp.GetComponent<NetworkObject>();
    }

    public void enableButtonongameobject()
    {
        GameObject buttonGO = obj.transform.FindChild("ReadyButton").gameObject;
        buttonGO.SetActive(true);
    }
}