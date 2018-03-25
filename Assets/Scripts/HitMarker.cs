using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HitMarker : MonoBehaviour {

    public string damageType;

    public Material textRed;
    public Material textOrange;

    private GameObject entity;

	// Use this for initialization
	void Start () {
        StartCoroutine(DestroySelf());
	}
	
	// Update is called once per frame
	void Update () {
		if (entity != null)
        {
            transform.position = Vector3.Slerp(transform.position, entity.transform.position, 1f * Time.deltaTime);
        }
    }

    public void SetHitMarker(float damageAmount, string damageType, GameObject _entity)
    {
        entity = _entity;
        this.GetComponent<UnityEngine.UI.Text>().text = damageAmount.ToString();

        if (_entity.tag == "Player")
        {
            this.GetComponent<UnityEngine.UI.Text>().material = textRed;
        }
        else
        {
            this.GetComponent<UnityEngine.UI.Text>().material = textOrange;
        }

    }


    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
