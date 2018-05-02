using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HitMarker : MonoBehaviour
{

    public Material damageToPlayer;
    public Material damageToEnemy;

    private GameObject entity;

    Text hitText;
    Text hitTextBg;
    
    // Use this for initialization
    void Start()
    {
        StartCoroutine(DestroySelf());
    }

    // Update is called once per frame
    void Update()
    {
        if (entity != null)
        {
            Vector3 newPos = entity.transform.position;
            newPos.y -= .4f;
            newPos.z += .5f;
            transform.position = Vector3.Slerp(transform.position, newPos, 4f * Time.deltaTime);
        }

        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, .75f * Time.deltaTime);
    }


    public void SetHitMarker(float damageAmount, string damageType, GameObject _entity)
    {
        entity = _entity;

        hitText = transform.GetChild(0).transform.GetComponent<UnityEngine.UI.Text>();
        hitTextBg = transform.GetChild(1).transform.GetComponent<UnityEngine.UI.Text>();


        hitText.text = damageAmount.ToString();
        hitTextBg.text = damageAmount.ToString();

        if (damageType == "Fire")
        {
            transform.Find("FireEffect").gameObject.SetActive(true);
        }
        else if (damageType == "Ice")
        {
            transform.Find("IceEffect").gameObject.SetActive(true);
        }
        else if (damageType == "Electric")
        {
            transform.Find("ElectricEffect").gameObject.SetActive(true);
        }

        if (_entity.tag == "Player")
        {
            hitText.color = Color.red;
        }
        else
        {
            hitText.color = Color.white;
        }

    }


    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }
}
