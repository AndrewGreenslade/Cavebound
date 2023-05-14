using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oreDepositer : NetworkBehaviour
{
    public GameObject playerObject;
    public bool isPlayerIn;

    public GameObject oreStoredGameobject;

    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    private void Update()
    {
        if (oreStoredGameobject != null)
        {
            if (isPlayerIn)
            {
                oreStoredGameobject.SetActive(true);
            }
            else
            {
                oreStoredGameobject.SetActive(false);
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.GetComponent<PlayerScript>().isLocalPlayer)
            {
                if (oreStoredGameobject == null)
                {
                    oreStoredGameobject = emptyPlayer.instance.GetComponent<StoredInventory>().oreStoredUIPanel.parent.gameObject;
                }

                collision.GetComponent<PlayerScript>().isInMenu= true;
                playerObject = collision.gameObject;
                isPlayerIn = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.GetComponent<PlayerScript>().isLocalPlayer)
            {
                collision.GetComponent<PlayerScript>().isInMenu = false;
                playerObject = null;
                isPlayerIn = false;
            }
        }
    }
}
