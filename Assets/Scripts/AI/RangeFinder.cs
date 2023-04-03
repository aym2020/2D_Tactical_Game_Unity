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

    public static List<Tile> GetSpellRangeTiles(Tile originTile, int spellRange)
    {
        List<Tile> rangeTiles = new List<Tile>();

        foreach (Tile tile in GridManager.Instance._tiles.Values)
        {
            int distance = originTile.CalculateDistance(tile);
            if (distance <= spellRange && distance > 0 && HasLineOfSight(originTile, tile))
            {
                rangeTiles.Add(tile);
            }
        }

        return rangeTiles;
    }

    private static bool HasLineOfSight(Tile start, Tile end)
    {
        int x0 = start.X;
        int y0 = start.Y;
        int x1 = end.X;
        int y1 = end.Y;

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;

        int err = dx - dy;

        while (true)
        {
            if (x0 == x1 && y0 == y1) break;

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }

            Tile currentTile = GridManager.Instance.GetTileAtPosition(new Vector2(x0, y0));
            if (currentTile == start || currentTile == end) continue;

            if (currentTile != null && currentTile.isObstacle)
            {
                return false;
            }
        }

        return true;
    }

}
