using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorBuilder : MonoBehaviour
{

    public static FloorBuilder instance;
    public GameObject _floorBlock;
    public Transform _floorFolder;
    public GameObject _baseFloor;
    public static GameObject[,] floorBlocks;

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

    public static GameObject baseFloor
    {
        get
        {
            return instance._baseFloor;
        }
    }

    void Awake()
    {
        instance = this;
    }

    public static void BuildFloor(int mapWidth, int mapHeight, int[,] openPoints, int[,] border, int borderSize)
    {

        floorBlocks = new GameObject[(int)(mapWidth + borderSize * 2), (int)(mapHeight + borderSize * 2)];

        foreach (Transform child in floorFolder)
        {
            GameObject.Destroy(child.gameObject);
        }

        Vector3 floorPos = instance.transform.position;
        floorPos.y = -1.5f;
        floorPos.z += .5f;
        GameObject baseFloor = Instantiate(FloorBuilder.baseFloor, floorPos, instance.transform.rotation);
        baseFloor.transform.localScale = new Vector3(mapWidth + borderSize + 3, 1, mapHeight + borderSize + 3);
        baseFloor.transform.parent = floorFolder;

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {


                Vector3 pos = new Vector3(x - (mapWidth / 2) + .5f, -1, (y - (mapHeight / 2) + .5f) * 1.155f);
                if (x % 2 == 0)
                    pos.z += .578f;

                if (openPoints[x, y] >= 0 && openPoints[x, y] >= 0)
                {
                    floorBlocks[x, y] = Instantiate(FloorBuilder.floorBlock, pos, Quaternion.Euler(0, 90, 0));
                    floorBlocks[x, y].transform.localScale = new Vector3(1, 1 - (openPoints[x, y] * .05f), 1);
                    floorBlocks[x, y].name = "FloorBlock";
                    floorBlocks[x, y].transform.parent = floorFolder;
                }

            }
        }

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {

                if (openPoints[x, y] >= 8)
                {

                    for (int i = 0; i < 2 && x + i < mapWidth && y + i < mapHeight && x - i > 0 && y - i > 0; i++)
                    {
                        //set flood child inavtice
                        floorBlocks[x, y].transform.GetChild(0).gameObject.SetActive(false);
                        floorBlocks[x, y + i].transform.GetChild(0).gameObject.SetActive(false);
                        floorBlocks[x, y - i].transform.GetChild(0).gameObject.SetActive(false);
                        floorBlocks[x + i, y + i].transform.GetChild(0).gameObject.SetActive(false);
                        floorBlocks[x + i, y - i].transform.GetChild(0).gameObject.SetActive(false);
                        floorBlocks[x - i, y + i].transform.GetChild(0).gameObject.SetActive(false);
                        floorBlocks[x - i, y - i].transform.GetChild(0).gameObject.SetActive(false);
                        floorBlocks[x + i, y].transform.GetChild(0).gameObject.SetActive(false);
                        floorBlocks[x - i, y].transform.GetChild(0).gameObject.SetActive(false);

                        //set lava child active
                        floorBlocks[x, y].transform.GetChild(1).gameObject.SetActive(true);
                        floorBlocks[x, y + i].transform.GetChild(1).gameObject.SetActive(true);
                        floorBlocks[x, y - i].transform.GetChild(1).gameObject.SetActive(true);
                        floorBlocks[x + i, y + i].transform.GetChild(1).gameObject.SetActive(true);
                        floorBlocks[x + i, y - i].transform.GetChild(1).gameObject.SetActive(true);
                        floorBlocks[x - i, y + i].transform.GetChild(1).gameObject.SetActive(true);
                        floorBlocks[x - i, y - i].transform.GetChild(1).gameObject.SetActive(true);
                        floorBlocks[x + i, y].transform.GetChild(1).gameObject.SetActive(true);
                        floorBlocks[x - i, y].transform.GetChild(1).gameObject.SetActive(true);

                        //set lava blocks to same height
                        floorBlocks[x, y].transform.localScale = Vector3.one;
                        floorBlocks[x, y + i].transform.localScale = Vector3.one;
                        floorBlocks[x, y - i].transform.localScale = Vector3.one;
                        floorBlocks[x + i, y + i].transform.localScale = Vector3.one;
                        floorBlocks[x + i, y - i].transform.localScale = Vector3.one;
                        floorBlocks[x - i, y + i].transform.localScale = Vector3.one;
                        floorBlocks[x - i, y - i].transform.localScale = Vector3.one;
                        floorBlocks[x + i, y].transform.localScale = Vector3.one;
                        floorBlocks[x - i, y].transform.localScale = Vector3.one;
                    }

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
