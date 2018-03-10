using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour {

    public float health;
    public float thornsDamage;

    bool applyThorns;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (health <= 0){
            Destroy(gameObject);
        }
		
	}

    public void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            if (applyThorns == false)
            {
                StartCoroutine(ThornsDamage(col.gameObject));
            }
        }
    }

    public void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            applyThorns = false;
        }
    }

    IEnumerator ThornsDamage(GameObject damagee)
    {
        Player player = damagee.GetComponent<Player>();
        applyThorns = true;

        while (applyThorns == true && player.invincible != true  && player.dead != true) {
            player.health -= thornsDamage;
            Debug.Log(thornsDamage + " thorns damage applied" + damagee.name);
            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForSeconds(1f);
        applyThorns = false;
    }


}
