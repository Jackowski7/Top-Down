using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour {

    public float health;
    public float energy;

    public float speed;
    public float rotationSpeed;


    public float thornsDamage;
    public string thornsDamageType;
    public float thornsKnockback;

    public bool thornsKineticDamage;
    public bool thornsFireDamage;
    public bool thornsIceDamage;
    public bool thornsElectricDamage;

    public GameObject hitMarker;

    [HideInInspector]
    public bool dead;
    [HideInInspector]
    public bool invincible;
    [HideInInspector]
    public bool shielding;

    void Start()
    {
        if (thornsKineticDamage == true || (thornsKineticDamage != true && thornsFireDamage != true && thornsIceDamage != true && thornsElectricDamage != true))
        {
            thornsDamageType = "Kinetic";
        }
        else if (thornsFireDamage == true)
        {
            thornsDamageType = "Fire";
        }
        else if (thornsIceDamage == true)
        {
            thornsDamageType = "Ice";
        }
        else if (thornsElectricDamage == true)
        {
            thornsDamageType = "Electric";
        }
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void Damage(float damageAmount, string damageType, Vector3 hitDirection, GameObject entity)
    { 

        if (damageAmount > 0)
        {
            health -= damageAmount;
            GameObject hit = Instantiate(hitMarker);
            hit.transform.position = transform.position - hitDirection;
            hit.transform.rotation = Quaternion.Euler(90, 0, 0);

            HitMarker hitScript = hit.GetComponent<HitMarker>();

            hitScript.SetHitMarker(damageAmount, damageType, entity);
        }
        else
        {
            //Debug.Log("Got hit a 0");
        }
    }
}
