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
        bulletLifetime = Mathf.Max(chargeSpeed, .1f);
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
        pos += _rot;    

        GameObject bullet = Instantiate(bulletPrefab, pos, rot);
        bulletBehavior bulletBehavior = bullet.GetComponent<bulletBehavior>();

        if (tag == "Staff")
        {
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            Vector3 dir = player.forward.normalized;
            dir.y = 0;
            dir.x += Random.Range(-bulletSpread / 100, bulletSpread / 100);
            rb.velocity = dir * bulletSpeed + (playerVelocity);
        }

        bulletBehavior.damage = bulletDamage;
        bulletBehavior.durability = bulletDurability;
        bulletBehavior.lifetime = bulletLifetime;
        bulletBehavior.knockback = bulletKnockback;
        bulletBehavior.team = player.tag;

        return;

    }

}

