using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AILaserScript : NetworkBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("AI"))
        {
            ServerManager.Despawn(gameObject);
        }
    }
}
