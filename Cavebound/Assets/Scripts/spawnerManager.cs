using FishNet.Object;
using System.Collections.Generic;

public class spawnerManager : NetworkBehaviour
{
    public List<playerSpawn> spawns = new List<playerSpawn>();

    [ServerRpc(RequireOwnership = false)]
    public void setPlayerID(int playerID)
    {
        for (int i = 0; i < spawns.Count; i++)
        {
            if (!spawns[i].isSet)
            {
                spawns[i].setSpawn(playerID);
                break;
            }
        }
    } 
}
