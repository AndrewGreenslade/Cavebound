using FishNet;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class Missile : NetworkBehaviour
{
    public GameObject Explosion;

    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsServer)
        {
            spawnExp(Explosion, playerMove.instance.LocalConnection);
            Vector3Int newLoc = new Vector3Int((int)(transform.position.x), (int)(transform.position.y), (int)(transform.position.z));
            spawnOre(newLoc,LocalConnection);
            modifyTilemap(newLoc);
            modifyNodes(newLoc);
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
    void modifyTilemap(Vector3Int pos)
    {

        for (int y = -2; y < 3; y++)
        {
            for (int x = -2; x < 3; x++)
            {
                Vector3Int finalPos = new Vector3Int(Mathf.CeilToInt(pos.x), pos.y , 0) + new Vector3Int(x,y, 0);

                MapGenerator.instance.GroundMap.SetTile(finalPos, null);
                MapGenerator.instance.OreMap.SetTile(finalPos, null);

                if (finalPos.y > MapGenerator.instance.edgeSize && finalPos.y < MapGenerator.instance.MapHieght - MapGenerator.instance.edgeSize && finalPos.x > MapGenerator.instance.edgeSize && finalPos.x < MapGenerator.instance.MapWidth - MapGenerator.instance.edgeSize)
                {
                    MapGenerator.instance.BorderMap.SetTile(finalPos, null);
                }
            }
        }
    }

    void modifyNodes(Vector3Int pos)
    {

        for (int y = -2; y < 3; y++)
        {
            for (int x = -2; x < 3; x++)
            {
                Vector3Int finalPos = new Vector3Int(Mathf.CeilToInt(pos.x), pos.y, 0) + new Vector3Int(x, y, 0);

                AiNav.instance.nodes[finalPos.x, finalPos.y].RaycastDown();
            }
        }
    }

    void spawnOre(Vector3Int pos,NetworkConnection conn)
    {
        for (int x = -2; x < 3; x++)
        {
            for (int y = -2; y < 3; y++)
            {

                Vector3Int finalPos = pos + new Vector3Int(x, y, 0);

                if (MapGenerator.instance.OreMap.GetTile(finalPos) != null)
                {
                    int index = MapGenerator.instance.OreMapIndexs[finalPos.x,finalPos.y];
                    Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0, 360));
                    GameObject ore = Instantiate(MapGenerator.instance.ores[index].droppedOre, finalPos + new Vector3(0.5f,0.5f), rot);
                    ore.GetComponent<OreChunk>().oreName = MapGenerator.instance.ores[index].OreName;
                    InstanceFinder.ServerManager.Spawn(ore, conn);
                }
            }
        }
    }
}