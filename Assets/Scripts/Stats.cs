using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour {

    public float health;
    public float energy;

    public GameObject hitMarker;

    [HideInInspector]
    public bool dead;
    [HideInInspector]
    public bool invincible;

    public bool shielding;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Damage(float damageAmount, string damageType, Vector3 hitDirection, GameObject entity)
    {
        health -= damageAmount;
        GameObject hit = Instantiate(hitMarker);
        hit.transform.position = transform.position - hitDirection;
        hit.transform.rotation = Quaternion.Euler(90,0,0);

        HitMarker hitScript = hit.GetComponent<HitMarker>();
        hitScript.SetHitMarker(damageAmount, damageType, entity);
    }
}
