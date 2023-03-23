using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AILaserScript : NetworkBehaviour
{
    public float damage = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsServer)
        {
            if (collision.CompareTag("Player"))
            {
                collision.GetComponent<PlayerScript>().damagePlayer(damage);
            }
        }

        if (!collision.CompareTag("AI"))
        {
            ServerManager.Despawn(gameObject);
        }
    }
}
