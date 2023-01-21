using FishNet.Managing;
using FishNet.Transporting.Tugboat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IPScript : MonoBehaviour
{
    private Tugboat myNM;

    void Start()
    {
        myNM = FindObjectOfType<Tugboat>();
    }

    public void ChangeIP(string t_ip)
    {
        myNM.SetClientAddress(t_ip);
    }
}
