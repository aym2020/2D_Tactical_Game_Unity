using System.Collections.Generic;
using UnityEngine;

public class RangeFinder
{
    public static List<Tile> GetMovementRangeTiles(Tile originTile, int movementPoints)
    {
        List<Tile> availableTiles = new List<Tile>();
        Queue<TileNode> queue = new Queue<TileNode>();
        HashSet<Tile> visitedTiles = new HashSet<Tile>();

        queue.Enqueue(new TileNode(originTile, 0));
        visitedTiles.Add(originTile);

        while (queue.Count > 0)
        {
            TileNode currentNode = queue.Dequeue();
            Tile currentTile = currentNode.tile;
            int currentDistance = currentNode.distance;

            /*Debug.Log($"Processing tile {currentTile.name} with distance {currentDistance}");*/

            if (currentDistance < movementPoints)
            {
                List<Tile> neighbors = GridManager.Instance.GetNeighbors(currentTile);

                foreach (Tile neighbor in neighbors)
                {
                    if (!visitedTiles.Contains(neighbor) && neighbor.Walkable)
                    {
                        queue.Enqueue(new TileNode(neighbor, currentDistance + 1));
                        visitedTiles.Add(neighbor);
                        availableTiles.Add(neighbor);
                    }
                }
            }
        }

        foreach (var tile in availableTiles)
        {
            /*Debug.Log($"Available tile : {tile.name}");*/
        }

        return availableTiles;
    }

    private class TileNode
    {
        public Tile tile;
        public int distance;

        public TileNode(Tile tile, int distance)
        {
            this.tile = tile;
            this.distance = distance;
        }
    }
}
