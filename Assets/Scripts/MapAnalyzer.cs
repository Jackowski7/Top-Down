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
    //points[x,y].w = tile rotation (in 60^ snaps)

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
                points[x, y] = new Vector4(0, 0, 0, 60 * Random.Range(0, 5));

                if (x < 15 || y < 15 || x >= mapWidth + 25 || y >= mapHeight + 25)
                    points[x, y].x = 2; //set to unbreakable tile type

                if (( (x >= 15 && x < 20) || (x >= mapWidth + 20 && x < mapWidth + 25)) && (y >= 15 && y < mapHeight + 25))
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

        SetHeightsToArea();

        PlaceTiles();

        EntryExit();
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

    public void PlaceTiles()
    {

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                GameObject tileType = mapTiles[(int)points[x, y].x];
                //int area = (int)points[x, y].y;
                float height = points[x, y].y * .08f;
                Quaternion rot = Quaternion.Euler(0, points[x, y].w, 0);

                Vector3 pos = new Vector3(x - halfWidth + .5f, 0, (y - halfHeight + .5f) * 1.155f);
                if (x % 2 == 0) pos.z += .578f;

                GameObject _groundTile = Instantiate(groundTile, pos, rot);
                _groundTile.transform.localScale = new Vector3(1, height, 1);
                _groundTile.name = tileType.name;
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

                    Vector3 pos = new Vector3(x - halfWidth + .5f, 0, (y - halfHeight + .5f) * 1.155f);
                    if (x % 2 == 0) pos.z += .578f;

                    GameObject tile = Instantiate(tileType, pos, rot);
                    tile.transform.localScale = Vector3.one;
                    tile.name = tileType.name;
                    tile.transform.parent = this.transform;
                    foundEntry = true;
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

                    Vector3 pos = new Vector3(x - halfWidth + .5f, height, (y - halfHeight + .5f) * 1.155f);
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

                        Vector3 pos = new Vector3(x - halfWidth + .5f, height, (y - halfHeight + .5f) * 1.155f);
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