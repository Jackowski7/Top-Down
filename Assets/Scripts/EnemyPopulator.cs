using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPopulator : MonoBehaviour
{

    public GameObject enemyPrefab;
    public GameObject enemySpawnPoint;

    string[] enemyNames;

    // Use this for initialization
    void Start()
    {
        enemyNames = new string[1];
        enemyNames[0] = "jim";
    }




    public void SpawnEnemies(int width, int height, float halfWidth, float halfHeight, Vector4[,] points, float seed, float levelDifficulty, Vector3 enterPoint)
    {

        List<Vector2> enemySpawners = new List<Vector2>();
        List<Vector2> enemies = new List<Vector2>();

        enemySpawners.Add(new Vector2(enterPoint.x + (width/2), enterPoint.z + (height / 2) ));
        Debug.Log("spawn - " + enemySpawners[0]);

        int numGroups = (int)(((width - 10) * (height - 10)) / 600 * (1 + (levelDifficulty * .25f)));
        int enemiesPerGroup = (int)(5 * (1 + (levelDifficulty * .25f)));

        for (int n = 0; n < numGroups; n++)
        {

            bool spawnPointFound = false;
            int tries = 100;
            while (spawnPointFound != true && tries > 0)
            {
                int x = Random.Range(0, width);
                int y = Random.Range(0, height);

                //is this point an open ground tile?
                if (points[x, y].x == 0 && points[x, y].y > 2)
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
                        Debug.Log(new Vector2(x, y));


                        Vector3 pos = new Vector3(x - halfWidth + .5f, 0, -(y - halfHeight + .5f) * 1.155f);
                        if (x % 2 == 0) pos.z -= .578f;

                        float _rot = Random.Range(0, 6);
                        Quaternion rot = Quaternion.Euler(0, _rot * 60, 0);

                        GameObject enemy = Instantiate(enemySpawnPoint, pos, rot);
                        enemy.name = (enemyNames[Random.Range(0, enemyNames.Length)]);
                        enemy.transform.parent = transform.Find("Enemies").transform;

                    }
                }

                tries--;
            }
        }



        for (int n = 1; n < enemySpawners.Count; n++)
        {
            float enemiesLeftToSpawnInGroup = enemiesPerGroup;

            int tries = 100;
            while (enemiesLeftToSpawnInGroup > 0 && tries > 0)
            {
                Vector2 spawnerLocation = enemySpawners[n];

                int x = Random.Range((int)spawnerLocation.x - 3, (int)spawnerLocation.x + 3);
                int y = Random.Range((int)spawnerLocation.y - 3, (int)spawnerLocation.y + 3);

                if (x < width - 1 && x > 1 && y < height - 1 && y > 1)
                {
                    // is this point an open ground tile?
                    if (points[x, y].x == 0 && points[x, y].y > 2)
                    {

                        bool tooClose = false;
                        int j = enemies.Count;
                        for (int i = 0; i < j; i++)
                        {
                            // is this point on another enemy point?
                            if ((x == enemies[i].x) && (y == enemies[i].y))
                            {
                                tooClose = true;
                                break;
                            }
                        }

                        if (tooClose == false)
                        {
                            enemies.Add(new Vector2(x, y));

                            Vector3 pos = new Vector3(x - halfWidth + .5f, 1f, -(y - halfHeight + .5f) * 1.155f);
                            if (x % 2 == 0) pos.z -= .578f;

                            float _rot = Random.Range(0, 6);
                            Quaternion rot = Quaternion.Euler(0, _rot * 60, 0);

                            GameObject enemy = Instantiate(enemyPrefab, pos, rot);
                            enemy.name = (enemyNames[Random.Range(0, enemyNames.Length)]);
                            enemy.transform.parent = transform.Find("Enemies").transform;

                            enemiesLeftToSpawnInGroup--;
                        }
                    }
                }

                tries--;
            }
        }
    }


}
