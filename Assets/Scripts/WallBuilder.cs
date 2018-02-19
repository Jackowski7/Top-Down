using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBuilder : MonoBehaviour
{

    public static WallBuilder instance;
    public GameObject _wallBlock;
    public GameObject _wallBlockX;
    public Transform _wallFolder;

    public static GameObject wallBlock
    {
        get
        {
            return instance._wallBlock;
        }
    }

    public static GameObject wallBlockX
    {
        get
        {
            return instance._wallBlockX;
        }
    }



    public static Transform wallFolder
    {
        get
        {
            return instance._wallFolder;
        }
    }

    void Awake()
    {
        instance = this;
    }

    public static void BuildWalls(int mapWidth, int mapHeight, int[,] closedPoints, int[,] border, int borderSize)
    {

        foreach (Transform child in wallFolder)
        {
            GameObject.Destroy(child.gameObject);
        }

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (closedPoints[x, y] > 0)
                {
                    Vector3 pos = new Vector3(x - (mapWidth / 2) + .5f, 0, (y - (mapHeight / 2) + .5f) * 1.155f);
                    if (x % 2 == 0)
                    {
                        pos.z += .578f;
                    }
                    GameObject wallBlock = Instantiate(WallBuilder.wallBlock, pos, Quaternion.Euler(0, 90, 0));
                    wallBlock.transform.localScale = new Vector3(1, 3 + (closedPoints[x, y] * .2f), 1);
                    wallBlock.name = "WallBlock";
                    wallBlock.transform.parent = wallFolder;
                }
            }
        }

        for (int x = 0; x < mapWidth + borderSize * 2 ; x++ )
        {
            for (int y = 0; y < mapHeight + borderSize * 2 ; y++)
            {
                if (border[x,y] == 1)
                {
                    Vector3 pos = new Vector3(x - (mapWidth / 2) - borderSize - .5f, 0, (y - (mapHeight / 2) - borderSize - .5f) * 1.155f);
        if (x % 2 == 0)
                    {
                        pos.z += .578f;
                    }
                    GameObject wallBlockX = Instantiate(WallBuilder.wallBlockX, pos, Quaternion.Euler(0, 90, 0));
                    wallBlockX.transform.localScale = new Vector3(1, 3 + Random.Range(0.1f,1), 1);
                    wallBlockX.name = "WallBlockX";
                    wallBlockX.transform.parent = wallFolder;
                }
                if (border[x, y] == -1)
                {
                    Vector3 pos = new Vector3(x - (mapWidth / 2) - borderSize - .5f, 0, (y - (mapHeight / 2) - borderSize - .5f) * 1.155f);
                    if (x % 2 == 0)
                    {
                        pos.z += .578f;
                    }
                    GameObject wallBlock = Instantiate(WallBuilder.wallBlock, pos, Quaternion.Euler(0, 90, 0));
                    wallBlock.transform.localScale = new Vector3(1, 3 + Random.Range(0.1f, 1), 1);
                    wallBlock.name = "WallBlock";
                    wallBlock.transform.parent = wallFolder;
                }
            }
        }

    }



}
