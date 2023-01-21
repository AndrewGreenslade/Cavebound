using FishNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MissileLauncher : MonoBehaviour
{
    private float fireInput;
    public float FireRate;
    public float timer;
    public Transform firePoint;
    private WeaponNetworkingSystem wepNetworking;
    public float speed = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        timer = FireRate;
        wepNetworking = transform.GetComponentInParent<WeaponNetworkingSystem>(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(timer >= 0)
        {
            timer -= Time.deltaTime;
        }

        if(fireInput > 0 && timer <= 0)
        {
            spawnRocket();
            timer = FireRate;   
        }
    }

    void OnFire(InputValue t_fireVal)
    {
        fireInput = t_fireVal.Get<float>();
    }

    void spawnRocket()
    {
        wepNetworking.spawnObj(0, firePoint.position, firePoint.rotation, speed, playerMove.instance.LocalConnection);
    }
}
