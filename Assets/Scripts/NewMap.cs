using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMap : MonoBehaviour
{

    MapGenerator mapGenerator;
    MapProcessor mapProcessor;
    GameManager gameManager;
    EnemyPopulator enemyPopulator;

    public int width;
    public int height;
    public int innerBorderSize;
    public int outerBorderSize;
    public float wallThreshold;
    public float roomThreshold;
    public int mapSmoothness;
    public int hallwaySize;
    [Range(0, 100)]
    public int randomFillPercent;

    public static Vector3 enterPoint;

    public Vector4[,] points;
    //points[x,y].x = tile type
    //points[x,y].y = tile area (adjacent tiles of same type)
    //points[x,y].z = adjacent tiles
    //points[x,y].w = tile rotation

    //generated map
    public int[,] generatedMap;

    int mapWidth;
    int mapHeight;
    float halfWidth;
    float halfHeight;


    private void Awake()
    {

        mapGenerator = GetComponent<MapGenerator>();
        mapProcessor = GetComponent<MapProcessor>();
        gameManager = GetComponent<GameManager>();
        enemyPopulator = GetComponent<EnemyPopulator>();

    }

    public void BuildMap(float seed, float levelDifficulty)
    {

        CavePopulator cavePopulator = GetComponent<CavePopulator>();
        cavePopulator.SetValues(seed);

        halfWidth = (width / 2);
        halfHeight = (height / 2);

        width = width + ((innerBorderSize + outerBorderSize) * 2);
        height = height + ((innerBorderSize + outerBorderSize) * 2);

        mapWidth = width - ((innerBorderSize + outerBorderSize) * 2);
        mapHeight = height - ((innerBorderSize + outerBorderSize) * 2);

        foreach (Transform child in transform.Find("Map"))
            GameObject.Destroy(child.gameObject);
        foreach (Transform child in transform.Find("Enemies"))
            GameObject.Destroy(child.gameObject);

        points = new Vector4[width, height];
        mapGenerator.GenerateMap(mapWidth, mapHeight, wallThreshold, roomThreshold, mapSmoothness, hallwaySize, randomFillPercent, seed);
        mapProcessor.ProcessMap(width, height, outerBorderSize, innerBorderSize, mapWidth, mapHeight, points, generatedMap, seed);


        //if (seed % 6 > 3)
        //mapProcessor.CreateLake();

        mapProcessor.SmoothMap();
        mapProcessor.CalcAdjacentTiles();

        cavePopulator.PopulateMap(width, height, halfWidth, halfHeight, points);

        enemyPopulator.SpawnEnemies(width, height, halfWidth, halfHeight, points, seed, levelDifficulty);

        gameManager.MovePlayer(enterPoint);


    }



}