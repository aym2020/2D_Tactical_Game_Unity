using System.Collections.Generic;
using UnityEngine;

public class Pathfinder
{
    private class Node
    {
        public Tile Tile;
        public Node Parent;
        public int GCost;
        public int HCost;
        public int FCost => GCost + HCost;

        public Node(Tile tile, Node parent, int gCost, int hCost)
        {
            Tile = tile;
            Parent = parent;
            GCost = gCost;
            HCost = hCost;
        }
    }

    public static List<Tile> GetPath(Tile startTile, Tile targetTile)
    {
        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();

        Node startNode = new Node(startTile, null, 0, CalculateHCost(startTile, targetTile));
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Node currentNode = openList[0];

            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].FCost < currentNode.FCost || openList[i].FCost == currentNode.FCost && openList[i].HCost < currentNode.HCost)
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode.Tile == targetTile)
            {
                return RetracePath(currentNode);
            }

            foreach (Tile neighbor in GridManager.Instance.GetNeighbors(currentNode.Tile))
            {
                if (!neighbor.Walkable || closedList.Contains(new Node(neighbor, null, 0, 0)))
                {
                    continue;
                }

                int newGCost = currentNode.GCost + CalculateHCost(currentNode.Tile, neighbor);
                Node neighborNode = new Node(neighbor, currentNode, newGCost, CalculateHCost(neighbor, targetTile));

                if (newGCost < neighborNode.GCost || !openList.Contains(neighborNode))
                {
                    neighborNode.GCost = newGCost;
                    neighborNode.HCost = CalculateHCost(neighbor, targetTile);
                    neighborNode.Parent = currentNode;

                    if (!openList.Contains(neighborNode))
                    {
                        openList.Add(neighborNode);
                    }
                }
            }
        }

        return null;
    }

    private static int CalculateHCost(Tile tileA, Tile tileB)
    {
        int distX = Mathf.Abs(tileA.X - tileB.X);
        int distY = Mathf.Abs(tileA.Y - tileB.Y);

        return distX + distY;
    }

    private static List<Tile> RetracePath(Node endNode)
    {
        List<Tile> path = new List<Tile>();
        Node currentNode = endNode;

        while (currentNode != null)
        {
            path.Add(currentNode.Tile);
            currentNode = currentNode.Parent;
        }

        path.Reverse();
        return path;
    }
}

