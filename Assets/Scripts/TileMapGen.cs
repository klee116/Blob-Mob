﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Photon.Pun;

public class TileMapGen : MonoBehaviour
{
    [System.Serializable]
    public struct MapGenOption {
        public TileBase tileBase;
        public int minDistBetweenTiles;
    }
    //public static TileMapGen Instance{set;get;}
    public MapGenOption[] mapGenOptions;
    public TileBase defaultTile;
    public int testSeed;
    public TileBase ground1, ground2, ground3;
    public BoundsInt tileArea;
    private Tilemap tileMap;
    private int[,] intMap;

    public void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            object seed;
            PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(RoomLauncher.SEED_PROP_KEY, out seed);
            GenerateWithSeed((int)seed);
        }
        else
        {
            GenerateWithSeed(testSeed);
        }
    }

    void Update()
    {
        #if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GenerateWithSeed(testSeed);
            }
        #endif
    }
    
    public void Generate()
    {
        intMap = new int[tileArea.size.x, tileArea.size.y];
        // PoissonPlace(1, 5, ref intMap, 30);
        // PoissonPlace(2, 5, ref intMap, 30);

        for (int i = 0; i < mapGenOptions.Length; i++)
        {
            PoissonPlace(i + 1, mapGenOptions[i].minDistBetweenTiles, ref intMap, 30);
        }

        tileMap = GetComponent<Tilemap>();
        TileBase[] tileArray = new TileBase[tileArea.size.x * tileArea.size.y * tileArea.size.z];
        for (int y = 0; y < tileArea.size.y; y++)
        {
            for (int x = 0; x < tileArea.size.x; x++)
            {
                if ((x > 1) && (y > 1) && (x < tileArea.size.x - 2) && (y < tileArea.size.y -2))
                {
                    if (intMap[x, y] > 0)
                    {
                        tileArray[y * tileArea.size.x + x] = mapGenOptions[intMap[x, y] - 1].tileBase;
                    }
                    else
                    {
                        tileArray[y * tileArea.size.x + x] = defaultTile;
                    }
                }
            }
        }

        tileMap.SetTilesBlock(tileArea, tileArray);
    }

    public void GenerateWithSeed(int seed)
    {
        System.Random generator = new System.Random(seed);

        intMap = new int[tileArea.size.x, tileArea.size.y];
        //PoissonPlaceSeed(1, 5, ref generator, ref intMap, 30);
        //PoissonPlaceSeed(2, 5, ref generator, ref intMap, 30);

        for (int i = 0; i < mapGenOptions.Length; i++)
        {
            PoissonPlaceSeed(i + 1, mapGenOptions[i].minDistBetweenTiles, ref generator, ref intMap, 30);
        }

        tileMap = GetComponent<Tilemap>();
        TileBase[] tileArray = new TileBase[tileArea.size.x * tileArea.size.y * tileArea.size.z];
        for (int y = 0; y < tileArea.size.y; y++)
        {
            for (int x = 0; x < tileArea.size.x; x++)
            {
                if (intMap[x, y] > 0)
                {
                    tileArray[y * tileArea.size.x + x] = mapGenOptions[intMap[x, y] - 1].tileBase;
                }
                else
                {
                    tileArray[y * tileArea.size.x + x] = defaultTile;
                }
            }
        }

        tileMap.SetTilesBlock(tileArea, tileArray);
    }

    public List<Vector2Int> PoissonPlace(int tileType, int radius, ref int[,] tileMap, int rejectionLimit = 30)
    {
        List<Vector2Int> points = new List<Vector2Int>();

        Queue<Vector2Int> activeSample = new Queue<Vector2Int>();

        float cellSize = radius / 1.4f;
        int[,] bg_grid = new int[Mathf.CeilToInt(tileMap.GetLength(0) / cellSize), Mathf.CeilToInt(tileMap.GetLength(1) / cellSize)];
        for (int i = 0; i < bg_grid.GetLength(0); i++) 
        {
            for (int j = 0; j < bg_grid.GetLength(1); j++) 
            {
                bg_grid[i, j] = -1;
            }
        }       

        Vector2Int initialPoint = new Vector2Int(Random.Range(0, tileMap.GetLength(0)), Random.Range(0, tileMap.GetLength(1)));
        activeSample.Enqueue(initialPoint);
        points.Add(initialPoint);
        tileMap[initialPoint.x, initialPoint.y] = tileType;
        bg_grid[Mathf.FloorToInt(initialPoint.x / cellSize), Mathf.FloorToInt(initialPoint.y / cellSize)] = 0;

        while (activeSample.Count > 0)
        {
            Vector2Int chosenSample = activeSample.Peek();
            bool foundCandidate = false;

            for (int i = 0; i < rejectionLimit; i++)
            {
                Vector2Int potentialSample = chosenSample;
                Vector2 randomDir = Random.insideUnitCircle;
                randomDir = randomDir.normalized * radius + randomDir * radius;

                potentialSample.x += Mathf.RoundToInt(randomDir.x);
                potentialSample.y += Mathf.RoundToInt(randomDir.y);

                if (potentialSample.x < 0 || potentialSample.y < 0 || potentialSample.x >= tileMap.GetLength(0) || potentialSample.y >= tileMap.GetLength(1))
                {
                    continue;
                }

                //Debug.Log("Checking " + potentialSample);

                if (poissonGridCheck(potentialSample, cellSize, radius, ref bg_grid, ref points))
                {
                    foundCandidate = true;
                    activeSample.Enqueue(potentialSample);
                    bg_grid[Mathf.FloorToInt(initialPoint.x / cellSize), Mathf.FloorToInt(initialPoint.y / cellSize)] = points.Count;
                    points.Add(potentialSample);
                    tileMap[potentialSample.x, potentialSample.y] = tileType;
                }
            }

            if (!foundCandidate)
            {
                activeSample.Dequeue();
            }
        }
        return points;
    }

    public List<Vector2Int> PoissonPlaceSeed(int tileType, int radius, ref System.Random rand, ref int[,] tileMap, int rejectionLimit = 30)
    {
        List<Vector2Int> points = new List<Vector2Int>();

        Queue<Vector2Int> activeSample = new Queue<Vector2Int>();

        float cellSize = radius / 1.4f;
        int[,] bg_grid = new int[Mathf.CeilToInt(tileMap.GetLength(0) / cellSize), Mathf.CeilToInt(tileMap.GetLength(1) / cellSize)];
        for (int i = 0; i < bg_grid.GetLength(0); i++) 
        {
            for (int j = 0; j < bg_grid.GetLength(1); j++) 
            {
                bg_grid[i, j] = -1;
            }
        }       

        Vector2Int initialPoint = new Vector2Int(rand.Next(0, tileMap.GetLength(0)), rand.Next(0, tileMap.GetLength(1)));
        activeSample.Enqueue(initialPoint);
        points.Add(initialPoint);
        tileMap[initialPoint.x, initialPoint.y] = tileType;
        bg_grid[Mathf.FloorToInt(initialPoint.x / cellSize), Mathf.FloorToInt(initialPoint.y / cellSize)] = 0;

        while (activeSample.Count > 0)
        {
            Vector2Int chosenSample = activeSample.Peek();
            bool foundCandidate = false;

            for (int i = 0; i < rejectionLimit; i++)
            {
                Vector2Int potentialSample = chosenSample;
                float randRadians = (float)rand.NextDouble() * Mathf.Deg2Rad;
                Vector2 randomDir = new Vector2(Mathf.Cos(radius), Mathf.Sin(radius)) * (float)rand.NextDouble();
                randomDir = randomDir.normalized * radius + randomDir * radius;

                potentialSample.x += Mathf.RoundToInt(randomDir.x);
                potentialSample.y += Mathf.RoundToInt(randomDir.y);

                if (potentialSample.x < 0 || potentialSample.y < 0 || potentialSample.x >= tileMap.GetLength(0) || potentialSample.y >= tileMap.GetLength(1))
                {
                    continue;
                }

                //Debug.Log("Checking " + potentialSample);

                if (poissonGridCheck(potentialSample, cellSize, radius, ref bg_grid, ref points))
                {
                    foundCandidate = true;
                    activeSample.Enqueue(potentialSample);
                    bg_grid[Mathf.FloorToInt(initialPoint.x / cellSize), Mathf.FloorToInt(initialPoint.y / cellSize)] = points.Count;
                    points.Add(potentialSample);
                    tileMap[potentialSample.x, potentialSample.y] = tileType;
                }
            }

            if (!foundCandidate)
            {
                activeSample.Dequeue();
            }
        }
        return points;
    }

    private bool poissonGridCheck(Vector2Int potentialSample, float cellSize, int radius, ref int[,] bg_grid, ref List<Vector2Int> points)
    {
        // int xCheckMin = Mathf.Max(0, Mathf.FloorToInt(potentialSample.x / cellSize) - 2);
        // int xCheckMax = Mathf.Min(bg_grid.GetLength(0), Mathf.FloorToInt(potentialSample.x / cellSize) + 3);
        // int yCheckMin = Mathf.Max(0, Mathf.FloorToInt(potentialSample.y / cellSize) - 2);
        // int yCheckMax = Mathf.Min(bg_grid.GetLength(1), Mathf.FloorToInt(potentialSample.y / cellSize) + 3);
        // for (int j = xCheckMin; j < xCheckMax; j++)
        // {
        //     for (int k = yCheckMin; k < yCheckMax; k++)
        //     {
        //         if (bg_grid[j, k] > -1)
        //         {
        //             Debug.Log(bg_grid[j, k]);
        //             int dist = Mathf.Abs(points[bg_grid[j, k]].x - potentialSample.x) + Mathf.Abs(points[bg_grid[j, k]].y - potentialSample.y);
        //             if (dist <= radius)
        //             {
        //                 return false;
        //             }
        //             return false;
        //         }
        //     }
        // }

        //unoptimized variant time
        foreach (Vector2Int point in points)
        {
            int dist = Mathf.Abs(point.x - potentialSample.x) + Mathf.Abs(point.y - potentialSample.y);
            if (dist <= radius)
            {
                return false;
            }
        }
        
        return true;
    }

    void OnDrawGizmos() 
    {
        if (intMap != null) 
        {
            for (int x = 0; x < tileArea.size.x; x++) 
            {
                for (int y = 0; y < tileArea.size.y; y++) 
                {
                    Gizmos.color = (intMap[x,y] == 1)? Color.black : Color.white;
                    Vector3 pos = new Vector3(x, y);
                    Gizmos.DrawCube(pos, Vector3.one);
                }
            }
        }
    }
}
