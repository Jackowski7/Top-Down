using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletBehavior : MonoBehaviour
{

    Transform player;
    WeaponBehavior weapon;

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

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Enemy")
        {
            EnemyBehavior enemy = col.GetComponent<EnemyBehavior>();
            enemy.health -= damage;
			Debug.Log ("I got one!");

            enemy.GetComponent<Rigidbody>().AddForce(GetComponent<Rigidbody>().velocity.normalized * knockback, ForceMode.Impulse);
        }

        if (col.tag != "Player")
        {
            durability--;
        }

        if (col.tag == "Player")
        {
            Debug.Log("Stop Hitting yourself!");
        }

    }

    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }




}
