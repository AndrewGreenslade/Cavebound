using FishNet;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : NetworkBehaviour
{
    public GameObject Explosion;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        spawnExp(Explosion);
        Destroy(gameObject);
    }

    void spawnExp(GameObject t_obj)
    {
        GameObject explosion = Instantiate(t_obj, transform.position, Quaternion.identity);
        Destroy(explosion, 4.0f);
        InstanceFinder.ServerManager.Spawn(explosion);
    }
}
