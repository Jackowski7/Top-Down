using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{

    public GameObject explosionPrefab;

    Transform player;
    WeaponBehavior weapon;

    [HideInInspector]
    public string team;
    [HideInInspector]
    public string damageType;
    [HideInInspector]
    public float durability;
    [HideInInspector]
    public bool invincible;
    [HideInInspector]
    public float damage;
    [HideInInspector]
    public float lifetime;
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
        if (durability <= 0 && invincible == false)
        {
            Explode();
        }

    }

    private void OnTriggerEnter(Collider col)
    {

        if (col.tag == "Enemy" || col.tag == "Player")
        {
            if (col.gameObject.tag != team)
            {
                Stats stats = col.gameObject.GetComponent<Stats>();

                if (col.gameObject.tag == "Shield")
                {
                    ShieldBehavior shield = col.gameObject.GetComponent<ShieldBehavior>();
                    if (shield.damageType == damageType)
                    {
                        damage -= shield.damageAbsorb;
                    }
                    else
                    {
                        damage -= (shield.damageAbsorb / 2);
                    }

                    knockback = knockback / 4;
                }

                stats.Damage(Mathf.Max(0, damage), damageType, transform.forward, col.gameObject);
                stats.GetComponent<Rigidbody>().AddForce(transform.forward * knockback, ForceMode.Impulse);
                durability--;

            }
        }

        if (col.tag != "Enemy" && col.tag != "Player" && col.tag != "Destructible" && col.tag != "Effect")
        {
            durability--;
        }

    }

    void Explode()
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }

    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(lifetime);
        Explode();
    }




}
