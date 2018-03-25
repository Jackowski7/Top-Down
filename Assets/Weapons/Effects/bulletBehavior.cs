using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletBehavior : MonoBehaviour
{

    public GameObject explosionPrefab;

    Transform player;
    WeaponBehavior weapon;

    [HideInInspector]
    public string team;
    [HideInInspector]
    public float durability;
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
        if (durability <= 0)
        {
            StartCoroutine(Explode());
        }

    }

    private void OnTriggerEnter(Collider col)
    {

        if (col.tag == "Enemy" || col.tag == "Player" || col.tag == "Destructible")
        {
            if (col.gameObject.tag != team)
            {
                Stats stats = col.gameObject.GetComponent<Stats>();
                stats.health -= damage;
                //Debug.Log("Bullet Hit " + col.transform.name);
                stats.GetComponent<Rigidbody>().AddForce(transform.forward * knockback, ForceMode.Impulse);
                durability--;
            }
        }
        if (col.tag != "Enemy" && col.tag != "Player" && col.tag != "Destructible" && col.tag != "Effect")
        {
            durability--;
        }

    }

    IEnumerator Explode()
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, transform.rotation);
            yield return new WaitForFixedUpdate();
        }
        Destroy(gameObject);
    }

    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(lifetime);
        StartCoroutine(Explode());
    }




}
