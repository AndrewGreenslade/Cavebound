using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using TMPro;

public class countdownTimer : NetworkBehaviour
{
    public float serverTimerInMins;
    private float serverTimerInSeconds;
    public TextMeshProUGUI countdownTimerText;

    [SyncVar]
    public int minsLeft;

    [SyncVar]
    public int secondsLeft;

    public override void OnStartServer()
    {
        base.OnStartServer();

        serverTimerInSeconds = serverTimerInMins * 60.0f;
        minsLeft = Mathf.FloorToInt(serverTimerInSeconds / 60);
        secondsLeft = Mathf.RoundToInt(serverTimerInSeconds % 60);
    }

    // Update is called once per frame
    void Update()
    {
        countdownTimerText.text = minsLeft.ToString() + ":" + secondsLeft.ToString();

        if (!IsServer)
        {
            return;
        }

        serverTimerInSeconds -= Time.deltaTime;

        minsLeft = Mathf.FloorToInt(serverTimerInSeconds / 60);
        secondsLeft = Mathf.RoundToInt(serverTimerInSeconds % 60);
    }
}
