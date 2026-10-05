using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridMap : MonoBehaviour
{
    private Level[] levels;
    public int LevelCount { get { return levels.Length; } }

    private Grid grid;
    private Dictionary<Vector3Int, Tile> tiles = new();

    public List<Gate> Gates { get; private set; } = new List<Gate>();
    public List<Key> Keys { get; private set; } = new List<Key>();

    public StartPoint CurrentStartPoint { get; private set; }

    private void Awake()
    {
        grid = GetComponent<Grid>();
        levels = GetComponentsInChildren<Level>();
    }

    public void SetLevelCamera()
    {
        if (levels == null) return;

        int index = LevelManager.Instance.CurrentLevel - 1;
        Camera.main.transform.position = levels[index].levelCamera.position;
    }

    public void RegisterLevelTile()
    {
        if (levels == null) return;

        int index = LevelManager.Instance.CurrentLevel - 1;
        RegisterGameObjectTiles(levels[index].groundTilemap);
    }

    public void RegisterGameObjectTiles(Tilemap tilemap)
    {
        if (tilemap == null) return;

        tiles.Clear();
        Gates.Clear();
        Keys.Clear();
        CurrentStartPoint = null;

        foreach (Transform child in tilemap.transform)
        {
            if (!child.TryGetComponent(out Tile tile)) continue;

            if (child.TryGetComponent(out Gate gate))
            {
                Gates.Add(gate);
            }

            if (child.TryGetComponent(out Key key))
            {
                Keys.Add(key);
            }

            Vector3Int cellPos = grid.WorldToCell(child.position);
            tiles[cellPos] = tile;

            if (tile.type == TileType.Start)
                CurrentStartPoint = child.GetComponent<StartPoint>();
        }
    }

    public bool TryGetTileType(Vector3 world, out TileType type)
    {
        Tile tile = GetTile(world);

        if (tile != null)
        {
            type = tile.type;
            return true;
        }
        type = default;
        return false;
    }

    public bool TryGetTileType(Vector3Int cell, out TileType type)
    {
        if (tiles.TryGetValue(cell, out Tile tile))
        {
            type = tile.type;
            return true;
        }
        type = default; 
        return false;
    }

    public Tile GetTile(Vector3 world)
    {
        if (grid == null) return null;

        Vector3Int cellPos = grid.WorldToCell(world);
        return tiles.TryGetValue(cellPos, out Tile tile) ? tile : null;
    }

    public Vector3Int GetStartCell()
    {
        if (grid != null)
        {
            return grid.WorldToCell(CurrentStartPoint.transform.position);
        }
        return Vector3Int.zero;
    }

    public Vector3 CellToWorld(Vector2Int cell)
    {
        return grid.CellToWorld(new Vector3Int(cell.x, cell.y, 0));
    }
}