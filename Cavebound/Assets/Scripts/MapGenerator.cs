using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class MapGenerator : NetworkBehaviour
{
    [SyncVar]
    public float seed = 0;

    public int MapWidth = 5;
    public int MapHieght = 5;
    public int MapSpawnSize = 5;

    public Tilemap GroundMap;
    public Tilemap BorderMap;
    public Tilemap BackgroundMap;
    public Tilemap OreMap;

    public Tile GroundSquare;
    public Tile borderSquare;
    public Tile BGSquare;

    public List<Ore> ores= new List<Ore>();
    
    public int[,] OreMapIndexs = new int[0,0];

    public float NoiseScale = 1.0f;
    public int edgeSize = 10;

    private Vector3Int bottomLeft;
    private Vector3Int topLeft;
    private Vector3Int bottomRight;
    private Vector3Int topRight;

    public static MapGenerator instance;
    public bool onBlockSpawn = false;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (instance == null)
        {
            instance = this;
        }

        //generate starting offsets for each player spawn room
        bottomLeft = new Vector3Int(edgeSize, edgeSize);
        topLeft = new Vector3Int(edgeSize, MapHieght - MapSpawnSize - edgeSize);
        bottomRight = new Vector3Int(MapWidth - MapSpawnSize - edgeSize, edgeSize);
        topRight = new Vector3Int(MapWidth - MapSpawnSize - edgeSize, MapHieght - MapSpawnSize - edgeSize);

        OreMapIndexs = new int[MapWidth, MapHieght];

        generateMap();

        for (int i = 0; i < ores.Count; i++)
        {
            generateOre(ores[i], i);
        }

        generateColliders();

        //set player spawn for each corner of map
        setPlayerSpawn(bottomLeft);
        setPlayerSpawn(bottomRight);
        setPlayerSpawn(topLeft);
        setPlayerSpawn(topRight);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        seed = Random.Range(int.MinValue + 10000, int.MaxValue - 10000) / 100;
    }

    public void generateMap()
    {
        for (int y = 0; y < MapHieght; y++)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                BackgroundMap.SetTile(new Vector3Int(x, y), BGSquare);
                int range = Random.Range(0, 4);
                BackgroundMap.SetTransformMatrix(new Vector3Int(x, y), Matrix4x4.Rotate(Quaternion.Euler(0, 0, 90f * range)));

                if (y < edgeSize || y > MapHieght - edgeSize || x < edgeSize || x > MapWidth - edgeSize)
                {
                    BorderMap.SetTile(new Vector3Int(x, y), borderSquare);
                    range = Random.Range(0, 4);
                    BorderMap.SetTransformMatrix(new Vector3Int(x, y), Matrix4x4.Rotate(Quaternion.Euler(0, 0, 90f * range)));
                    continue;
                }

                float xCoord = (seed + x) / MapWidth * NoiseScale;
                float yCoord = (seed + y) / MapWidth * NoiseScale;

                float sample = Mathf.PerlinNoise(xCoord, yCoord);

                if (sample >= 0.45f && sample <= 0.75f)
                {
                    GroundMap.SetTile(new Vector3Int(x, y), GroundSquare);
                    range = Random.Range(0, 4);
                    GroundMap.SetTransformMatrix(new Vector3Int(x, y), Matrix4x4.Rotate(Quaternion.Euler(0, 0, 90f * range)));
                }
            }
        }
    }

    public void generateOre(Ore t_Ore, int index)
    {
        for (int y = 0; y < MapHieght; y++)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                float xCoord = (seed + x) / MapWidth * (NoiseScale / t_Ore.noiseScaleMod);
                float yCoord = (seed + y) / MapWidth * (NoiseScale / t_Ore.noiseScaleMod);

                float sample = Mathf.PerlinNoise(xCoord, yCoord);

                if (sample >= t_Ore.minRarity && sample <= t_Ore.maxRarity)
                {
                    if (GroundMap.GetTile(new Vector3Int(x, y)) != null)
                    {
                        if (OreMap.GetTile(new Vector3Int(x, y)) == null)
                        {
                            OreMap.SetTile(new Vector3Int(x, y), t_Ore.OreTile);

                            int range = Random.Range(0, 4);

                            OreMap.SetTransformMatrix(new Vector3Int(x, y), Matrix4x4.Rotate(Quaternion.Euler(0, 0, 90f * range)));

                            OreMapIndexs[x, y] = index;
                        }
                    }
                }
            }
        }
    }

    public void setPlayerSpawn(Vector3Int coords)
    { 
        for (int y = 0; y < MapSpawnSize; y++)
        {
            for (int x = 0; x < MapSpawnSize; x++)
            {
                if(x == 0 || y == 0 || x == MapSpawnSize - 1 || y == MapSpawnSize - 1)
                {
                    GroundMap.SetTile(new Vector3Int(coords.x + x, coords.y + y), GroundSquare);
                    continue;
                }

                BackgroundMap.SetTile(new Vector3Int(coords.x + x, coords.y + y), BGSquare);
                GroundMap.SetTile(new Vector3Int(coords.x + x, coords.y + y), null);
                OreMap.SetTile(new Vector3Int(coords.x + x, coords.y + y), null);
            }
        }
    }

    public void generateColliders()
    {
        //GroundMap.GetComponent<CompositeCollider2D>().GenerateGeometry();
        //BorderMap.GetComponent<CompositeCollider2D>().GenerateGeometry();
    }

    public Vector3 FindSafeSpawn()
    {
        int x = Random.Range(MapSpawnSize + edgeSize + 2, MapWidth - MapSpawnSize - (edgeSize + 2));
        int y = Random.Range(MapSpawnSize + edgeSize + 2, MapHieght - MapSpawnSize - (edgeSize + 2));

        if (!onBlockSpawn)
        {
            if (GroundMap.GetTile(new Vector3Int(x, y)) == null)
            {
                return new Vector3(x + .5f, y + .5f, 0);
            }
            else
            {
                return FindSafeSpawn();
            }
        }
        else
        {
            if (GroundMap.GetTile(new Vector3Int(x, y)) != null)
            {
                if (GroundMap.GetTile(new Vector3Int(x, y + 1)) == null)
                {
                    return new Vector3(x + .5f, y + 1 + .5f, 0);
                }
                else
                {
                    return FindSafeSpawn();
                }
            }
            else
            {
                return FindSafeSpawn();
            }
        }
    }
}
