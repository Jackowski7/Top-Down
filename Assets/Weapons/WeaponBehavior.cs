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
		Vector3 _rot = playerBody.transform.forward;
		Quaternion rot = playerBody.transform.rotation;

		pos += _rot;


		GameObject bullet = Instantiate(bulletPrefab, pos, rot);       
        bulletBehavior bulletBehavior = bullet.GetComponent<bulletBehavior>();
		bulletBehavior.damage = baseDamage;
        bulletBehavior.durability = bulletDurability;
        bulletBehavior.lifetime = bulletLifetime;
        bulletBehavior.knockback = bulletKnockback;

        if (staff == true)
		{
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            Vector3 dir = playerBody.forward.normalized;
            dir.x += Random.Range(-bulletSpread / 100, bulletSpread / 100);
            rb.velocity = dir * bulletSpeed + (playerVelocity);
        }

        if (sword == true)
        {

		}

		if (shield == true)
		{


		}

		return;

    }



}
