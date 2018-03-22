using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBehavior : MonoBehaviour
{
    public GameObject bulletPrefab;

    public bool sword;
    public bool staff;
    public bool shield;

    public float fireSpeed;
    public float ChargeTime;
    public float PlayerRotSlow;
    public float EnergyDrainAmount;

    public float bulletDamage;
    public float bulletKnockback;
    public float bulletSpeed;
    public float bulletSpread;
    public float bulletDurability;
    public float bulletLifetime;

    public bool shieldOut = false;

    void OnValidate()
    {
        fireSpeed = Mathf.Max(fireSpeed, .5f);
        ChargeTime = Mathf.Max(ChargeTime, .2f);
    }


    // Use this for initialization
    void Start()
    {
        
    }


    public Vector4 WeaponInfo()
    {
        Vector4 weaponInfo = new Vector4(0, 0, 0, 0);
        weaponInfo.x = fireSpeed;
        weaponInfo.y = ChargeTime;
        weaponInfo.z = PlayerRotSlow;
        weaponInfo.w = EnergyDrainAmount;
        return weaponInfo;
    }

    public void FireWeapon(Transform playerBody, Vector3 playerVelocity)
    {

        Vector3 pos = playerBody.transform.position;
        Vector3 _rot = playerBody.transform.forward;
        Quaternion rot = playerBody.transform.rotation;

        pos += _rot;


        GameObject bullet = Instantiate(bulletPrefab, pos, rot);
        bulletBehavior bulletBehavior = bullet.GetComponent<bulletBehavior>();
        bulletBehavior.damage = bulletDamage;
        bulletBehavior.durability = bulletDurability;
        bulletBehavior.lifetime = bulletLifetime;
        bulletBehavior.knockback = bulletKnockback;

        //get target, and don't hit friendlies
        string user = playerBody.transform.parent.tag;
        string target = null;
        string firer = null;

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

        bulletBehavior.target = target;
        bulletBehavior.firer = firer;

        if (tag == "Staff")
        {
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            Vector3 dir = playerBody.forward.normalized;
            dir.x += Random.Range(-bulletSpread / 100, bulletSpread / 100);
            rb.velocity = dir * bulletSpeed + (playerVelocity);
        }

        return;

    }

}

