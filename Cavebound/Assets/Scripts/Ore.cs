using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

//allows me to create a new ore in unity
[CreateAssetMenu(fileName = "Ore", menuName = "Ore/OreAsset", order = 1)]
public class Ore : ScriptableObject
{
    public string OreName;
    public Tile OreTile;
    public GameObject droppedOre;
    public float valueWorth;
    public float minRarity;
    public float maxRarity;
    public float noiseScaleMod;
}
