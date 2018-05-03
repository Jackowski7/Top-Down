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

    public void MovePlayer()
    {
        player.transform.position = enterPoint;
    }

}
