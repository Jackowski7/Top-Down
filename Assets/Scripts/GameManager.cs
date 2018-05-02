using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    [HideInInspector]
    public GameObject player;
    [HideInInspector]
    public GameObject eventSystem;

    public int level;

    public Vector3[] currentPath;
    public int pathProgress;
    public static Vector3 enterPoint;


    // Use this for initialization
    void Start() {

        player = transform.Find("Player").gameObject;
        eventSystem = transform.Find("EventSystem").gameObject;
    }

    public void StartGame()
    {
        eventSystem.transform.Find("Screen").Find("Welcome Menu").gameObject.SetActive(false);
    }

    public void LevelFinished()
    {
        eventSystem.transform.Find("Screen").Find("Level Summary").gameObject.SetActive(true);
    }


    // Update is called once per frame
    void Update() {

        if (Input.GetAxisRaw("NewMap") != 0)
        {
            CloseAllWindows();
        }

    }

    public void CloseAllWindows()
    {
        eventSystem.transform.Find("Screen").Find("Level Summary").gameObject.SetActive(false);
        eventSystem.transform.Find("Screen").Find("Welcome Menu").gameObject.SetActive(false);
    }

    public void MovePlayer()
    {
        player.transform.position = enterPoint;
    }

}
