using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using UnityEngine.Rendering;
using FishNet.Object;

public class MapGenUI : MonoBehaviour
{
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

    public List<Ore> ores = new List<Ore>();

    public int[,] OreMapIndexs = new int[0, 0];

    public float NoiseScale = 1.0f;
    public int edgeSize = 10;

    public static MapGenerator instance;
    public bool onBlockSpawn = false;

    public void Start()
    {
        OreMapIndexs = new int[MapWidth, MapHieght];
        seed = Random.Range(int.MinValue + 10000, int.MaxValue - 10000) / 100;

        generateMap();

        for (int i = 0; i < ores.Count; i++)
        {
            generateOre(ores[i], i);
        }
    }

    public void clearMap()
    {
        GroundMap.ClearAllTiles();
        BorderMap.ClearAllTiles();
        BackgroundMap.ClearAllTiles();
        OreMap.ClearAllTiles();
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
}
