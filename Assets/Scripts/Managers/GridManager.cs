using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GridManager : MonoBehaviour
{

    public static GridManager Instance;

    [SerializeField] private Tile grassTile, mountainTile;
    [SerializeField] private int _width, _height;
    
    public Camera _mainCamera;
    private Dictionary<Vector2, Tile> _tiles;

    private void Awake()
    {
        Instance = this;
    }

    public void GenerateGrid()
    {
        _tiles = new Dictionary<Vector2, Tile>();
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var randomTile = Random.Range(0, 6) == 3 ? mountainTile : grassTile;
                var spawnedTile = Instantiate(randomTile, new Vector3(x, y), Quaternion.identity, transform);
                spawnedTile.name = $"Tile ({x}, {y})";

                spawnedTile.Init(x, y);

                /*Debug.Log($"Generated tile {spawnedTile.name} ({x}, {y}), Tile coordinates: ({spawnedTile.X}, {spawnedTile.Y})");*/

                _tiles[new Vector2(x, y)] = spawnedTile;
            }
        }

        _mainCamera.transform.position = new Vector3((float)_width / 2 - 0.5f, (float)_height / 2 - 0.5f, -10);

        // Test GetNeighbors for a specific tile
        Tile testTile = _tiles[new Vector2(3, 3)];
        GetNeighbors(testTile);

        GameManager.Instance.ChangeState(GameState.SpawnHeroes);
    }

    public Tile GetHeroSpawnTile()
    {
        return _tiles.Where(t => t.Key.x < _width / 2 && t.Value.Walkable).OrderBy(t => Random.value).First().Value;
    }

    public Tile GetEnemySpawnTile()
    {
        return _tiles.Where(t => t.Key.x > _width / 2 && t.Value.Walkable).OrderBy(t => Random.value).First().Value;
    }

    public Tile GetTileAtPosition(Vector2 pos)
    {
        if(_tiles.TryGetValue(pos, out var tile))
        {
            return tile;
        }
        return null;

    }

    public List<Tile> GetNeighbors(Tile tile)
    {
        List<Tile> neighbors = new List<Tile>();

        int x = tile.X;
        int y = tile.Y;
        
        /*Debug.Log($"Tile coordinate {tile.name} ({x}, {y}");*/

        // Top
        if (y + 1 < _height)
        {
            neighbors.Add(_tiles[new Vector2(x, y + 1)]);
        }

        // Bottom
        if (y - 1 >= 0)
        {
            neighbors.Add(_tiles[new Vector2(x, y - 1)]);
        }

        // Right
        if (x + 1 < _width)
        {
            neighbors.Add(_tiles[new Vector2(x + 1, y)]);
        }

        // Left
        if (x - 1 >= 0)
        {
            neighbors.Add(_tiles[new Vector2(x - 1, y)]);
        }

        /*Debug.Log($"Neighbors of tile {tile.name}: {string.Join(", ", neighbors.Select(n => n.name))}");*/

        return neighbors;
    }

}

