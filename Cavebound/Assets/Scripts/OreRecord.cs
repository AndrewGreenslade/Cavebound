using FishNet.Object.Synchronizing;
using FishNet.Object;

/// <summary>
/// This should only show on host, so player has no access to his Inventory on his client
/// </summary>
[System.Serializable]
public class OreRecord : NetworkBehaviour
{
    public Ore prefab;
    [SyncVar]
    public int amount;
}
