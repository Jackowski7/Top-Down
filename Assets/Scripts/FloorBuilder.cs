using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorBuilder : MonoBehaviour
{

    public static FloorBuilder instance;
    public GameObject _floorBlock;
    public Transform _floorFolder;

    public static GameObject floorBlock
    {
        get
        {
            return instance._floorBlock;
        }
    }

    public static Transform floorFolder
    {
        get
        {
            return instance._floorFolder;
        }
    }

    void Awake()
    {
        instance = this;
    }

    public static void BuildFloor(int mapWidth, int mapHeight, int[,] openPoints, int[,] border, int borderSize)
    {

        foreach (Transform child in floorFolder)
        {
            GameObject.Destroy(child.gameObject);
        }

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (openPoints[x, y] >= 0)
                {
                    Vector3 pos = new Vector3(x - (mapWidth / 2) + .5f, -1, (y - (mapHeight / 2) + .5f) * 1.155f);
        if (x % 2 == 0)
                    {
                        pos.z += .578f;
                    }
                    GameObject floorBlock = Instantiate(FloorBuilder.floorBlock, pos, Quaternion.Euler(0, 90, 0));
                    floorBlock.transform.localScale = new Vector3(1, 1 - (openPoints[x, y] * .05f), 1);
                    floorBlock.name = "FloorBlock";
                    floorBlock.transform.parent = floorFolder;
                }
            }
        }

        for (int x = 0; x < mapWidth + borderSize * 2; x++)
        {
            for (int y = 0; y < mapHeight + borderSize * 2; y++)
            {
                if (border[x, y] == 1 || border[x, y] == -1)
                {
                    Vector3 pos = new Vector3(x - (mapWidth / 2) - borderSize - .5f, -1, (y - (mapHeight / 2) - borderSize - .5f) * 1.155f);
                    if (x % 2 == 0)
                    {
                        pos.z += .578f;
                    }
                    GameObject floorBlock = Instantiate(FloorBuilder.floorBlock, pos, Quaternion.Euler(0, 90, 0));
                    floorBlock.transform.localScale = new Vector3(1, 1, 1);
                    floorBlock.name = "FloorBlock";
                    floorBlock.transform.parent = floorFolder;
                }

            }
        }



    }

}
