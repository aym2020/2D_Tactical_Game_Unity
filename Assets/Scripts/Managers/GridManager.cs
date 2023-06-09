using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GridManager : MonoBehaviour
{

    public static GridManager Instance;

    [SerializeField] private Tile groundTile, mountainTile;
    [SerializeField] private int _fieldSize;

    // getter and setter methods
    public int GetFieldSize() => _fieldSize;
    
    public Camera _mainCamera;
    public Dictionary<Vector2, Tile> _tiles;

    private void Awake()
    {
        Instance = this;
    }

    // Generate a grid of tiles
    public void GenerateGrid()
    {
        _tiles = new Dictionary<Vector2, Tile>();
        for (int x = 0; x < _fieldSize; x++)
        {
            for (int y = 0; y < _fieldSize; y++)
            {
                var randomTile = Random.Range(0, 6) == 3 ? mountainTile : groundTile;
                var spawnedTile = Instantiate(randomTile, new Vector3(x, y), Quaternion.identity, transform);
                spawnedTile.name = $"Tile ({x}, {y})";

                spawnedTile.Init(x, y);

                _tiles[new Vector2(x, y)] = spawnedTile;
            }
        }

        _mainCamera.transform.position = new Vector3((float)_fieldSize / 2 - 0.5f, (float)_fieldSize / 2 - 0.5f, -10);

        // Adjust the camera's orthographic size based on the grid size
        float aspectRatio = (float)Screen.width / (float)Screen.height;
        float gridHeight = _fieldSize;
        float gridWidth = _fieldSize;

        if (gridWidth / gridHeight > aspectRatio)
        {
            _mainCamera.orthographicSize = gridWidth / (2 * aspectRatio);
        }
        else
        {
            _mainCamera.orthographicSize = gridHeight / 2;
        }

        GameManager.Instance.ChangeState(GameState.SpawnHeroes);
    }

    // Spawn heroes and enemies
    public Tile GetHeroSpawnTile()
    {
        return _tiles.Where(t => t.Key.x < _fieldSize / 2 && t.Value.Walkable).OrderBy(t => Random.value).First().Value;
    }

    public Tile GetEnemySpawnTile()
    {
        return _tiles.Where(t => t.Key.x > _fieldSize / 2 && t.Value.Walkable).OrderBy(t => Random.value).First().Value;
    }

    public Tile GetTileAtPosition(Vector2 pos)
    {
        if(_tiles.TryGetValue(pos, out var tile))
        {
            return tile;
        }
        return null;

    }

    // Get all neighbors of a tile
    public List<Tile> GetNeighbors(Tile tile)
    {
        List<Tile> neighbors = new List<Tile>();

        int x = tile.X;
        int y = tile.Y;
        
        // Top
        if (y + 1 < _fieldSize)
        {
            neighbors.Add(_tiles[new Vector2(x, y + 1)]);
        }

        // Bottom
        if (y - 1 >= 0)
        {
            neighbors.Add(_tiles[new Vector2(x, y - 1)]);
        }

        // Right
        if (x + 1 < _fieldSize)
        {
            neighbors.Add(_tiles[new Vector2(x + 1, y)]);
        }

        // Left
        if (x - 1 >= 0)
        {
            neighbors.Add(_tiles[new Vector2(x - 1, y)]);
        }

        return neighbors;
    }

}
