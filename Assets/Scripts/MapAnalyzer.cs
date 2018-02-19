using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapAnalyzer : MonoBehaviour
{

    public GameObject _mapGenerator;
    MapGenerator mapGenerator;

    public GameObject enter;
    public GameObject exit;
    public GameObject exitDeeper;

    public static bool mapReady = false;
    public static bool populated = false;

    public static int[,] openPoints;
    public static int[,] closedPoints;
    public static int[,] border;

    Vector2 exitSpacing;
    bool firstExitfound;

    private int iteration;

    private void Start()
    {
        mapGenerator = _mapGenerator.GetComponent<MapGenerator>();
    }

    public void AnalyzeMap(int mapWidth, int mapHeight, int[,]map, int mapBorder, int PreBorder)
    {

        //first, destroy old map
        if (iteration > 0)
        {
            GameObject exit = transform.Find("Exit").gameObject;
            GameObject exitDeeper = transform.Find("Exit_Deeper").gameObject;
            GameObject enter = transform.Find("Enter").gameObject;
            Destroy(exit);
            Destroy(enter);
            Destroy(exitDeeper);
        }

        firstExitfound = false;
        openPoints = new int[mapWidth, mapHeight];
        closedPoints = new int[mapWidth, mapHeight];
        border = new int[mapWidth + mapBorder * 2, mapHeight + mapBorder * 2];

        float halfWidth = (mapWidth / 2);
        float halfHeight = (mapHeight / 2);

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {

                //open points (0)
                if (map[x, y] == 0)
                {

                    int W = 0;
                    int E = 0;
                    int N = 0;
                    int S = 0;
                    int NE = 0;
                    int NW = 0;
                    int SE = 0;
                    int SW = 0;

                    //check surroundings
                    for (int z = 0; z < 10 && x - z > 0 && map[x - z, y] == 0; z++)
                    W++;

                    for (int z = 0; z < 10 && x + z < mapWidth && map[x + z, y] == 0; z++)
                    E++;

                    for (int z = 0; z < 10 && y + z < mapHeight && map[x, y + z] == 0; z++)
                    N++;

                    for (int z = 0; z < 10 && y - z > 0 && map[x, y - z] == 0; z++)
                    S++;

                    for (int z = 0; z < 10 && y + z < mapHeight && x + z < mapWidth && map[x + z, y + z] == 0; z++)
                    NE++;

                    for (int z = 0; z < 10 && y + z < mapHeight && x - z > 0 && map[x - z, y + z] == 0; z++)
                    NW++;

                    for (int z = 0; z < 10 && y - z > 0 && x + z < mapWidth &&  map[x + z, y - z] == 0; z++)
                    SE++;

                    for (int z = 0; z < 10 && y - z > 0 && x - z > 0 && map[x - z, y - z] == 0; z++)
                    SW++;

                    int area = Mathf.Min(N, S, E, W, NE, NW, SE, SW);
                    area++;    
                    
                    openPoints[x, y] = area;                  

                }
                //open points (1)
                if (map[x, y] == 1)
                {
                    int W = 0;
                    int E = 0;
                    int N = 0;
                    int S = 0;
                    int NE = 0;
                    int NW = 0;
                    int SE = 0;
                    int SW = 0;

                    //check surroundings
                    for (int z = 0; z < 10 && x - z > 0 && map[x - z, y] == 1; z++)
                        W++;

                    for (int z = 0; z < 10 && x + z < mapWidth && map[x + z, y] == 1; z++)
                        E++;

                    for (int z = 0; z < 10 && y + z < mapHeight && map[x, y + z] == 1; z++)
                        N++;

                    for (int z = 0; z < 10 && y - z > 0 && map[x, y - z] == 1; z++)
                        S++;

                    for (int z = 0; z < 10 && y + z < mapHeight && x + z < mapWidth && map[x + z, y + z] == 1; z++)
                        NE++;

                    for (int z = 0; z < 10 && y + z < mapHeight && x - z > 0 && map[x - z, y + z] == 1; z++)
                        NW++;

                    for (int z = 0; z < 10 && y - z > 0 && x + z < mapWidth && map[x + z, y - z] == 1; z++)
                        SE++;

                    for (int z = 0; z < 10 && y - z > 0 && x - z > 0 && map[x - z, y - z] == 1; z++)
                        SW++;

                    int area = Mathf.Min(N, S, E, W, NE, NW, SE, SW);
                    area++;

                    closedPoints[x, y] = area;

                }


            }
        }     
        
        
        
        //make borders solid and unbreakable (1)
        for (int x = 0; x < mapWidth + mapBorder + mapBorder; x++)
        {
            for (int y = 0; y < mapHeight + mapBorder + mapBorder; y++)
            {

                //for inside breakable border
                if (x <= mapBorder || y <= mapBorder || x > mapWidth + mapBorder || y > mapHeight + mapBorder)
                {
                    border[x, y] = -1;
                }

                //for outside unbreakables
                if (x <= mapBorder - PreBorder || y <= mapBorder - PreBorder || x >= mapWidth + mapBorder + PreBorder || y >= mapHeight + mapBorder + PreBorder)
                {
                    border[x, y] = 1;
                }

            }

        }

        //find entry point
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (openPoints[x, y] > 3)
                {

                    Vector3 pos = new Vector3(x - halfWidth + .5f, 0, (y - halfHeight +1f) * 1.155f);
                    if (x % 2 == 0)
                    {
                        pos.z += .578f;
                    }

                    GameObject go;
                    go = Instantiate(enter, pos, transform.rotation);
                    go.name = "Enter";
                    go.transform.parent = transform;
                    x = 1000;
                    y = 1000;
                }
            }

        }

        //find exit point
        for (int x = (int)mapWidth-1; x > 0; x--)
        {
            for (int y = (int)mapHeight-1; y > 0; y--)
            {
                if (firstExitfound == false && openPoints[x, y] > 5)
                {

                    Vector3 pos = new Vector3(x - halfWidth + .5f, 0, (y - halfHeight + 1) * 1.155f);
                    if (x % 2 == 1)
                    {
                        pos.z += .578f;
                    }

                    GameObject go;
                    go = Instantiate(exitDeeper, pos, transform.rotation);
                    go.name = "Exit_Deeper";
                    go.transform.parent = transform;
                    firstExitfound = true;
                    exitSpacing = new Vector2(x,y);
                }
                if (firstExitfound == true && openPoints[x, y] > 5 && (y > exitSpacing.y + 10 || y < exitSpacing.y - 10 || x < exitSpacing.x - 10 || x > exitSpacing.x + 10))
                {

                    Vector3 pos = new Vector3(x - halfWidth + .5f, 0, (y - halfHeight + 1) * 1.155f);
        if (x % 2 == 1)
                    {
                        pos.z += .578f;
                    }

                    GameObject go;
                    go = Instantiate(exit, pos, transform.rotation);
                    go.name = "Exit";
                    go.transform.parent = transform;
                    x = 0;
                    y = 0;
                }
            }
        }

        WallBuilder.BuildWalls(mapWidth, mapHeight, closedPoints, border, mapBorder);
        FloorBuilder.BuildFloor(mapWidth, mapHeight, openPoints, border, mapBorder);


        iteration++;

    }

}
