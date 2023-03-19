using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class AiNav : NetworkBehaviour
{
    public Tilemap tilemap;

    public int width;
    public int height;

    public Node[,] nodes;

    public GameObject NodePrefab;

    public static AiNav instance;

    public override void OnStartServer()
    {
        base.OnStartServer();     
        Invoke("SetAINodes", 2.0f);
    }

    void SetAINodes()
    {
        instance = this;
        nodes = new Node[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject temp = Instantiate(NodePrefab, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity, transform);
                nodes[x, y] = temp.GetComponent<Node>();
                nodes[x, y].startNode();
            }
        }
    }
}
