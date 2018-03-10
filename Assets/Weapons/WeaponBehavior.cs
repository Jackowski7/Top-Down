using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBehavior : MonoBehaviour
{
    public GameObject bulletPrefab;

    public bool sword;
    public bool staff;
    public bool sheild;

    public float fireSpeed;
    public float ChargeTime;
    public float PlayerRotSlow;
    public float EnergyDrainAmount;

    public float baseDamage;

    public float bulletKnockback;
    public float bulletSpeed;
    public float bulletSpread;
    public float bulletDurability;
    public float bulletLifetime;


    [HideInInspector]
    public float bulletDamage;

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
        Vector3 rot = playerBody.transform.forward.normalized;

        GameObject bullet = Instantiate(bulletPrefab);       
        bulletBehavior bulletBehavior = bullet.GetComponent<bulletBehavior>();
        bulletBehavior.damage = bulletDamage;
        bulletBehavior.durability = bulletDurability;
        bulletBehavior.lifetime = bulletLifetime;
        bulletBehavior.knockback = bulletKnockback;

        if (staff == true)
        {
            pos += rot;
            bullet.transform.position = pos;
            bullet.transform.rotation = Quaternion.Euler(rot);

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            Vector3 dir = playerBody.forward.normalized;
            dir.x += Random.Range(-bulletSpread / 100, bulletSpread / 100);
            rb.velocity = dir * bulletSpeed + (playerVelocity);
        }

        if (sword == true)
        {

            bullet.transform.position = pos;
            bullet.transform.rotation = Quaternion.Euler(rot);

        }

    }



}
