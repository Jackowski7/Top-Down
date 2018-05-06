using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterExit : MonoBehaviour
{

    GameManager gameManager;
    Map map;

    // Use this for initialization
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        map = GameObject.Find("Map").GetComponent<Map>();

        GetComponent<Collider>().enabled = false;
        StartCoroutine(Activate());
    }

    IEnumerator Activate()
    {
        yield return new WaitForSeconds(3);
        GetComponent<Collider>().enabled = true;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {

            if (this.tag == "NextArea")
            {
                // NextArea();
            }

            if (this.tag == "PreviousArea")
            {
                // PreviousArea();
            }

        }
    }

}
