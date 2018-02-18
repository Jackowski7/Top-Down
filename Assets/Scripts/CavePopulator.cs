using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CavePopulator : MonoBehaviour
{

    public GameObject _mapGenerator;
    MapGenerator mapGenerator;

    float mapWidth;
    float mapHeight;
    float borderSize;

    public float pointDensity;
    public GameObject point;
    public Transform pointsFolder;

    public static bool mapReady = false;
    public static bool populated = false;


    void OnValidate()
    { 
        pointDensity = Mathf.Min(pointDensity, 3);
    }

    private void Start()
    {
        mapGenerator = _mapGenerator.GetComponent<MapGenerator>();
    }

    // Update is called once per frame
    void Update()
    {

        if (mapReady == true && populated == false)
        {
            populated = true;
            PopulateCave();
            Debug.Log("ok, Populating!");
        }

    }

    public void PopulateCave()
    {

        float borderSize = mapGenerator.borderSize;
        float mapWidth = mapGenerator.width;
        float mapHeight = mapGenerator.height;

        float halfWidth = (mapWidth / 2 );
        float halfHeight = (mapHeight / 2 );

        for (int i = 0; i < mapWidth * pointDensity; i++)
        {
            for (int j = 0; j < mapHeight * pointDensity; j++)
            {

                GameObject go;
                go = Instantiate(point);
                go.transform.parent = pointsFolder;
                point.transform.position = new Vector3(((i - halfWidth * pointDensity) / pointDensity), 0, ((j -halfHeight * pointDensity) / pointDensity));

            }


        }
    }


}
