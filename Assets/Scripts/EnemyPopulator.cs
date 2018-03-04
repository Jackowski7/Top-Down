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

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnEnemies(int width, int height, float halfWidth, float halfHeight, Vector4[,] points, float seed, float levelDifficulty)
    {

        Vector2 enterPoint = GameManager.enterPoint;

        int numGroups = (int)(((width - 10) * (height - 10)) / 600 * (1 + (levelDifficulty * .25f)) );
        int enemiesPerGroup = (int)(5 * (1 + (levelDifficulty * .25f) ) );
        int numEnemies = numGroups * enemiesPerGroup;

        Debug.Log(numEnemies +"="+ numGroups + "*" + enemiesPerGroup);


        Vector2[] enemyGroups;
        enemyGroups = new Vector2[(int)numGroups];


        for (int g = 0; g < numGroups; g++)
        {

            bool groupSpawned = false;
            for (int x = 5; x < width - 1 && groupSpawned == false; x++)
            {
                for (int y = 5; y < height - 1 && groupSpawned == false; y++)
                {

                    int randomX = x + Random.Range(-(width / width), (width / width));
                    int randomY = y + Random.Range(-(height / height), (height / height));

                    //is this random point still in the points array?
                    if (randomX < width && randomX > 5 && randomY < height && randomY > 5)
                    {
                        //is this point an open ground tile?
                        if (points[randomX, randomY].x == 0 && points[randomX, randomY].y > 3)
                        {

                            // is this point far enough away from player spawn?
                            if ((randomX >= enterPoint.x + 8 || randomX <= enterPoint.x - 8) && (randomY >= enterPoint.y + 8 || randomY <= enterPoint.y - 8))
                            {
                                bool tooClose = false;
                                for (int h = 0; h < numGroups; h++)
                                {
                                    // is this point close to another enemy spawn point?
                                    if (((randomX > enemyGroups[h].x - (width * 2 / numGroups) && randomX < enemyGroups[h].x + (width * 2 / numGroups)) && (randomY > enemyGroups[h].y - (height * 2 / numGroups) && randomY < enemyGroups[h].y + (height * 2 / numGroups))))
                                    {
                                        tooClose = true;
                                        break;
                                    }
                                }

                                if (tooClose == false)
                                {
                                    enemyGroups[g] = new Vector2(randomX, randomY);
                                    groupSpawned = true;
                                    Debug.Log(enemyGroups[g]);

                                    Vector3 pos = new Vector3(randomX - halfWidth + .5f, 0, -(randomY - halfHeight + .5f) * 1.155f);
                                    if (randomX % 2 == 0) pos.z -= .578f;

                                    float _rot = Random.Range(0, 6);
                                    Quaternion rot = Quaternion.Euler(0, _rot * 60, 0);

                                    GameObject enemy = Instantiate(enemySpawnPoint, pos, rot);
                                    enemy.name = (enemyNames[Random.Range(0, enemyNames.Length)]);
                                    enemy.transform.parent = transform.Find("Enemies").transform;
                                }

                            }
                        }
                    }
                }
            }

            //if we didnt spawn a enemyspawnpoint, we need to try again
            if (groupSpawned == false)
            {
                for (int x = width - 1; x > 5 && groupSpawned == false; x--)
                {
                    for (int y = height - 1; y > 5 && groupSpawned == false; y--)
                    {

                        int randomX = x + Random.Range(-(width / 10), (width / 10));
                        int randomY = y + Random.Range(-(height / 10), (height / 10));

                        //is this random point still in the points array?
                        if (randomX < width && randomX > 5 && randomY < height && randomY > 5)
                        {
                            //is this point an open ground tile?
                            if (points[randomX, randomY].x == 0 && points[randomX, randomY].y > 1)
                            {

                                // is this point far enough away from player spawn?
                                if ((randomX >= enterPoint.x + 8 || randomX <= enterPoint.x - 8) && (randomY >= enterPoint.y + 8 || randomY <= enterPoint.y - 8))
                                {
                                    bool tooClose = false;
                                    for (int h = 0; h < numGroups; h++)
                                    {
                                        // is this point close to another enemy spawn point?
                                        if (((randomX > enemyGroups[h].x - (width * 1 / numGroups) && randomX < enemyGroups[h].x + (width * 1 / numGroups)) && (randomY > enemyGroups[h].y - (height * 1 / numGroups) && randomY < enemyGroups[h].y + (height * 1 / numGroups))))
                                        {
                                            tooClose = true;
                                            break;
                                        }
                                    }

                                    if (tooClose == false)
                                    {
                                        enemyGroups[g] = new Vector2(randomX, randomY);
                                        groupSpawned = true;
                                        Debug.Log("Backup Spawn" + enemyGroups[g]);

                                        Vector3 pos = new Vector3(randomX - halfWidth + .5f, 0, -(randomY - halfHeight + .5f) * 1.155f);
                                        if (randomX % 2 == 0) pos.z -= .578f;

                                        float _rot = Random.Range(0, 6);
                                        Quaternion rot = Quaternion.Euler(0, _rot * 60, 0);

                                        GameObject enemy = Instantiate(enemySpawnPoint, pos, rot);
                                        enemy.name = (enemyNames[Random.Range(0, enemyNames.Length)]);
                                        enemy.transform.parent = transform.Find("Enemies").transform;
                                    }

                                }
                            }
                        }
                    }
                }

            }

            //if we didnt spawn a enemyspawnpoint, we need to try again
            if (groupSpawned == false)
            {
                for (int x = 5; x < width - 1 && groupSpawned == false; x++)
                {
                    for (int y = 5; y < height - 1 && groupSpawned == false; y++)
                    {

                        int randomX = x + Random.Range(-(width / 5), (width / 5));
                        int randomY = y + Random.Range(-(height / 5), (height / 5));

                        //is this random point still in the points array?
                        if (randomX < width && randomX > 5 && randomY < height && randomY > 5)
                        {
                            //is this point an open ground tile?
                            if (points[randomX, randomY].x == 0 && points[randomX, randomY].y > 0)
                            {

                                // is this point far enough away from player spawn?
                                if ((randomX >= enterPoint.x + 8 || randomX <= enterPoint.x - 8) && (randomY >= enterPoint.y + 8 || randomY <= enterPoint.y - 8))
                                {
                                    bool tooClose = false;
                                    for (int h = 0; h < numGroups; h++)
                                    {
                                        // is this point close to another enemy spawn point?
                                        if (((randomX > enemyGroups[h].x - (width * .25 / numGroups) && randomX < enemyGroups[h].x + (width * .25 / numGroups)) && (randomY > enemyGroups[h].y - (height * .25 / numGroups) && randomY < enemyGroups[h].y + (height * .25 / numGroups))))
                                        {
                                            tooClose = true;
                                            break;
                                        }
                                    }

                                    if (tooClose == false)
                                    {
                                        enemyGroups[g] = new Vector2(randomX, randomY);
                                        groupSpawned = true;
                                        Debug.Log("SECOND Backup Spawn" + enemyGroups[g]);

                                        Vector3 pos = new Vector3(randomX - halfWidth + .5f, 0, -(randomY - halfHeight + .5f) * 1.155f);
                                        if (randomX % 2 == 0) pos.z -= .578f;

                                        float _rot = Random.Range(0, 6);
                                        Quaternion rot = Quaternion.Euler(0, _rot * 60, 0);

                                        GameObject enemy = Instantiate(enemySpawnPoint, pos, rot);
                                        enemy.name = (enemyNames[Random.Range(0, enemyNames.Length)]);
                                        enemy.transform.parent = transform.Find("Enemies").transform;
                                    }

                                }
                            }
                        }
                    }
                }

            }

        }

        


        for (int g = 0; g < numGroups; g++)
        {

            float enemiesLeftToSpawnInGroup = enemiesPerGroup;

            for (int x = (int)enemyGroups[g].x - 3; x < (int)enemyGroups[g].x + 3  && enemiesLeftToSpawnInGroup >= 0; x++)
            {
                for (int y = (int)enemyGroups[g].y - 3; y < (int)enemyGroups[g].y + 3 && enemiesLeftToSpawnInGroup >= 0; y++)
                {
                    int randomX = x + Random.Range(-1, 1);
                    int randomY = y + Random.Range(-1, 1);

                    // is this point still in the points array?
                    if (randomX < width && randomX > 0 && randomY < height && randomY > 0)
                    {
                        // is this point an open ground tile?
                        if (points[randomX, randomY].x == 0 && points[randomX, randomY].y > 1)
                        {

                            Vector3 pos = new Vector3(randomX - halfWidth + .5f, 0, -(randomY - halfHeight + .5f) * 1.155f);
                            if (randomX % 2 == 0) pos.z -= .578f;

                            float _rot = Random.Range(0, 6);
                            Quaternion rot = Quaternion.Euler(0, _rot * 60, 0);

                            GameObject enemy = Instantiate(enemyPrefab, pos, rot);
                            enemy.name = (enemyNames[Random.Range(0, enemyNames.Length)]);
                            enemy.transform.parent = transform.Find("Enemies").transform;

                            enemiesLeftToSpawnInGroup--;

                        }
                    }

                }
            }

            if (enemiesLeftToSpawnInGroup > 0)
            {
                for (int x = (int)enemyGroups[g].x - 3; x < (int)enemyGroups[g].x + 3 && enemiesLeftToSpawnInGroup >= 0; x++)
                {
                    for (int y = (int)enemyGroups[g].y - 3; y < (int)enemyGroups[g].y + 3 && enemiesLeftToSpawnInGroup >= 0; y++)
                    {
                        int randomX = x + Random.Range(-1, 1);
                        int randomY = y + Random.Range(-1, 1);

                        // is this point still in the points array?
                        if (randomX < width && randomX > 0 && randomY < height && randomY > 0)
                        {
                            // is this point an open ground tile?
                            if (points[randomX, randomY].x == 0 && points[randomX, randomY].y > 0)
                            {

                                Vector3 pos = new Vector3(randomX - halfWidth + .5f, 0, -(randomY - halfHeight + .5f) * 1.155f);
                                if (randomX % 2 == 0) pos.z -= .578f;

                                float _rot = Random.Range(0, 6);
                                Quaternion rot = Quaternion.Euler(0, _rot * 60, 0);

                                GameObject enemy = Instantiate(enemyPrefab, pos, rot);
                                enemy.name = (enemyNames[Random.Range(0, enemyNames.Length)]) + ".2";
                                enemy.transform.parent = transform.Find("Enemies").transform;

                                enemiesLeftToSpawnInGroup--;

                            }
                        }

                    }
                }
            }

        }
    }


}
