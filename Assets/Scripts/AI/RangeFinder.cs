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

    public static bool HasLineOfSight(Tile origin, Tile target, int ignorePos = 100)
    {
        // Initialize start and end positions
        int x0 = origin.X;
        int y0 = origin.Y;
        int x1 = target.X;
        int y1 = target.Y;

        // Calculate the differences in x and y
        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);

        // Determine the direction of the line
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;

        // Get the grid size from the GridManager instance
        int gridSize = GridManager.Instance.GetFieldSize();

        // Flag to track if the line of sight is clear
        bool inSight = true;

        // Initialize variables for the Bresenham's line algorithm
        int i = 2 * dx;
        int c = 2 * dy;
        int p = dx - dy;
        int d = -1 + dx + dy;

        // Move the starting point one step closer to the target
        for (int m = 0; m < 1; m++)
        {
            if (p > 0)
            {
                x0 += sx;
                p -= c;
            }
            else if (p < 0)
            {
                y0 += sy;
                p += i;
            }
            else
            {
                x0 += sx;
                p -= c;
                y0 += sy;
                p += i;
                d--;
            }
        }

        // Iterate through the points along the line
        while (d > 0 && inSight)
        {
            int tileIndex = y0 * gridSize + x0;
            Tile tile = GridManager.Instance.GetTileAtPosition(new Vector2(x0, y0));

            // Check if the current tile is not walkable and is not the ignorePos
            if (tile != null && !tile.Walkable && tileIndex != ignorePos)
            {
                inSight = false;
            }
            else
            {
                // Move to the next point along the line
                if (p > 0)
                {
                    x0 += sx;
                    p -= c;
                }
                else if (p < 0)
                {
                    y0 += sy;
                    p += i;
                }
                else
                {
                    x0 += sx;
                    p -= c;
                    y0 += sy;
                    p += i;
                    d--;
                }
                d--;
            }
        }

        // Return true if the line of sight is clear, false otherwise
        return inSight;
    }

}
