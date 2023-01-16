using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponNetworkingSystem : NetworkBehaviour
{
    public Transform myTurret;
    Vector3 worldPosition;
    public List<GameObject> BulletRoundTypes;

    // Update is called once per frame
    void Update()
    {
        if(!isLocalPlayer)
        {
            return;
        }

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;

        worldPosition = Camera.main.ScreenToWorldPoint(mousePos);
        worldPosition.z = 0;

        myTurret.transform.up = worldPosition;
    }

    [Command]
    public void spawnObj(int t_roundType,Vector3 pos, Quaternion rot,float speed)
    {
        GameObject rocket = Instantiate(BulletRoundTypes[t_roundType], pos, rot);
        rocket.GetComponent<Rigidbody2D>().velocity = rocket.transform.up * speed;
        NetworkServer.Spawn(rocket);
    }
}
