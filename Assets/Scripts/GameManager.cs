using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject player;
    public GameObject eventSystem;
    bool menuActive = true;


    NewMap newMap;
    float seed;
    float levelDifficulty = 1;

    public static bool enterBackwards;
    public static Vector2 enterPoint;


    // Use this for initialization
    void Start() {

        player = transform.Find("Player").gameObject;
        eventSystem = transform.Find("EventSystem").gameObject;
        newMap = GetComponent<NewMap>();

    }

    public void StartGame()
    {
        BuildMap();
        menuActive = false;
        eventSystem.transform.Find("Screen").Find("Welcome Menu").gameObject.SetActive(false);
    }

    public void LevelFinished()
    {
        menuActive = true;
        eventSystem.transform.Find("Screen").Find("Level Summary").gameObject.SetActive(true);
        levelDifficulty++;
    }

    public void NextLevel()
    {
        BuildMap();
        menuActive = false;
        eventSystem.transform.Find("Screen").Find("Level Summary").gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update() {

        if (menuActive == true)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }

        if (Input.GetAxisRaw("NewMap") != 0)
        {
            CloseAllWindows();
            NextLevel();
        }

    }

    public void CloseAllWindows()
    {
        eventSystem.transform.Find("Screen").Find("Level Summary").gameObject.SetActive(false);
        eventSystem.transform.Find("Screen").Find("Welcome Menu").gameObject.SetActive(false);
    }

    public void MovePlayer(Vector3 startPoint)
    {
        player.transform.position = startPoint;
    }

    public void BuildMap()
    {

        seed = Time.unscaledTime;
        newMap.BuildMap(seed, levelDifficulty);

    }

}
