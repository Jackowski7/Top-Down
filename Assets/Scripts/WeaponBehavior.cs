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

    public float bulletDamage;
    public string damageType;
    public float bulletKnockback;
    public float bulletSpeed;
    public float bulletSpread;
    public float bulletDurability;
    public float bulletLifetime;

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

        Vector3 pos = player.position;
        Vector3 _rot = player.forward.normalized;
        Quaternion rot = player.rotation;

        if (tag == "Staff")
        {
            pos += _rot;
        }


        GameObject bullet = Instantiate(bulletPrefab, pos, rot);
        BulletBehavior bulletBehavior = bullet.GetComponent<BulletBehavior>();

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        Vector3 dir = player.forward.normalized;
        dir.y = 0;
        dir.x += Random.Range(-bulletSpread / 100, bulletSpread / 100);
        rb.velocity = dir * bulletSpeed + (playerVelocity);

        bulletBehavior.damage = bulletDamage;
        bulletBehavior.damageType = damageType;
        bulletBehavior.durability = bulletDurability;
        bulletBehavior.lifetime = bulletLifetime;
        bulletBehavior.knockback = bulletKnockback;
        bulletBehavior.team = player.tag;

        if (tag != "Staff")
        {
            bulletBehavior.invincible = true;
        }

        return;

    }

}

