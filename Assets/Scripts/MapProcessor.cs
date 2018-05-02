using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapProcessor : MonoBehaviour
{

    [HideInInspector]
    public int width;
    [HideInInspector]
    public int height;
    float seed;
    public Vector4[,] points;

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


    public void ProcessMap(int _width, int _height, int outerBorderSize, int innerBorderSize, int mapWidth, int mapHeight, Vector4[,] _points, int[,] generatedMap, float _seed)
    {


        width = _width;
        height = _height;
        points = _points;
        seed = _seed;


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
                    points[x, y].x = generatedMap[x - (innerBorderSize + outerBorderSize), y - (innerBorderSize + outerBorderSize)]; // set to map


            }
        }

        CalculateAreas();
    }


    public void CalcAdjacentTiles()
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

                    if (points[(int)d0.x, (int)d0.y].x != tileType && ( points[(int)d0.x, (int)d0.y].x != 2 && points[(int)d0.x, (int)d0.y].x != 3))
                    {
                        adj++; x0 = true;
                    }
                    if (points[(int)d1.x, (int)d1.y].x != tileType && ( points[(int)d1.x, (int)d1.y].x != 2 && points[(int)d1.x, (int)d1.y].x != 3))
                    {
                        adj++; x1 = true;
                    }
                    if (points[(int)d2.x, (int)d2.y].x != tileType && ( points[(int)d2.x, (int)d2.y].x != 2 && points[(int)d2.x, (int)d2.y].x != 3))
                    {
                        adj++; x2 = true;
                    }
                    if (points[(int)d3.x, (int)d3.y].x != tileType && ( points[(int)d3.x, (int)d3.y].x != 2 && points[(int)d3.x, (int)d3.y].x != 3))
                    {
                        adj++; x3 = true;
                    }
                    if (points[(int)d4.x, (int)d4.y].x != tileType && ( points[(int)d4.x, (int)d4.y].x != 2 && points[(int)d4.x, (int)d4.y].x != 3))
                    {
                        adj++; x4 = true;
                    }
                    if (points[(int)d5.x, (int)d5.y].x != tileType && ( points[(int)d5.x, (int)d5.y].x != 2 && points[(int)d5.x, (int)d5.y].x != 3))
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

                    if(adj > 0)
                    points[x, y].w = leftmost;
                    points[x, y].z = adj;
                }

            }
        }
    }

    public void SmoothMap()
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


    public void CalculateAreas()
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


    public void CreateLake()
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

}