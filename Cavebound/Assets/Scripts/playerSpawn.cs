using FishNet.Object.Synchronizing;
using FishNet.Object;
using UnityEngine;

public class playerSpawn : NetworkBehaviour
{
    public Transform pos;
    [SyncVar]
    public int playerID = -999;
    [SyncVar]
    public bool isSet = false;

    public override void OnStartClient()
    {
        base.OnStartClient();
        pos = transform;
    }

    [ServerRpc(RequireOwnership = false)]
    public void setSpawn(int t_id)
    {
        isSet = true;
        playerID = t_id;
    }
}
