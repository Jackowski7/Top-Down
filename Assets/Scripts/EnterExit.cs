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

        Deactivate();
    }

    public void Deactivate()
    {
        StopCoroutine(Activate());
        GetComponent<Collider>().enabled = false;
        StartCoroutine(Activate());
    }

    IEnumerator Activate()
    {
        yield return new WaitForSeconds(5);
        GetComponent<Collider>().enabled = true;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {

            if (this.tag == "NextArea")
            {
                 map.NextArea();
            }

            if (this.tag == "PreviousArea")
            {
                map.PreviousArea();
            }

        }
    }

}
