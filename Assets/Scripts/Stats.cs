using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour {

    public float maxHealth;
    public float health;
    public float healthRecoverSpeed;
    float lastDamagedTime;

    public float maxEnergy;    
    public float energy;
    public float energyRecoverSpeed;
    public float lastEnergyUsedTime;

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

        if (health <= maxHealth)
        {
            if (Time.time > lastDamagedTime + 15f)
            {
                health = Mathf.Min((health + (healthRecoverSpeed * 3 * Time.smoothDeltaTime)), maxHealth);
            }
            else
            {
                health = Mathf.Min((health + (healthRecoverSpeed * Time.smoothDeltaTime)), maxHealth);
            }
        }
        if (energy <= maxEnergy)
        {
            if (Time.time > lastEnergyUsedTime + 9f)
            {
                energy = Mathf.Min((energy + (energyRecoverSpeed * 3 * Time.smoothDeltaTime)), maxEnergy);
            }
            else
            {
                energy = Mathf.Min((energy + (energyRecoverSpeed * Time.smoothDeltaTime)), maxEnergy);
            }

        }

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
            lastDamagedTime = Time.time;
        }
        else
        {
            //Debug.Log("Got hit a 0");
        }
    }
}
