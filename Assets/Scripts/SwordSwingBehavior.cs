﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSwingBehavior : MonoBehaviour
{

    Transform player;
    WeaponBehavior weapon;

    [HideInInspector]
    public string team;
    [HideInInspector]
    public string damageType;
    [HideInInspector]
    public float damage;
    [HideInInspector]
    public float knockback;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(DestroySelf());
    }

    // Update is called once per frame
    void Update()
    {
    }

    bool ShieldDirection(float shieldDir, float bulletDir)
    {
        float a = Mathf.Max(shieldDir, bulletDir);
        float b = Mathf.Min(shieldDir, bulletDir);

        if (a - b > 140 && a - b < 220)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnTriggerEnter(Collider col)
    {

        if (col.tag == "Enemy" || col.tag == "Player")
        {
            if (col.gameObject.tag != team)
            {
                Stats stats = col.gameObject.GetComponent<Stats>();

                if (stats.shielding == true)
                {

                    Transform shield = col.transform.Find("Equipment").Find("ShieldSlot").GetChild(0).transform;
                    ShieldBehavior shieldBehavior = shield.GetComponent<ShieldBehavior>();

                    bool shieldFacingBullet = ShieldDirection(col.transform.eulerAngles.y, transform.rotation.eulerAngles.y);

                    if (shieldFacingBullet == true)
                    {
                        if (shieldBehavior.damageType == damageType)
                        {
                            damage -= shieldBehavior.damageAbsorb;
                        }
                        else
                        {
                            damage -= (shieldBehavior.damageAbsorb / 2);
                        }

                        knockback = knockback / 2;
                    }

                }

                stats.Damage(Mathf.Max(0, damage), damageType, transform.forward, col.gameObject);
                stats.GetComponent<Rigidbody>().AddForce(transform.forward * knockback, ForceMode.Impulse);

            }
        }

        else if (col.tag == "Destructible" || col.tag == "Effect")
        {
            //
        }

    }

    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(.1f);
        Destroy(gameObject);
    }

}