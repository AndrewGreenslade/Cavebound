using FishNet;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

public class WeaponNetworkingSystem : NetworkBehaviour
{
    public Transform myTurret;
    Vector3 FinalPos;
    public List<GameObject> BulletRoundTypes;
    public float turretRotSpeed = 5.0f;

    // Update is called once per frame
    void Update()
    {
        if(!base.IsOwner)
        {
            return;
        }

        FinalPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - myTurret.position;
        float angle = Mathf.Atan2(FinalPos.y, FinalPos.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.AngleAxis(angle, Vector3.forward);
        myTurret.rotation = Quaternion.Slerp(myTurret.rotation, rot, turretRotSpeed * Time.deltaTime);
    }

    [ServerRpc]
    public void spawnObj(int t_roundType,Vector3 pos, Quaternion rot,float speed)
    {
        GameObject rocket = Instantiate(BulletRoundTypes[t_roundType], pos, rot);
        rocket.GetComponent<Rigidbody2D>().velocity = rocket.transform.up * speed;
        InstanceFinder.ServerManager.Spawn(rocket, LocalConnection);
    }
}
