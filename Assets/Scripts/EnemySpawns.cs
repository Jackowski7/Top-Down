using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawns : MonoBehaviour {

    GameManager gameManager;
    public GameObject enemyPrefab;
    public GameObject enemySpawnPoint;
    GameObject enemiesFolder;


    string[] enemyNames;

    // Use this for initialization
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        enemyNames = new string[1];
        enemyNames[0] = "jim";
    }
    
    public void SpawnEnemies(int width, int height, float halfWidth, float halfHeight, Vector4[,] points, float seed, float levelDifficulty, Vector2 enterPointXY, Vector2 exitPointXY, int numberInSet)
    {

        enemiesFolder = new GameObject("Enemies");
        enemiesFolder.transform.parent = this.transform;

        List<Vector2> enemySpawners = new List<Vector2>();
        List<Vector2> enemies = new List<Vector2>();

        enemySpawners.Add(new Vector2(enterPointXY.x, enterPointXY.y));
        enemySpawners.Add(new Vector2(exitPointXY.x, exitPointXY.y));
        //Debug.Log("spawn - " + enemySpawners[0]);

        int numGroups = (int)(((width - 10) * (height - 10)) / 500 * (1 + (levelDifficulty * .25f)));
        int enemiesPerGroup = (int)(1 * (1 + (levelDifficulty * .25f)));

        for (int n = 0; n < numGroups; n++)
        {

            bool spawnPointFound = false;
            int tries = 100;
            while (spawnPointFound != true && tries > 0)
            {
                int x = Random.Range(0, width);
                int y = Random.Range(0, height);

                //is this point an open ground tile?
                if (points[x, y].x == 0 && points[x, y].y > 3)
                {

                    bool tooClose = false;
                    int j = enemySpawners.Count;
                    for (int i = 0; i < j; i++)
                    {

                        // is this point close to another enemy spawn point?
                        if (((x > enemySpawners[i].x - 10) && x < enemySpawners[i].x + 10) && (y > enemySpawners[i].y - 10 && y < enemySpawners[i].y + 10))
                        {
                            tooClose = true;
                            break;
                        }
                    }

                    if (tooClose == false)
                    {
                        enemySpawners.Add(new Vector2(x, y));
                        spawnPointFound = true;
                        //Debug.Log(new Vector2(x, y));


                        Vector3 pos = new Vector3(x - halfWidth + .5f + ((width + 5) * numberInSet), 0, -(y - halfHeight + .5f) * 1.155f);
                        if (x % 2 == 0) pos.z -= .578f;

                        float _rot = Random.Range(0, 6);
                        Quaternion rot = Quaternion.Euler(0, _rot * 60, 0);

                        GameObject enemy = Instantiate(enemySpawnPoint, pos, rot);
                        enemy.name = "EnemySpawner";
                        enemy.transform.parent = enemiesFolder.transform;

                    }
                }

                tries--;
            }
        }
    }
}
