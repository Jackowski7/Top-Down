using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CavePopulator : MonoBehaviour
{

    GameObject[] mapTiles;
    Transform mapFolder;
    CaveEntrance caveEntrance;

    public GameObject enterPoint;
    public GameObject exitPoint;
    public GameObject exitDeeperPoint;

    public GameObject groundTile;
    public GameObject wallTile;
    public GameObject unbreakableWallTile;
    public GameObject waterTile;
    public GameObject lavaTile;


    public GameObject ground0;
    public GameObject ground1;
    public GameObject ground2;
    public GameObject ground3;
    public GameObject ground4;
    public GameObject ground5;
    public GameObject ground6;
    public GameObject ground7;
    public GameObject ground8;
    public GameObject ground9;

    public GameObject wall0;
    public GameObject wall1;
    public GameObject wall2;
    public GameObject wall3;
    public GameObject wall4;
    public GameObject wall5;
    public GameObject wall6;
    public GameObject wall7;
    public GameObject wall8;
    public GameObject wall9;

    public GameObject water0;
    public GameObject water1;
    public GameObject water2;
    public GameObject water3;
    public GameObject water4;
    public GameObject water5;
    public GameObject water6;
    public GameObject water7;
    public GameObject water8;
    public GameObject water9;


    private void Start()
    {
        mapFolder = GameObject.Find("Map").transform;
        caveEntrance = GetComponent<CaveEntrance>();
    }
    public void PopulateMap(int width, int height, float halfWidth, float halfHeight, Vector4[,] points)
    {


        mapTiles = new GameObject[100];

        mapTiles[0] = ground0;
        mapTiles[1] = ground1;
        mapTiles[2] = ground2;
        mapTiles[3] = ground3;
        mapTiles[4] = ground4;
        mapTiles[5] = ground5;
        mapTiles[6] = ground6;
        mapTiles[7] = ground7;
        mapTiles[8] = ground8;
        mapTiles[9] = ground9;

        mapTiles[10] = wall0;
        mapTiles[11] = wall1;
        mapTiles[12] = wall2;
        mapTiles[13] = wall3;
        mapTiles[14] = wall4;
        mapTiles[15] = wall5;
        mapTiles[16] = wall6;
        mapTiles[17] = wall7;
        mapTiles[18] = wall8;
        mapTiles[19] = wall9;

        mapTiles[20] = water0;
        mapTiles[21] = water1;
        mapTiles[22] = water2;
        mapTiles[23] = water3;
        mapTiles[24] = water4;
        mapTiles[25] = water5;
        mapTiles[26] = water6;
        mapTiles[27] = water7;
        mapTiles[28] = water8;
        mapTiles[29] = water9;


        for (int x = 30; x < 40; x++)
        {
            mapTiles[x] = lavaTile;
        }
        for (int x = 40; x < 50; x++)
        {
            mapTiles[x] = unbreakableWallTile;
        }

        mapTiles[51] = enterPoint;
        mapTiles[52] = exitPoint;
        mapTiles[53] = exitDeeperPoint;


        EntryExit(width, height, halfWidth, halfHeight, points);

        PlaceTiles(width, height, halfWidth, halfHeight, points);

    }

    public void PlaceTiles(int width, int height, float halfWidth, float halfHeight, Vector4[,] points)
    {

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                Vector3 pos = new Vector3(x - halfWidth + .5f, 0, -(y - halfHeight + .5f) * 1.155f);
                if (x % 2 == 0) pos.z -= .578f;

                float area = (int)points[x, y].y * .08f;
                int _rot = (int)points[x, y].w;
                int _tileType = (int)points[x, y].x;

                if (_rot > 6 || _tileType == 1)
                    _rot = Random.Range(0, 6);

                Quaternion rot = Quaternion.Euler(0, _rot * 60, 0);
                

                int _numberAdjacent = (int)points[x, y].z;

                GameObject tileType;

                if (_tileType == 0 || _tileType == 4)
                {
                    _tileType = Mathf.Min((_tileType * 10) + (_numberAdjacent * 3) + Random.Range(0, 3), (_tileType * 10) + 9);
                }
                else if (_tileType == 1)
                {
                    _tileType = Mathf.Min((_tileType * 10) + Random.Range(0, 10), (_tileType * 10) + 9);
                }
                else if (_tileType == 2 || _tileType == 3)
                {
                    if (_numberAdjacent < 1)
                    {
                        _tileType = (_tileType * 10);
                    }
                    else if (_numberAdjacent < 4)
                    {
                        _tileType = Mathf.Min((_tileType * 10) + ((_numberAdjacent - 1) * 3) + Random.Range(1, 4), (_tileType * 10) + 8);
                    }
                    else
                    {
                        _tileType = (_tileType * 10) + 9;
                    }
                }

                tileType = mapTiles[_tileType];

                GameObject tile = Instantiate(tileType, pos, rot);
                tile.name = (tileType.name + _tileType);
                tile.transform.parent = mapFolder;


                //scale walls according to area
                if ((_tileType >= 10 && _tileType < 20))
                    tile.transform.localScale = new Vector3(1, 1f + (area * .35f), 1);

                //if walls, or something other than a regular ground, water, lava, or border tile, make a ground tile as well
                if ((_tileType >= 10 && _tileType < 20) || _tileType >= 50)
                {
                    GameObject groundTileType = mapTiles[Random.Range(0, 3)];
                    GameObject groundTile = Instantiate(groundTileType, pos, rot);
                    groundTile.name = (groundTileType.name);
                    groundTile.transform.parent = mapFolder;
                }


                //set where the player spwans
                if (_tileType == 51)
                {
                    pos.y++;
                    GameManager.enterPoint = pos;
                }

                //did we enter backwards?
                if (_tileType == 52)
                {
                    pos.y++;
                    //GameManager.enterPoint = pos;
                }


            }
        }

    }

    public void EntryExit(int width, int height, float halfWidth, float halfHeight, Vector4[,] points)
    {

        bool foundEntry = false;
        //find entry point
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                if (points[x, y].x == 0 && points[x, y].y >= 3 && foundEntry == false) // if this is ground, and area is 3 or more
                {
                    foundEntry = true;
                    points[x, y].x = 51;
                }

            }
        }

        bool foundExit1 = false;
        bool foundExit2 = false;
        Vector2 exitFoundAt = new Vector2(0, 0);

        //find exit points
        for (int x = width - 1; x > 0; x--)
        {
            for (int y = height - 1; y > 0; y--)
            {

                if (points[x, y].x == 0 && points[x, y].y >= 3 && foundExit1 == false && foundExit2 == false) // if this is ground, and area is 3 or more
                {
                    points[x, y].x = 52;
                    foundExit1 = true;
                    exitFoundAt = new Vector2(x, y);
                }

                /* if (points[x, y].x == 0 && points[x, y].y >= 3 && foundExit1 == true && foundExit2 == false) // if this is ground, and area is 3 or more
                 {
                     if (x < exitFoundAt.x - 10 || x > exitFoundAt.x + 10 || y < exitFoundAt.y - 10 || y > exitFoundAt.y + 10)
                     {
                         points[x, y].x = 53;
                         foundExit2 = true;
                         x = 0;
                         y = 0;
                     }
                 }
                 */

            }
        }

    }


}


