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
    Transform bg;
    Renderer bgRenderer;
    
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
            transform.position = Vector3.Slerp(transform.position, entity.transform.position, 3f * Time.deltaTime);
        }

        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, .75f * Time.deltaTime);
    }


    public void SetHitMarker(float damageAmount, string damageType, GameObject _entity)
    {
        entity = _entity;

        bg = transform.GetChild(0).transform;
        bgRenderer = bg.GetComponent<Renderer>();
        hitText = transform.GetChild(1).transform.GetComponent<UnityEngine.UI.Text>();

        hitText.text = damageAmount.ToString();

        if (damageType == "Kinetic")
        {
            //hitText.color = kineticDamageColor;
        }
        else if (damageType == "Fire")
        {
            //hitText.color = fireDamageColor;
        }
        else if (damageType == "Ice")
        {
            //hitText.color = iceDamageColor;
        }
        else if (damageType == "Electric")
        {
            //hitText.color = electricDamageColor;
        }


        int length = (int)Mathf.Floor(Mathf.Log10(damageAmount) + 1);
        bg.localScale = new Vector3(length * .3f, bg.localScale.y, bg.localScale.z);

        if (_entity.tag == "Player")
        {
            bgRenderer.material = damageToPlayer;
        }
        else
        {
            bgRenderer.material = damageToEnemy;
        }

    }


    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
