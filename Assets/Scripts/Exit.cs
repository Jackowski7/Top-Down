using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour {

    MapGenerator mapGenerator;

	// Use this for initialization
	void Start () {
        mapGenerator = transform.parent.GetComponent<MapGenerator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            mapGenerator.GenerateMap();
        }
    }
}
