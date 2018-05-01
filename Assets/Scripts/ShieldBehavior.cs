using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBehavior : MonoBehaviour
{

    public float fireSpeed;
    public float chargeSpeed;
    public float PlayerRotSlow;

    public float EnergyDrainAmount;
    public float knockBack;

    public bool kineticDamage;
    public bool fireDamage;
    public bool iceDamage;
    public bool electricDamage;

    [HideInInspector]
    public string damageType;
    public float baseDamageAbsorb;
    public float damageAbsorbPercent;

    Transform playerBody;

    string target = null;
    string firer = null;

    void OnValidate()
    {
        fireSpeed = Mathf.Max(fireSpeed, .5f);
        chargeSpeed = Mathf.Max(chargeSpeed, .2f);
    }

    void Start()
    {
        if (kineticDamage == true || (kineticDamage != true && iceDamage != true && fireDamage != true && electricDamage != true))
        {
            damageType = "Kinetic";
        }
        else if (fireDamage == true)
        {
            damageType = "Fire";
        }
        else if (iceDamage == true)
        {
            damageType = "Ice";
        }
        else if (electricDamage == true)
        {
            damageType = "Electric";
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == target)
        {
            Debug.Log("hit the shield");
            Vector3 dir = col.transform.position - transform.position;
            Rigidbody rb = col.gameObject.GetComponent<Rigidbody>();
            rb.AddForce(dir.normalized * knockBack, ForceMode.Impulse);
        }
    }


    public Vector4 ShieldInfo()
    {
        Vector4 weaponInfo = new Vector4(0, 0, 0, 0);
        weaponInfo.x = fireSpeed;
        weaponInfo.y = chargeSpeed;
        weaponInfo.z = PlayerRotSlow;
        weaponInfo.w = EnergyDrainAmount;
        return weaponInfo;
    }

    public void DrawShield(Transform _playerBody)
    {

        playerBody = _playerBody;

        string user = playerBody.transform.parent.tag;

        if (user == "Enemy")
        {
            target = "Player";
            firer = "Enemy";
        }

        if (user == "Player")
        {
            target = "Enemy";
            firer = "Player";
        }

        Stats stats = playerBody.GetComponent<Stats>();
        stats.shielding = true;

    }

    public void FireShield()
    {
        //Debug.Log("Fired my shield?");
    }

    public void PutAway()
    {
        Stats stats = playerBody.GetComponent<Stats>();
        stats.shielding = false;
    }




}

