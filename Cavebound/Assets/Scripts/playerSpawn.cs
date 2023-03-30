using FishNet.Object.Synchronizing;
using FishNet.Object;
using UnityEngine;

public class playerSpawn : NetworkBehaviour
{
    public Transform pos;
    [SyncVar]
    public int playerID;

    public override void OnStartClient()
    {
        base.OnStartClient();

        pos = this.transform;
    }
}
