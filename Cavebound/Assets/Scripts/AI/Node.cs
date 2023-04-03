using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Node : MonoBehaviour 
{
    public int gCost, hCost;

    public LayerMask mask;

    public int localX, localY;

    public bool greenNode;

    public List<Node> connectingNodes;

    public Node parent;

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public void startNode()
    {
        localX = (int)transform.position.x;
        localY = (int)transform.position.y;
        RaycastDown();
    }

    public void RaycastDown()
    {
        Tilemap tilemap = AiNav.instance.tilemap;

        if (tilemap.GetTile(new Vector3Int(localX, localY, 0)) != null)
            greenNode = false;
        else
            greenNode = true;

    }

    public List<Node> GetNeighbours()
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for(int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                int checkX = localX + x;
                int checkY = localY + y;

                if (checkX >= 0 && checkX < AiNav.instance.width && checkY >= 0 && checkY < AiNav.instance.height)
                {
                    neighbours.Add(AiNav.instance.nodes[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }
}
