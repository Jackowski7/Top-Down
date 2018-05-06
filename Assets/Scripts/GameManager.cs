using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    GameObject player;
    GameObject eventSystem;
    Map map;

    public Vector4[] path;
    public int pathProgress;


    // Use this for initialization
    void Awake()
    {
        player = transform.Find("Player").gameObject;
        eventSystem = transform.Find("EventSystem").gameObject;
        map = GameObject.Find("Map").GetComponent<Map>();

        path = new Vector4[100];

        GeneratePath();
    }


    void GeneratePath()
    {
        // x = seed (and ID)
        // y = 'set' and difficulty
        // z = biome

        float y = 0;
        int z = Random.Range(0, 4);
        for (int x = 0; x < path.Length; x++)
        {
            path[x].x = x;

            y += Random.Range(0.1f, 0.4f);
            path[x].y = (int)y;


            if (x > 0)
            {
                if (path[x - 1].y != path[x].y)
                {
                    z = Random.Range(0, 4);
                }
            }

            path[x].z = z;

            path[x].w = 0;
        }

        map.NextArea();
}

    public void MovePlayer()
    {
        //player.transform.position = enterPoint;
    }

}
