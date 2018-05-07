using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{

    GameObject player;

    GameManager gameManager;
    UIController uiController;
    PlayerController playerController;
    MapGenerator mapGenerator;
    EnemySpawns enemySpawns;
    CavePopulator cavePopulator;

    [HideInInspector]
    public Vector4[,] points;
    //points[x,y].x = tile type
    //points[x,y].y = tile area (adjacent tiles of same type)
    //points[x,y].z = adjacent tiles
    //points[x,y].w = tile rotation

    [HideInInspector]
    public Vector2 enterPointXY;
    [HideInInspector]
    public Vector2 exitPointXY;

    public int numberInSet = 0;

    [HideInInspector]
    public Vector3[] enterPoints;
    [HideInInspector]
    public Vector3[] exitPoints;


    // Use this for initialization
    void Awake()
    {
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        uiController = GameObject.Find("EventSystem").GetComponent<UIController>();
        mapGenerator = GetComponent<MapGenerator>();
        enemySpawns = GetComponent<EnemySpawns>();
        cavePopulator = GetComponent<CavePopulator>();

        enterPoints = new Vector3[10];
        exitPoints = new Vector3[10];
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown("right"))
        {
            NextArea();
        }
        if (Input.GetKeyDown("left"))
        {
            PreviousArea();
        }

    }

   public void NextArea()
    {
        GameObject[] PrevDoors = new GameObject[20];
        PrevDoors = GameObject.FindGameObjectsWithTag("PreviousArea");
        foreach (GameObject door in PrevDoors)
        {
            door.GetComponent<EnterExit>().Deactivate();
        }
        StartCoroutine(uiController.CrossFade());
        StartCoroutine(_NextArea());
    }


    IEnumerator _NextArea()
    {
        yield return new WaitForSeconds(.5f);

        if (gameManager.path[gameManager.pathProgress + 1].w == 0)
        {
            BuildMap(gameManager.path[gameManager.pathProgress + 1].x, gameManager.path[gameManager.pathProgress + 1].y, gameManager.path[gameManager.pathProgress + 1].z);
        }
        else
        {
            MovePlayer(false);
        }
    }

    public void PreviousArea()
    {
        GameObject[] nextDoors = new GameObject[20];
        nextDoors = GameObject.FindGameObjectsWithTag("NextArea");
        foreach (GameObject door in nextDoors)
        {
            door.GetComponent<EnterExit>().Deactivate();
        }
        StartCoroutine(uiController.CrossFade());
        StartCoroutine(_PreviousArea());
    }


    IEnumerator _PreviousArea()
    {
        yield return new WaitForSeconds(.5f);

        if (gameManager.path[gameManager.pathProgress].x > 0)
        {
            if (numberInSet > 1)
            {
                MovePlayer(true);
            }
        }
    }

    public void BuildMap(float seed, float level, float biome)
    {

        //if this is the first in a new set, generate a town and destroy last set of maps.
        if (gameManager.pathProgress > 0)
        {
            if (gameManager.path[gameManager.pathProgress].y != gameManager.path[gameManager.pathProgress - 1].y)
            {
                DestroyOldMap();
                numberInSet = 0;
            }
        }


        //set these based on biome
        int width = 65;
        int height = 20;
        int innerBorderSize = 2;
        int outerBorderSize = 2;
        float wallThreshold = 1f;
        float roomThreshold = 1f;
        int mapSmoothness = 1;
        int hallwaySize = 3;
        int randomFillPercent = 43;


        float halfWidth = (width / 2);
        float halfHeight = (height / 2); ;

        width = width + ((innerBorderSize + outerBorderSize) * 2);
        height = height + ((innerBorderSize + outerBorderSize) * 2);

        int mapWidth = width - ((innerBorderSize + outerBorderSize) * 2);
        int mapHeight = height - ((innerBorderSize + outerBorderSize) * 2);

        points = new Vector4[width, height];

        int[,] map = mapGenerator.GenerateMap(mapWidth, mapHeight, wallThreshold, roomThreshold, mapSmoothness, hallwaySize, randomFillPercent, seed);
        ProcessMap(width, height, outerBorderSize, innerBorderSize, mapWidth, mapHeight, map, seed);

        // determine biome, run script 
        if (biome != 2342342423)
        {
            cavePopulator.PopulateMap(width, height, halfWidth, halfHeight, points, numberInSet);
            enemySpawns.SpawnEnemies(width, height, halfWidth, halfHeight, points, seed, level, enterPointXY, exitPointXY, numberInSet);
        }

        //set this map as previsouly generated
        gameManager.path[(int)seed].w = 1;

        MovePlayer(false);

    }

    public void MovePlayer(bool backwards)
    {

        if (backwards == false)
        {
            player.transform.position = enterPoints[numberInSet];
            playerController.MapCamera.transform.position = enterPoints[numberInSet];
            playerController.myCamera.transform.position = enterPoints[numberInSet];
            StartCoroutine(WalkRight());
            numberInSet++;
            gameManager.pathProgress++;
        }
        else
        {    
            numberInSet--;
            gameManager.pathProgress--;
            player.transform.position = exitPoints[numberInSet-1];
            playerController.MapCamera.transform.position = exitPoints[numberInSet-1];
            playerController.myCamera.transform.position = exitPoints[numberInSet-1];
            StartCoroutine(WalkLeft());
        }
    }

    IEnumerator WalkRight()
    {
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        player.transform.rotation = Quaternion.Euler(Vector3.right);

        float walkingFor = 0;
        while (walkingFor < 1.5f)
        {
            playerRb.AddForce(Vector3.right * 15, ForceMode.Acceleration);
            walkingFor += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator WalkLeft()
    {
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        player.transform.rotation = Quaternion.Euler(Vector3.left);

        float walkingFor = 0;
        while (walkingFor < 1.5f)
        {
            playerRb.AddForce(Vector3.left * 15, ForceMode.Acceleration);
            walkingFor += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    void DestroyOldMap()
    {
        foreach (Transform child in this.transform)
            GameObject.Destroy(child.gameObject);
    }

    void ProcessMap(int width, int height, int outerBorderSize, int innerBorderSize, int mapWidth, int mapHeight, int[,] map, float seed)
    {
        // set borders and put map inside borders
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //make some points
                points[x, y] = new Vector4(0, 0, 0, 7);

                if (x < outerBorderSize || y < outerBorderSize || x >= mapWidth + (outerBorderSize + (innerBorderSize * 2)) || y >= mapHeight + (outerBorderSize + (innerBorderSize * 2)))
                    points[x, y].x = 4; //set to unbreakable tile type

                if (((x >= outerBorderSize && x < (innerBorderSize + outerBorderSize) || (x >= mapWidth + (innerBorderSize + outerBorderSize) && x < mapWidth + (outerBorderSize + (innerBorderSize * 2)))) && (y >= outerBorderSize && y < mapHeight + (outerBorderSize + (innerBorderSize * 2)))))
                    points[x, y].x = 1; // set to breakable wall tile type

                if (((y >= outerBorderSize && y < (innerBorderSize + outerBorderSize)) || (y >= mapHeight + (innerBorderSize + outerBorderSize) && y < mapHeight + (outerBorderSize + (innerBorderSize * 2)))) && (x >= outerBorderSize && x < mapWidth + (outerBorderSize + (innerBorderSize * 2))))
                    points[x, y].x = 1; // set to breakable wall tile type


                if (x >= (innerBorderSize + outerBorderSize) && x < mapWidth + (innerBorderSize + outerBorderSize) && y >= (innerBorderSize + outerBorderSize) && y < mapHeight + (innerBorderSize + outerBorderSize))
                    points[x, y].x = map[x - (innerBorderSize + outerBorderSize), y - (innerBorderSize + outerBorderSize)]; // set to map

            }
        }

        CalculateAreas(width, height);
        //CreateLake(width, height);
        SmoothMap(width, height);
        CalcAdjacentTiles(width, height);
    }

    public void CalculateAreas(int width, int height)
    {
        //calculate area around map tiles
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool broke = false;

                int thisTile = (int)points[x, y].x; //this tile type
                int area = 0;

                Vector2 calcPoint;
                Vector3 cubePoint = Evenq_to_cube(x, y);

                if (thisTile != 2)
                {

                    for (int q = 0; x + q < width && x - q > 0 && y + q < height && y - q > 0 && broke == false; q++)
                    {
                        for (int r = -q; r <= q && broke == false; r++)
                        {
                            for (int s = -q; s <= q; s++)
                            {
                                for (int t = -q; t <= q; t++)
                                {
                                    calcPoint = Cube_to_evenq((int)cubePoint.x + r, (int)cubePoint.y + s, (int)cubePoint.z + t);
                                    if (calcPoint.x > 0 && calcPoint.x < width && calcPoint.y > 0 && calcPoint.y < height)
                                    {
                                        if (points[(int)calcPoint.x, (int)calcPoint.y].x == thisTile)
                                        {
                                            area = q;
                                        }
                                        else
                                        {
                                            area = q - 1;
                                            broke = true;
                                        }
                                    }
                                }
                            }
                        }
                    }

                }

                area++;
                points[x, y].y = area;
            }
        }
    }

    public void CreateLake(int width, int height)
    {
        bool broke = false;
        //calculate area around map tiles
        for (int x = 0; x < width && broke == false; x++)
        {
            for (int y = 0; y < height && broke == false; y++)
            {


                Vector2 calcPoint;
                Vector3 cubePoint = Evenq_to_cube(x, y);

                for (int z = 20; z > 0 && broke == false; z--)
                {
                    if (points[x, y].x == 0 && points[x, y].y > 5)
                    {
                        int q = 6;
                        for (int r = -q; r <= q; r++)
                        {
                            for (int s = -q; s <= q; s++)
                            {
                                for (int t = -q; t <= q; t++)
                                {
                                    if (r + s + t == 0)
                                    {
                                        calcPoint = Cube_to_evenq((int)cubePoint.x + r, (int)cubePoint.y + s, (int)cubePoint.z + t);
                                        if (calcPoint.x > 0 && calcPoint.x < width && calcPoint.y > 0 && calcPoint.y < height && points[(int)calcPoint.x, (int)calcPoint.y].y > 2)
                                        {
                                            points[(int)calcPoint.x, (int)calcPoint.y].x = 2;
                                            //points[(int)calcPoint.x, (int)calcPoint.y].y = q - Mathf.Max(r,s,t,-r,-s,-t);
                                        }

                                        broke = true;

                                    }

                                }
                            }
                        }
                    }
                }

                if (points[x, y].x == 2 && points[x, y].z > 3)
                {
                    points[x, y].x = 0;
                    Debug.Log("lol");
                }

            }
        }
    }

    public void SmoothMap(int width, int height)
    {
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                int tileType = (int)points[x, y].x;
                Vector3 cubePoint = Evenq_to_cube(x, y);
                int cubeX = (int)cubePoint.x;
                int cubeY = (int)cubePoint.y;
                int cubeZ = (int)cubePoint.z;

                Vector2 d0 = Cube_to_evenq(cubeX, cubeY + 1, cubeZ - 1);
                Vector2 d1 = Cube_to_evenq(cubeX + 1, cubeY, cubeZ - 1);
                Vector2 d2 = Cube_to_evenq(cubeX + 1, cubeY - 1, cubeZ);
                Vector2 d3 = Cube_to_evenq(cubeX, cubeY - 1, cubeZ + 1);
                Vector2 d4 = Cube_to_evenq(cubeX - 1, cubeY, cubeZ + 1);
                Vector2 d5 = Cube_to_evenq(cubeX - 1, cubeY + 1, cubeZ);

                bool x0 = false;
                bool x1 = false;
                bool x2 = false;
                bool x3 = false;
                bool x4 = false;
                bool x5 = false;

                int adj = 0;
                int _newTileType = 100;

                if (points[(int)d0.x, (int)d0.y].x != tileType)
                {
                    adj++; x0 = true; _newTileType = (int)points[(int)d0.x, (int)d0.y].x;
                }
                if (points[(int)d1.x, (int)d1.y].x != tileType)
                {
                    adj++; x1 = true; _newTileType = (int)points[(int)d1.x, (int)d1.y].x;
                }
                if (points[(int)d2.x, (int)d2.y].x != tileType)
                {
                    adj++; x2 = true; _newTileType = (int)points[(int)d2.x, (int)d2.y].x;
                }
                if (points[(int)d3.x, (int)d3.y].x != tileType)
                {
                    adj++; x3 = true; _newTileType = (int)points[(int)d3.x, (int)d3.y].x;
                }
                if (points[(int)d4.x, (int)d4.y].x != tileType)
                {
                    adj++; x4 = true; _newTileType = (int)points[(int)d4.x, (int)d4.y].x;
                }
                if (points[(int)d5.x, (int)d5.y].x != tileType)
                {
                    adj++; x5 = true; _newTileType = (int)points[(int)d5.x, (int)d5.y].x;
                }

                if (adj > 3)
                {
                    points[x, y].x = _newTileType;
                }


                //fix small hallways
                if (x0 == true && (x1 == false && x5 == false) && (x2 == true || x3 == true || x4 == true))
                    points[(int)d0.x, (int)d0.y].x = 0;

                if (x1 == true && (x2 == false && x0 == false) && (x3 == true || x4 == true || x5 == true))
                    points[(int)d1.x, (int)d1.y].x = 0;

                if (x2 == true && (x3 == false && x1 == false) && (x4 == true || x5 == true || x0 == true))
                    points[(int)d2.x, (int)d2.y].x = 0;

                if (x3 == true && (x4 == false && x2 == false) && (x5 == true || x0 == true || x1 == true))
                    points[(int)d3.x, (int)d3.y].x = 0;

                if (x4 == true && (x5 == false && x3 == false) && (x0 == true || x1 == true || x2 == true))
                    points[(int)d4.x, (int)d4.y].x = 0;

                if (x5 == true && (x0 == false && x4 == false) && (x1 == true || x2 == true || x3 == true))
                    points[(int)d5.x, (int)d5.y].x = 0;

            }
        }
    }

    public void CalcAdjacentTiles(int width, int height)
    {

        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {

                if (points[x, y].x < 40)
                {

                    int tileType = (int)points[x, y].x;
                    Vector3 cubePoint = Evenq_to_cube(x, y);
                    int cubeX = (int)cubePoint.x;
                    int cubeY = (int)cubePoint.y;
                    int cubeZ = (int)cubePoint.z;

                    Vector2 d0 = Cube_to_evenq(cubeX, cubeY + 1, cubeZ - 1);
                    Vector2 d1 = Cube_to_evenq(cubeX + 1, cubeY, cubeZ - 1);
                    Vector2 d2 = Cube_to_evenq(cubeX + 1, cubeY - 1, cubeZ);
                    Vector2 d3 = Cube_to_evenq(cubeX, cubeY - 1, cubeZ + 1);
                    Vector2 d4 = Cube_to_evenq(cubeX - 1, cubeY, cubeZ + 1);
                    Vector2 d5 = Cube_to_evenq(cubeX - 1, cubeY + 1, cubeZ);


                    bool x0 = false;
                    bool x1 = false;
                    bool x2 = false;
                    bool x3 = false;
                    bool x4 = false;
                    bool x5 = false;

                    int adj = 0;
                    int leftmost = 0;

                    if (points[(int)d0.x, (int)d0.y].x != tileType && (points[(int)d0.x, (int)d0.y].x != 2 && points[(int)d0.x, (int)d0.y].x != 3))
                    {
                        adj++; x0 = true;
                    }
                    if (points[(int)d1.x, (int)d1.y].x != tileType && (points[(int)d1.x, (int)d1.y].x != 2 && points[(int)d1.x, (int)d1.y].x != 3))
                    {
                        adj++; x1 = true;
                    }
                    if (points[(int)d2.x, (int)d2.y].x != tileType && (points[(int)d2.x, (int)d2.y].x != 2 && points[(int)d2.x, (int)d2.y].x != 3))
                    {
                        adj++; x2 = true;
                    }
                    if (points[(int)d3.x, (int)d3.y].x != tileType && (points[(int)d3.x, (int)d3.y].x != 2 && points[(int)d3.x, (int)d3.y].x != 3))
                    {
                        adj++; x3 = true;
                    }
                    if (points[(int)d4.x, (int)d4.y].x != tileType && (points[(int)d4.x, (int)d4.y].x != 2 && points[(int)d4.x, (int)d4.y].x != 3))
                    {
                        adj++; x4 = true;
                    }
                    if (points[(int)d5.x, (int)d5.y].x != tileType && (points[(int)d5.x, (int)d5.y].x != 2 && points[(int)d5.x, (int)d5.y].x != 3))
                    {
                        adj++; x5 = true;
                    }

                    if (x0 == true && x5 != true)
                    {
                        leftmost = 0;
                    }
                    if (x1 == true && x0 != true)
                    {
                        leftmost = 1;
                    }
                    if (x2 == true && x1 != true)
                    {
                        leftmost = 2;
                    }
                    if (x3 == true && x2 != true)
                    {
                        leftmost = 3;
                    }
                    if (x4 == true && x3 != true)
                    {
                        leftmost = 4;
                    }
                    if (x5 == true && x4 != true)
                    {
                        leftmost = 5;
                    }

                    if (adj > 0)
                        points[x, y].w = leftmost;
                    points[x, y].z = adj;
                }

            }
        }
    }

    public Vector3 Cube_to_evenq(int _x, int _y, int _z)
    {
        int col = _x;
        int row = _z + (_x + (_x % 2)) / 2;
        Vector2 offsetPoint = new Vector3(col, row);
        return offsetPoint;
    }

    public Vector3 Evenq_to_cube(int _x, int _y)
    {
        int x = _x;
        int z = _y - (_x + (_x % 2)) / 2;
        int y = -x - z;
        Vector3 cubePoint = new Vector3(x, y, z);
        return cubePoint;
    }

}
