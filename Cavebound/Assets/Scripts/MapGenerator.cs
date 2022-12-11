using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using Mirror;

public class MapGenerator : NetworkBehaviour
{
    [SyncVar]
    public float seed = 0;

    public int MapWidth = 5;
    public int MapHieght = 5;
    public int MapSpawnCircle = 5;

    public Tilemap map;
    public Tilemap BorderMap;
    public Tilemap spawnMap;

    public Tile WhiteSquare;
    public Tile borderSquare;
    public Tile BGSquare;
    public Tile spawnSquare;

    public float NoiseScale = 1.0f;
    public int edgeSize = 10;

    private void Start()
    {
        if (!isServer)
        {
            generateMap();
            return;
        }

        seed = Random.Range(int.MinValue / 100, int.MaxValue / 100) / 100;

        generateMap();
    }

    public void generateMap()
    {
        for (int y = 0; y < MapHieght; y++)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                spawnMap.SetTile(new Vector3Int(x, y), BGSquare);

                if (y <= edgeSize || y >= MapHieght - edgeSize || x <= edgeSize || x >= MapWidth - edgeSize)
                {
                    BorderMap.SetTile(new Vector3Int(x, y), borderSquare);
                    continue;
                }

                float xCoord = (seed + x) / MapWidth * NoiseScale;
                float yCoord = (seed + y) / MapWidth * NoiseScale;

                float sample = Mathf.PerlinNoise(xCoord, yCoord);

                if (sample >= 0.45f && sample <= 0.75f)
                {
                    map.SetTile(new Vector3Int(x, y), WhiteSquare);
                }
            }
        }

        //Bottom left player gen
        Vector3Int bottomLeft = new Vector3Int(edgeSize, edgeSize);
        Vector3Int topLeft = new Vector3Int(edgeSize, MapHieght - MapSpawnCircle);
        Vector3Int bottomRight = new Vector3Int(MapWidth - MapSpawnCircle, edgeSize);
        Vector3Int topRight = new Vector3Int(MapWidth - MapSpawnCircle, MapHieght - MapSpawnCircle);

        setPlayerSpawn(bottomRight);
        setPlayerSpawn(bottomLeft);
        setPlayerSpawn(topLeft);
        setPlayerSpawn(topRight);
    }

    public void setPlayerSpawn(Vector3Int coords)
    { 
        for (int y = 0; y < MapSpawnCircle; y++)
        {
            for (int x = 0; x < MapSpawnCircle; x++)
            {
                if(x == 0 || y == 0 || x == MapSpawnCircle - 1 || y == MapSpawnCircle - 1)
                {
                    map.SetTile(new Vector3Int(coords.x + x, coords.y + y), WhiteSquare);
                    continue;
                }

                spawnMap.SetTile(new Vector3Int(coords.x + x, coords.y + y), spawnSquare);
                map.SetTile(new Vector3Int(coords.x + x, coords.y + y), null);
            }
        }
    }
}
