using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapAnalyzer : MonoBehaviour
{

    MapGenerator mapGenerator;

    public int width;
    public int height;

    int mapWidth;
    int mapHeight;

    float halfWidth;
    float halfHeight;

    //generated map
    public int[,] generatedMap;

    GameObject[] mapTiles;

    public GameObject enterPoint;
    public GameObject exitPoint;
    public GameObject exitDeeperPoint;

    public GameObject groundTile;
    public GameObject wallTile;
    public GameObject unbreakableWallTile;
    public GameObject waterTile;
    public GameObject lavaTile;

    // tile types:
    // 0 = ground
    // 1 = wall
    // 2 = unbreakable wall
    // 3 = water
    // 4 = lava    

    //map points
    public Vector4[,] points;
    //points[x,y].x = tile type
    //points[x,y].y = tile area (adjacent tiles of same type)
    //points[x,y].z = tile height
    //points[x,y].w = adjacent tiles and rotation ( (int)Math.Floor( x / 6 ) = how many adjacent whatevers, |  x%6 * 60 is rotation )

    private void Start()
    {
        //create tile type array
        mapTiles = new GameObject[50];
        mapTiles[0] = groundTile;
        mapTiles[1] = wallTile;
        mapTiles[2] = unbreakableWallTile;
        mapTiles[3] = waterTile;
        mapTiles[4] = lavaTile;

        halfWidth = (width / 2);
        halfHeight = (height / 2);

        //set map to build to 40 units smaller than total area, for borders
        width = width + 40;
        height = height + 40;

        mapWidth = width - 40;
        mapHeight = height - 40;

        mapGenerator = GetComponent<MapGenerator>();
        BuildMap(width, height);

    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            BuildMap(width, height);
        }
    }

    public void BuildMap(int width, int height)
    {

        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        points = new Vector4[width, height];

    mapGenerator.GenerateMap(mapWidth, mapHeight);


        // set borders and put map inside borders
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //make some points
                points[x, y] = new Vector4(0, 0, 0, 100);

                if (x < 15 || y < 15 || x >= mapWidth + 25 || y >= mapHeight + 25)
                    points[x, y].x = 2; //set to unbreakable tile type

                if (((x >= 15 && x < 20) || (x >= mapWidth + 20 && x < mapWidth + 25)) && (y >= 15 && y < mapHeight + 25))
                    points[x, y].x = 1; // set to breakable wall tile type

                if (((y >= 15 && y < 20) || (y >= mapHeight + 20 && y < mapHeight + 25)) && (x >= 15 && x < mapWidth + 25))
                    points[x, y].x = 1; // set to breakable wall tile type


                if (x >= 20 && x < mapWidth + 20 && y >= 20 && y < mapHeight + 20)
                    points[x, y].x = generatedMap[x - 20, y - 20]; // set to map


            }
        }


        //calculate area around map tiles
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                int a = 0; // up
                int b = 0; // up right
                int c = 0; // down right
                int d = 0; // down
                int e = 0; // down left
                int f = 0; // up left
                int g = 0; // right 2up
                int h = 0; // right 2 down
                int i = 0; // left 2 up
                int j = 0; // left 2 down

                int thisTile = (int)points[x, y].x; //this tile type

                for (int z = 0; z < 10 && y + z < height && points[x, y + z].x == thisTile; z++) a++;
                for (int z = 0; z < 10 && y - z > 0 && points[x, y - z].x == thisTile; z++) d++;

                for (int z = 0; z < 10 && x - z > 0 && points[x - z, y].x == thisTile; z++) e++;
                for (int z = 0; z < 10 && x + z < width && points[x + z, y].x == thisTile; z++) c++;

                for (int z = 0; z < 10 && x - z > 0 && y + z < height && points[x, y + z].x == thisTile; z++) f++;
                for (int z = 0; z < 10 && x + z < width && y + z < height && points[x, y + z].x == thisTile; z++) b++;

                for (int z = 0; z < 10 && x - z > 0 && y - z > 0 && points[x - z, y - z].x == thisTile; z++) j++;
                for (int z = 0; z < 10 && x + z < width && y - z > 0 && points[x + z, y - z].x == thisTile; z++) h++;

                for (int z = 0; z < 10 && x - z > 0 && y + z + 1 < height && points[x - z, y + z + 1].x == thisTile; z++) i++;
                for (int z = 0; z < 10 && x + z < width && y + z + 1 < height && points[x + z, y + z + 1].x == thisTile; z++) g++;

                int area = Mathf.Min(a, b, c, d, e, f, g, h, i, j); //get lowest number
                area++;
                points[x, y].y = area; //set area to that

            }
        }

        AdjacentTiles();

        SetHeightsToArea();

        PlaceTiles();

        EntryExit();
    }

    public void AdjacentTiles()
    {

        SmoothMap();

        for (int x = 0; x < width-1; x++)
        {
            for (int y = 0; y < height-1; y++)
            {

                if (points[x, y].x == 0)
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

                    if (points[(int)d0.x, (int)d0.y].x != tileType)
                    {
                        adj++;
                        x0 = true;
                    }
                    if (points[(int)d1.x, (int)d1.y].x != tileType)
                    {
                        adj++;
                        x1 = true;
                    }
                    if (points[(int)d2.x, (int)d2.y].x != tileType)
                    {
                        adj++;
                        x2 = true;
                    }
                    if (points[(int)d3.x, (int)d3.y].x != tileType)
                    {
                        adj++;
                        x3 = true;
                    }
                    if (points[(int)d4.x, (int)d4.y].x != tileType)
                    {
                        adj++;
                        x4 = true;
                    }
                    if (points[(int)d5.x, (int)d5.y].x != tileType)
                    {
                        adj++;
                        x5 = true;
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

                    Debug.Log((leftmost));
                    points[x, y].w = leftmost;
                }

                //hey 
                //what up me?
                //this is broken ay eff.
                //i think the dag nabbin.. uh.. offset to cube scripts might be wrong.. verify those are working correctly.
                //then go from there

            }
        }
    }

    public void SmoothMap()
    {
        for (int x = 0; x < width-1; x++)
        {
            for (int y = 0; y < height-1; y++)
            {

                if (points[x, y].x == 0)
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

                    int adj = 0;

                    if (points[(int)d0.x, (int)d0.y].x != tileType)
                        adj++;
                    if (points[(int)d1.x, (int)d1.y].x != tileType)
                        adj++;
                    if (points[(int)d2.x, (int)d2.y].x != tileType)
                        adj++;
                    if (points[(int)d3.x, (int)d3.y].x != tileType)
                        adj++;
                    if (points[(int)d4.x, (int)d4.y].x != tileType)
                        adj++;
                    if (points[(int)d5.x, (int)d5.y].x != tileType)
                        adj++;

                    if (points[x, y].x == 0 && adj > 3)
                        points[x, y].x = 1;

                }
            }
        }
    }

    public void SetHeightsToArea()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (points[x, y].x == 1 || points[x, y].x == 2)
                {
                    //       points[x, y].z = 2 + (points[x, y].y * .1f);
                }

            }
        }
    }

    public Vector3 Evenq_to_cube(int _x, int _y)
    {
        int x = (int)_x;
        int z = (int)_y - ((int)_x + ((int)_x & 1)) / 2;
        int y = -x - z;
        Vector3 cubePoint = new Vector3(x, y, z);
        return cubePoint;
    }

    public Vector3 Cube_to_evenq(int _x, int _y, int _z)
    {
        int x = _x;
        int y = _z + (_x + (_x & 1)) / 2;
        Vector2 offsetPoint = new Vector3(x, y);
        return offsetPoint;
    }

    public void PlaceTiles()
    {

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                GameObject tileType = mapTiles[(int)points[x, y].x];
                //int area = (int)points[x, y].y;
                float height = points[x, y].y * .08f;

                float pointRot;

                pointRot = 60 * (points[x, y].w);

                Quaternion rot = Quaternion.Euler(0, pointRot, 0);

                Vector3 pos = new Vector3(x - halfWidth + .5f, 0, (y - halfHeight + .5f) * 1.155f);
                if (x % 2 == 0) pos.z += .578f;

                GameObject _groundTile = Instantiate(groundTile, pos, rot);
                _groundTile.transform.localScale = new Vector3(1, height, 1);
                _groundTile.name = (tileType.name + " ("+x+","+y+")");
                _groundTile.transform.parent = this.transform;

                if (points[x, y].x != 0)
                {

                    //scale walls just cause
                    float height2 = 1 + (points[x, y].y * .35f);

                    Vector3 pos2 = new Vector3(x - halfWidth + .5f, height, (y - halfHeight + .5f) * 1.155f);
                    if (x % 2 == 0) pos2.z += .578f;

                    GameObject tile = Instantiate(tileType, pos2, rot);
                    tile.transform.localScale = new Vector3(1, height2, 1);
                    tile.name = tileType.name;
                    tile.transform.parent = this.transform;
                }

            }
        }

    }

    public void EntryExit()
    {

        bool foundEntry = false;
        //find entry point
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                if (points[x, y].x == 0 && points[x, y].y >= 3 && foundEntry == false) // if this is ground, and area is 3 or more
                {
                    GameObject tileType = enterPoint;
                    float height = points[x, y].z;
                    Quaternion rot = Quaternion.Euler(0, 0, 0);

                    Vector3 pos = new Vector3(x - halfWidth + .5f, height + 1, (y - halfHeight + .5f) * 1.155f);
                    if (x % 2 == 0) pos.z += .578f;

                    GameObject tile = Instantiate(tileType, pos, rot);
                    tile.transform.localScale = Vector3.one;
                    tile.name = tileType.name;
                    tile.transform.parent = this.transform;
                    foundEntry = true;

                    Vector3 cubePoint = Evenq_to_cube(x,y);
                    Debug.Log("the enter point is at cube point" + cubePoint);

                    Vector2 offsetPoint = Cube_to_evenq((int)cubePoint.x, (int)cubePoint.y, (int)cubePoint.z);
                    Debug.Log("the enter point is at offset point" + offsetPoint);
                }

            }
        }

        bool foundExit1 = false;
        bool foundExit2 = false;
        Vector2 exitFoundAt = new Vector2(0, 0);

        //find exot points
        for (int x = width -1; x > 0; x--)
        {
            for (int y = height -1; y > 0; y--)
            {

                if (points[x, y].x == 0 && points[x, y].y >= 3 && foundExit1 == false && foundExit2 == false) // if this is ground, and area is 3 or more
                {
                    GameObject tileType = exitPoint;
                    float height = points[x, y].z;
                    Quaternion rot = Quaternion.Euler(0, points[x, y].w, 0);

                    Vector3 pos = new Vector3(x - halfWidth + .5f, height + 1, (y - halfHeight + .5f) * 1.155f);
                    if (x % 2 == 0) pos.z += .578f;

                    GameObject tile = Instantiate(tileType, pos, rot);
                    tile.transform.localScale = Vector3.one;
                    tile.name = tileType.name;
                    tile.transform.parent = this.transform;

                    foundExit1 = true;
                    exitFoundAt = new Vector2(x, y);
                }

                if (points[x, y].x == 0 && points[x, y].y >= 3 && foundExit1 == true && foundExit2 == false) // if this is ground, and area is 3 or more
                {
                    if (x < exitFoundAt.x - 10 || x > exitFoundAt.x + 10 || y < exitFoundAt.y - 10 || y > exitFoundAt.y + 10)
                    {
                        GameObject tileType = exitDeeperPoint;
                        float height = points[x, y].z;
                        Quaternion rot = Quaternion.Euler(0, points[x, y].w, 0);

                        Vector3 pos = new Vector3(x - halfWidth + .5f, height + 1, (y - halfHeight + .5f) * 1.155f);
                        if (x % 2 == 0) pos.z += .578f;

                        GameObject tile = Instantiate(tileType, pos, rot);
                        tile.transform.localScale = Vector3.one;
                        tile.name = tileType.name;
                        tile.transform.parent = this.transform;

                        foundExit2 = true;
                        x = 0;
                        y = 0;
                    }
                }

            }
        }


    }



}