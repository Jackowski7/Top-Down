using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBehavior : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject aimLine;

    public float fireSpeed;
    public float chargeSpeed;
    public float PlayerRotSlow;
    public float EnergyDrainAmount;

    public bool kineticDamage;
    public bool fireDamage;
    public bool iceDamage;
    public bool electricDamage;

    public float damage;
    public float knockback;
    public float bulletSpeed;
    public float bulletSpread;
    public float bulletDurability;
    public float bulletLifetime;

    string team;


    [HideInInspector]
    public string damageType;


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
            bullet.transform.rotation = player.rotation;

            Rigidbody rb = bullet.transform.GetComponent<Rigidbody>();
            Vector3 dir = player.forward.normalized;
            dir.y = 0;
            dir.x += Random.Range(-bulletSpread / 100, bulletSpread / 100);

            rb.velocity = dir * bulletSpeed + (playerVelocity);

            float _bulletDamage = damage * Mathf.RoundToInt((Random.Range(.9f, 1.1f)) * 10) * .1f;

            BulletBehavior bulletBehavior = bullet.GetComponent<BulletBehavior>();
            bulletBehavior.bulletSpeed = bulletSpeed;
            bulletBehavior.damage = _bulletDamage;
            bulletBehavior.damageType = damageType;
            bulletBehavior.durability = bulletDurability;
            bulletBehavior.lifetime = bulletLifetime;
            bulletBehavior.knockback = knockback;
            bulletBehavior.team = player.tag;
        }

        if (tag == "Sword")
        { 
            this.GetComponent<Collider>().enabled = true;
            this.GetComponent<ParticleSystem>().Play();
            StartCoroutine(DeactivateSword());

            team = player.tag;

            float _bulletDamage = damage * Mathf.RoundToInt((Random.Range(.9f, 1.1f)) * 10) * .1f;
        }

        return;
    }

    IEnumerator DeactivateSword()
    {
        yield return new WaitForSeconds(.5f);
        this.GetComponent<Collider>().enabled = false;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Enemy" || col.tag == "Player")
        {
            if (col.gameObject.tag != team)
            {
                Stats stats = col.gameObject.GetComponent<Stats>();
                float _damage = damage * Mathf.RoundToInt((Random.Range(.9f, 1.1f)) * 10) * .1f;

                if (stats.shielding == true)
                {

                    Transform shield = col.transform.Find("Equipment").Find("ShieldSlot").GetChild(0).transform;
                    ShieldBehavior shieldBehavior = shield.GetComponent<ShieldBehavior>();

                    bool shieldFacingBullet = ShieldDirection(col.transform.eulerAngles.y, transform.rotation.eulerAngles.y);

                    if (shieldFacingBullet == true)
                    {

                        _damage -= shieldBehavior.baseDamageAbsorb;

                        if (shieldBehavior.damageType == damageType)
                        {
                            _damage *= (shieldBehavior.damageAbsorbPercent * .01f);
                        }
                        else
                        {
                            _damage *= ((shieldBehavior.damageAbsorbPercent / 2) * .01f);
                        }

                        knockback = knockback / 2;
                    }

                }

                stats.Damage(Mathf.Max(0, _damage), damageType, transform.forward, col.gameObject);
                stats.GetComponent<Rigidbody>().AddForce(transform.forward * knockback, ForceMode.VelocityChange);

            }
        }
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

}

