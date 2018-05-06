using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{

    GameManager gameManager;

    float level;
    float biome;

    public GameObject testEnemy;



    // Use this for initialization
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        level = gameManager.path[gameManager.pathProgress].y;
        biome = gameManager.path[gameManager.pathProgress].z;
        int numEnemies = Random.Range(1, 4);

        for (int x = 0; x < numEnemies; x++)
        {
            Vector3 pos = transform.position;
            pos.x += Random.Range(-3, 3);
            pos.z += Random.Range(-3, 3);
            Quaternion rot = Quaternion.Euler(0, Random.Range(0,360), 0);

            GameObject enemy = Instantiate(testEnemy, pos, rot);
            enemy.transform.parent = transform;
        }

    }
}
