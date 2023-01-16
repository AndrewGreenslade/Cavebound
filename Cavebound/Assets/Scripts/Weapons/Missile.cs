using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : NetworkBehaviour
{
    public GameObject Explosion;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        spawnExp();
        Destroy(gameObject);
    }

    [Command(requiresAuthority = false)]
    void spawnExp()
    {
        GameObject explosion = Instantiate(Explosion, transform.position, transform.rotation);
        Destroy(explosion, 2.5f);
        NetworkServer.Spawn(explosion);
    }
}
