using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletBehavior : MonoBehaviour
{

    Transform player;
    WeaponBehavior weapon;

    [HideInInspector]
    public string target;
    [HideInInspector]
    public string firer;
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
            Destroy(gameObject);
        }

    }


    private void OnCollisionEnter(Collision col)
    {
        //only works for magic
        if (col.transform.tag == target)
        {
            Stats stats = col.gameObject.GetComponent<Stats>();
            stats.health -= damage;
            Debug.Log("Bullet Hit target" + target);
            stats.GetComponent<Rigidbody>().AddForce(transform.forward * knockback, ForceMode.Impulse);
            Destroy(gameObject);
        }

        if (col.gameObject.tag != firer)
        {
            durability--;
        }

        if (col.gameObject.tag == firer)
        {
//            Debug.Log("Stop Hitting yourself!");
        }

        if (col.gameObject.tag == "Shield")
        {
            var _target = target;
            target = firer;
            firer = _target;

            StopCoroutine(DestroySelf());
            StartCoroutine(DestroySelf());
        }

    }

    private void OnTriggerEnter(Collider col)
    {
        // for sword swings
        if (col.tag == target)
        {
            Stats stats = col.gameObject.GetComponent<Stats>();
            stats.health -= damage;
            Debug.Log("Bullet Hit target" + target);
            stats.GetComponent<Rigidbody>().AddForce(transform.forward * knockback, ForceMode.Impulse);
        }

        if (col.tag != firer)
        {
            durability--;
        }

    }

    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }




}
