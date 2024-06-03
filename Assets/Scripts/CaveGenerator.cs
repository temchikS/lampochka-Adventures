using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;
using System;


public class CaveGenerator : MonoBehaviour
{
    public static CaveGenerator instance { get; private set; }

    public GameObject navmash;
    private NavBaker Baker;

    public int width = 50;
    public int height = 50;
    public int fillPercent = 45;
    public int smoothingIterations = 5;
    private string seed;
    private bool useRandomSeed = true;

    public Tilemap walkableTilemap;
    public Tilemap nonWalkableTilemap;
    public TileBase walkableTile;
    public TileBase nonWalkableTile;
    public TileBase TopWallTile;
    public TileBase LeftWallTile;
    public TileBase RightWallTile;
    public TileBase DownWallTile;
    public TileBase DownRightWallTile;
    public TileBase DownLeftWallTile;

    private int[,] map;
    private void Awake()
    {
        instance = this;
        Baker = navmash.GetComponent<NavBaker>();
    }
    void Start()
    {
        GenerateDungeon();
        
    }
    private Vector2 GetPlayerSpawnPoint()
    {
        System.Random pseudoRandom = new System.Random(seed.GetHashCode());
        for (int y = height - 1; y >= 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                if (map[x, y] == 0)
                {
                    return new Vector2(x, y);
                }
            }
        }
        return new Vector2(width / 2, height / 2);
    }
    public void GenerateDungeon()
    {
        map = new int[width, height];
        RandomFillMap();
        for (int i = 0; i < smoothingIterations; i++)
        {
            SmoothMap();
        }
        ConnectRegions();
        DrawMap();
        Vector2 playerSpawnPoint = GetPlayerSpawnPoint();
        Player.instance.transform.position = new Vector3(playerSpawnPoint.x, playerSpawnPoint.y, 0);
        Baker.BakeMap();
        NPCSpawner.instance.SpawnMobs();
    }
    void RandomFillMap()
    {
        if (useRandomSeed)
        {
            seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue).ToString();
        }

        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    map[x, y] = 1;
                }
                else
                {
                    map[x, y] = (pseudoRandom.Next(0, 100) < fillPercent) ? 1 : 0;
                }
            }
        }
    }
    void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);

                if (neighbourWallTiles > 4)
                    map[x, y] = 1;
                else if (neighbourWallTiles < 4)
                    map[x, y] = 0;
            }
        }
    }
    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += map[neighbourX, neighbourY];
                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }
        return wallCount;
    }
    void ConnectRegions()
    {
        List<List<Vector2Int>> regions = GetRegions(0);

        if (regions.Count <= 1)
        {
            return;
        }

        List<Vector2Int> mainRegion = regions[0];

        for (int i = 1; i < regions.Count; i++)
        {
            List<Vector2Int> bestConnection = null;
            int bestDistance = int.MaxValue;

            foreach (Vector2Int tileA in mainRegion)
            {
                foreach (Vector2Int tileB in regions[i])
                {
                    int distance = (tileA - tileB).sqrMagnitude;
                    if (distance < bestDistance)
                    {
                        bestDistance = distance;
                        bestConnection = new List<Vector2Int> { tileA, tileB };
                    }
                }
            }
            if (bestConnection != null)
            {
                CreatePassage(bestConnection[0], bestConnection[1]);
                mainRegion.AddRange(regions[i]);
            }
        }
    }

    List<List<Vector2Int>> GetRegions(int tileType)
    {
        List<List<Vector2Int>> regions = new List<List<Vector2Int>>();
        int[,] mapFlags = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (mapFlags[x, y] == 0 && map[x, y] == tileType)
                {
                    List<Vector2Int> newRegion = GetRegionTiles(x, y);
                    regions.Add(newRegion);

                    foreach (Vector2Int tile in newRegion)
                    {
                        mapFlags[tile.x, tile.y] = 1;
                    }
                }
            }
        }

        return regions;
    }

    List<Vector2Int> GetRegionTiles(int startX, int startY)
    {
        List<Vector2Int> tiles = new List<Vector2Int>();
        int[,] mapFlags = new int[width, height];
        int tileType = map[startX, startY];

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(new Vector2Int(startX, startY));
        mapFlags[startX, startY] = 1;

        while (queue.Count > 0)
        {
            Vector2Int tile = queue.Dequeue();
            tiles.Add(tile);

            for (int x = tile.x - 1; x <= tile.x + 1; x++)
            {
                for (int y = tile.y - 1; y <= tile.y + 1; y++)
                {
                    if (IsInMapRange(x, y) && (y == tile.y || x == tile.x))
                    {
                        if (mapFlags[x, y] == 0 && map[x, y] == tileType)
                        {
                            mapFlags[x, y] = 1;
                            queue.Enqueue(new Vector2Int(x, y));
                        }
                    }
                }
            }
        }

        return tiles;
    }

    bool IsInMapRange(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    void CreatePassage(Vector2Int tileA, Vector2Int tileB)
    {
        List<Vector2Int> line = GetLine(tileA, tileB);
        foreach (Vector2Int c in line)
        {
            DrawCircle(c, 1);
        }
    }
    List<Vector2Int> GetLine(Vector2Int from, Vector2Int to)
    {
        List<Vector2Int> line = new List<Vector2Int>();

        int x = from.x;
        int y = from.y;

        int dx = to.x - from.x;
        int dy = to.y - from.y;

        bool inverted = false;
        int step = (int)Mathf.Sign(dx);
        int gradientStep = (int)Mathf.Sign(dy);

        int longest = Mathf.Abs(dx);
        int shortest = Mathf.Abs(dy);

        if (longest < shortest)
        {
            inverted = true;
            longest = Mathf.Abs(dy);
            shortest = Mathf.Abs(dx);

            step = (int)Mathf.Sign(dy);
            gradientStep = (int)Mathf.Sign(dx);
        }

        int gradientAccumulation = longest / 2;
        for (int i = 0; i < longest; i++)
        {
            line.Add(new Vector2Int(x, y));

            if (inverted)
            {
                y += step;
            }
            else
            {
                x += step;
            }

            gradientAccumulation += shortest;
            if (gradientAccumulation >= longest)
            {
                if (inverted)
                {
                    x += gradientStep;
                }
                else
                {
                    y += gradientStep;
                }
                gradientAccumulation -= longest;
            }
        }

        return line;
    }
    void DrawCircle(Vector2Int c, int r)
    {
        for (int x = -r; x <= r; x++)
        {
            for (int y = -r; y <= r; y++)
            {
                if (x * x + y * y <= r * r)
                {
                    int drawX = c.x + x;
                    int drawY = c.y + y;
                    if (IsInMapRange(drawX, drawY))
                    {
                        map[drawX, drawY] = 0;
                    }
                }
            }
        }
    }

    void DrawMap()
    {
        walkableTilemap.ClearAllTiles();
        nonWalkableTilemap.ClearAllTiles();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (map[x, y] == 1)
                {
                    nonWalkableTilemap.SetTile(pos, nonWalkableTile);
                }
                else
                {
                    walkableTilemap.SetTile(pos, walkableTile);
                }
            }
        }
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == 1)
                {
                    AddWallsAroundWalkableTile(x, y);
                }
            }
        }
    }
    void AddWallsAroundWalkableTile(int x, int y)
    {
        Vector2Int[] directions = {
        new Vector2Int(-1,  0), // left
        new Vector2Int( 1,  0), // right
        new Vector2Int( 0, -1), // down
        new Vector2Int( 0,  1)  // up
    };
        foreach (Vector2Int dir in directions)
        {
            int newX = x + dir.x;
            int newY = y + dir.y;

            if (IsInMapRange(newX, newY) && map[newX, newY] == 0)
            {
                if (dir == Vector2Int.left)
                {
                    nonWalkableTilemap.SetTile(new Vector3Int(x, y ,0), RightWallTile);
                }
                else if (dir == Vector2Int.right)
                {
                    nonWalkableTilemap.SetTile(new Vector3Int(x, y, 0), LeftWallTile);
                }
                else if (dir == Vector2Int.up)
                {
                    if (IsInMapRange(x - 1, y) && map[x - 1, y] == 0 && IsInMapRange(x + 1, y) && map[x + 1, y] == 0)
                    {
                        nonWalkableTilemap.SetTile(new Vector3Int(x, y, 0), DownWallTile);
                    }
                    else
                    {
                        if (IsInMapRange(x - 1, y) && map[x - 1, y] == 0)
                        {
                            nonWalkableTilemap.SetTile(new Vector3Int(x, y, 0), DownLeftWallTile);
                        }
                        else if (IsInMapRange(x + 1, y) && map[x + 1, y] == 0)
                        {
                            nonWalkableTilemap.SetTile(new Vector3Int(x, y, 0), DownRightWallTile);
                        }
                        else
                        {
                            nonWalkableTilemap.SetTile(new Vector3Int(x, y, 0), DownWallTile);
                        }
                    }
                }
                else if (dir == Vector2Int.down)
                {
                    nonWalkableTilemap.SetTile(new Vector3Int(x, y, 0), TopWallTile);
                }
            }
        }
    }
    public Vector2Int GetRandomWalkableTile()
    {
        List<Vector2Int> walkableTiles = new List<Vector2Int>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == 0) // Проверяем, является ли ячейка проходимой
                {
                    if (map[x - 1, y] == 0 && map[x + 1, y] == 0 && map[x, y - 1] == 0 && map[x, y + 1] == 0)
                    {
                        walkableTiles.Add(new Vector2Int(x, y));
                    }
                }
            }
        }

        if (walkableTiles.Count == 0)
        {
            Debug.LogWarning("No walkable tiles found.");
            return Vector2Int.zero;
        }

        int randomIndex = UnityEngine.Random.Range(0, walkableTiles.Count);
        Vector2Int randomTile = walkableTiles[randomIndex];
        return new Vector2Int(randomTile.x, randomTile.y);
    }
}