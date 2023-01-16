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

    Vector3Int bottomLeft;
    Vector3Int topLeft;
    Vector3Int bottomRight;
    Vector3Int topRight;

    private void Start()
    {
        //generate starting offsets for each player spawn room
        bottomLeft = new Vector3Int(edgeSize, edgeSize);
        topLeft = new Vector3Int(edgeSize, MapHieght - MapSpawnCircle);
        bottomRight = new Vector3Int(MapWidth - MapSpawnCircle, edgeSize);
        topRight = new Vector3Int(MapWidth - MapSpawnCircle, MapHieght - MapSpawnCircle);

        if (!isServer)
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

        movePlayerToSpawnPosCommand();
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

    [Command(requiresAuthority = false)]
    public void movePlayerToSpawnPosCommand()
    {
        Vector3Int offsetAmount = new Vector3Int(MapSpawnCircle / 2, MapSpawnCircle / 2, 0);

        MyNetworkManager manager = FindObjectOfType<MyNetworkManager>();

        if (manager.ActivePlayerObjects.Count == 1)
        {
            movePlayerToSpawnPosRPC(bottomLeft + offsetAmount, manager.ActivePlayerObjects[0]);
        }
        if (manager.ActivePlayerObjects.Count == 2)
        {
            movePlayerToSpawnPosRPC(bottomLeft + offsetAmount, manager.ActivePlayerObjects[0]);
            movePlayerToSpawnPosRPC(bottomRight + offsetAmount, manager.ActivePlayerObjects[1]);
        }
        if (manager.ActivePlayerObjects.Count == 3)
        {
            movePlayerToSpawnPosRPC(bottomLeft + offsetAmount, manager.ActivePlayerObjects[0]);
            movePlayerToSpawnPosRPC(bottomRight + offsetAmount, manager.ActivePlayerObjects[1]);
            movePlayerToSpawnPosRPC(topLeft + offsetAmount, manager.ActivePlayerObjects[2]);
        }
        if (manager.ActivePlayerObjects.Count == 4)
        {
            movePlayerToSpawnPosRPC(bottomLeft + offsetAmount, manager.ActivePlayerObjects[0]);
            movePlayerToSpawnPosRPC(bottomRight + offsetAmount, manager.ActivePlayerObjects[1]);
            movePlayerToSpawnPosRPC(topLeft + offsetAmount, manager.ActivePlayerObjects[2]);
            movePlayerToSpawnPosRPC(topRight + offsetAmount, manager.ActivePlayerObjects[3]);
        }
    }

    [ClientRpc]
    public void movePlayerToSpawnPosRPC(Vector3Int coords,GameObject t_player)
    {
        t_player.transform.position = coords;
    }
}
