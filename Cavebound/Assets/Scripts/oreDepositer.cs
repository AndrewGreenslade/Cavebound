using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oreDepositer : MonoBehaviour
{
    public GameObject playerObject;
    public bool isPlayerIn;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerObject = collision.gameObject;
            isPlayerIn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerObject = null;
            isPlayerIn = false;
        }
    }
}
