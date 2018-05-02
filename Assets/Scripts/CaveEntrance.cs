using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveEntrance : MonoBehaviour {

    MapGenerator mapGenerator;
    MapProcessor mapProcessor;
    GameManager gameManager;
    EnemyPopulator enemyPopulator;
    CavePopulator cavePopulator;
    GameObject map;
    GameObject enemies;

    public bool randomSeed;
    public float seed;
    public float difficulty;

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

    [HideInInspector]
    int mapWidth;
    [HideInInspector]
    int mapHeight;
    [HideInInspector]
    float halfWidth;
    [HideInInspector]
    float halfHeight;

    [HideInInspector]
    public Vector3 enterPoint;

    [HideInInspector]
    public Vector4[,] points;
    //points[x,y].x = tile type
    //points[x,y].y = tile area (adjacent tiles of same type)
    //points[x,y].z = adjacent tiles
    //points[x,y].w = tile rotation

    //generated map
    [HideInInspector]
    public int[,] generatedMap;

    // Use this for initialization
    void Awake () {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        mapGenerator = GetComponent<MapGenerator>();
        mapProcessor = GetComponent<MapProcessor>();
        cavePopulator = GetComponent<CavePopulator>();
        enemyPopulator = GetComponent<EnemyPopulator>();
        map = GameObject.Find("Map").gameObject;
        enemies = GameObject.Find("Enemies").gameObject;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {

            if (randomSeed == true)
            {
                seed = Time.unscaledTime;
            }

            gameManager.currentPath = new Vector3[Random.Range(1, 4)];
            //x = seed
            //y = difficulty
            //z = type

            for (int x = 0; x < gameManager.currentPath.Length; x++)
            {
                gameManager.currentPath[x].x = seed + x;
                gameManager.currentPath[x].y = gameManager.level;
                gameManager.currentPath[x].z = 1;
                gameManager.pathProgress = 0;
            }

            BuildCave();
        }
    }

    void BuildCave()
    {

        DestroyOldMap();

        halfWidth = (width / 2);
        halfHeight = (height / 2);

        width = width + ((innerBorderSize + outerBorderSize) * 2);
        height = height + ((innerBorderSize + outerBorderSize) * 2);

        mapWidth = width - ((innerBorderSize + outerBorderSize) * 2);
        mapHeight = height - ((innerBorderSize + outerBorderSize) * 2);

        points = new Vector4[width, height];
        generatedMap = mapGenerator.GenerateMap(mapWidth, mapHeight, wallThreshold, roomThreshold, mapSmoothness, hallwaySize, randomFillPercent, seed);
        mapProcessor.ProcessMap(width, height, outerBorderSize, innerBorderSize, mapWidth, mapHeight, points, generatedMap, seed);


        //if (seed % 6 > 3)
        //mapProcessor.CreateLake();

        mapProcessor.SmoothMap();
        mapProcessor.CalcAdjacentTiles();

        cavePopulator.PopulateMap(width, height, halfWidth, halfHeight, points);

        enemyPopulator.SpawnEnemies(width, height, halfWidth, halfHeight, points, seed, difficulty, enterPoint);

        gameManager.MovePlayer();
    }

    void DestroyOldMap()
    {
        foreach (Transform child in map.transform)
            GameObject.Destroy(child.gameObject);

        foreach (Transform child in enemies.transform)
            GameObject.Destroy(child.gameObject);
    }
}
