using FishNet;
using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : NetworkBehaviour
{
    public GameObject Explosion;

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log(OwnerId);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(IsServer)
        {
            spawnExp(Explosion, playerMove.instance.LocalConnection);
            modifyTilemap(transform.position);
            Destroy(gameObject);
        }
    }

    void spawnExp(GameObject t_obj, NetworkConnection conn)
    {
        GameObject explosion = Instantiate(t_obj, transform.position, Quaternion.identity);
        Destroy(explosion, 3.0f);
        InstanceFinder.ServerManager.Spawn(explosion, conn);
    }

    [ObserversRpc(RunLocally = true)]
    void modifyTilemap(Vector3 pos)
    {
        Vector3Int newLoc = new Vector3Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z));
        
        for (int y = -2; y < 3; y++)
        {
            for (int x = -2; x < 3; x++)
            {
                Vector3Int finalPos = newLoc + new Vector3Int(x, y, 0);

                MapGenerator.instance.map.SetTile(finalPos, null);

                if (finalPos.y > MapGenerator.instance.edgeSize && finalPos.y < MapGenerator.instance.MapHieght - MapGenerator.instance.edgeSize && finalPos.x > MapGenerator.instance.edgeSize && finalPos.x < MapGenerator.instance.MapWidth - MapGenerator.instance.edgeSize)
                {
                    MapGenerator.instance.BorderMap.SetTile(finalPos, null);
                }
            }
        }
    }
}
