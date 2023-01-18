using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Managing;

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

    public Vector3Int bottomLeft;
    public Vector3Int topLeft;
    public Vector3Int bottomRight;
    public Vector3Int topRight;

    void Start()
    {
        //generate starting offsets for each player spawn room
        bottomLeft = new Vector3Int(edgeSize, edgeSize);
        topLeft = new Vector3Int(edgeSize, MapHieght - MapSpawnCircle);
        bottomRight = new Vector3Int(MapWidth - MapSpawnCircle, edgeSize);
        topRight = new Vector3Int(MapWidth - MapSpawnCircle, MapHieght - MapSpawnCircle);

        if (!IsServer)
        {
            generateMap();

            //set player spawn for each corner of map
            setPlayerSpawn(bottomLeft);
            setPlayerSpawn(bottomRight);

            setPlayerSpawn(topLeft);
            setPlayerSpawn(topRight);

            return;
        }

        seed = Random.Range(int.MinValue / 100, int.MaxValue / 100) / 100;

        generateMap();

        //set player spawn for each corner of map
        setPlayerSpawn(bottomLeft);
        setPlayerSpawn(bottomRight);

        setPlayerSpawn(topLeft);
        setPlayerSpawn(topRight);
    }

    public void generateMap()
    {
        for (int y = 0; y < MapHieght; y++)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                spawnMap.SetTile(new Vector3Int(x, y), BGSquare);

                if (y < edgeSize || y > MapHieght - edgeSize || x < edgeSize || x > MapWidth - edgeSize)
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
