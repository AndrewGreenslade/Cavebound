using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class MapGenerator : MonoBehaviour
{
    public float seed = 0;
    public int MapWidth = 5;
    public int MapHieght = 5;
    public Tilemap map;
    public Tile WhiteSquare;
    public Tile borderSquare;

    public float NoiseScale = 1.0f;
    public int edgeSize = 10;

    private void Awake()
    {
        generateMap();
    }

    public void generateMap()
    {
        seed = Random.Range(int.MinValue / 100, int.MaxValue / 100) / 100;

        for (int y = 0; y < MapHieght; y++)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                if (y <= edgeSize || y >= MapHieght - edgeSize || x <= edgeSize || x >= MapWidth - edgeSize)
                {
                    map.SetTile(new Vector3Int(x, y), borderSquare);
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

    public void clearMap()
    {
        map.ClearAllTiles();
    }
}
