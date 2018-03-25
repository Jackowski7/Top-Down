using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBehavior : MonoBehaviour
{
    public GameObject bulletPrefab;

    public bool animated;

    public float fireSpeed;
    public float chargeSpeed;
    public float PlayerRotSlow;
    public float EnergyDrainAmount;

    public bool kineticDamage;
    public bool fireDamage;
    public bool iceDamage;
    public bool electricDamage;

    public float bulletDamage;
    public float bulletKnockback;
    public float bulletSpeed;
    public float bulletSpread;
    public float bulletDurability;
    public float bulletLifetime;

    [HideInInspector]
    public string damageType;

    public bool shieldOut = false;

    void OnValidate()
    {
        fireSpeed = Mathf.Max(fireSpeed, .5f);
        chargeSpeed = Mathf.Max(chargeSpeed, .2f);
        bulletLifetime = Mathf.Max(bulletLifetime, .1f);
    }


    // Use this for initialization
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


    public Vector4 WeaponInfo()
    {
        Vector4 weaponInfo = new Vector4(0, 0, 0, 0);
        weaponInfo.x = fireSpeed;
        weaponInfo.y = chargeSpeed;
        weaponInfo.z = PlayerRotSlow;
        weaponInfo.w = EnergyDrainAmount;
        return weaponInfo;
    }

    public void FireWeapon(Transform player, Vector3 playerVelocity)
    {

        if (tag == "Staff")
        {
            Vector3 pos = player.position;
            Vector3 _rot = player.forward.normalized;
            Quaternion rot = player.rotation;

            pos += _rot;

            GameObject bullet = Instantiate(bulletPrefab, pos, rot);

            Rigidbody rb = bullet.transform.GetComponent<Rigidbody>();
            Vector3 dir = player.forward.normalized;
            dir.y = 0;
            dir.x += Random.Range(-bulletSpread / 100, bulletSpread / 100);

            rb.velocity = dir * bulletSpeed + (playerVelocity);

            float _bulletDamage = bulletDamage * Mathf.RoundToInt((Random.Range(.9f, 1.1f)) * 10) * .1f;

            BulletBehavior bulletBehavior = bullet.GetComponent<BulletBehavior>();
            bulletBehavior.damage = _bulletDamage;
            bulletBehavior.damageType = damageType;
            bulletBehavior.durability = bulletDurability;
            bulletBehavior.lifetime = bulletLifetime;
            bulletBehavior.knockback = bulletKnockback;
            bulletBehavior.team = player.tag;
        }

        if (tag == "Sword")
        {
            Vector3 pos = player.position;
            Vector3 _rot = player.forward.normalized;
            Quaternion rot = player.rotation;

            GameObject bullet = Instantiate(bulletPrefab, pos, rot);

            Rigidbody rb = bullet.transform.GetChild(0).GetComponent<Rigidbody>();
            Vector3 dir = player.forward.normalized;
            bullet.transform.localScale = Vector3.zero;

            float _bulletDamage = bulletDamage * Mathf.RoundToInt((Random.Range(.9f, 1.1f)) * 10) * .1f;

            SwordSwingBehavior swordSwingBehavior = bullet.GetComponent<SwordSwingBehavior>();
            swordSwingBehavior.damage = _bulletDamage;
            swordSwingBehavior.damageType = damageType;
            swordSwingBehavior.knockback = bulletKnockback;
            swordSwingBehavior.team = player.tag;
        }

        return;

    }

}

