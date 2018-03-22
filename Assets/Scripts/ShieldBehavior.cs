using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBehavior : MonoBehaviour {

    public float fireSpeed;
    public float ChargeTime;
    public float PlayerRotSlow;
    public float EnergyDrainAmount;
    public float knockBack;
    public float kineticReduction;
    public float reduction1;
    public float reduction2;
    public float reduction3;

    string target = null;
    string firer = null;

    void OnValidate()
    {
        fireSpeed = Mathf.Max(fireSpeed, .5f);
        ChargeTime = Mathf.Max(ChargeTime, .2f);
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == target)
        {
            Debug.Log("hit the shield");
            Vector3 dir = col.transform.position - transform.position;
            Rigidbody rb = col.gameObject.GetComponent<Rigidbody>();
            rb.AddForce(dir.normalized * knockBack, ForceMode.Impulse);
        }
    }


    public Vector4 ShieldInfo()
    {
        Vector4 weaponInfo = new Vector4(0, 0, 0, 0);
        weaponInfo.x = fireSpeed;
        weaponInfo.y = ChargeTime;
        weaponInfo.z = PlayerRotSlow;
        weaponInfo.w = EnergyDrainAmount;
        return weaponInfo;
    }

    public void DrawShield(Transform playerBody)
    {
        string user = playerBody.transform.parent.tag;

        if (user == "Enemy")
        {
            target = "Player";
            firer = "Enemy";
        }

        if (user == "Player")
        {
            target = "Enemy";
            firer = "Player";
        }

        Collider col = GetComponent<Collider>();
        col.enabled = true;
    }

    public void PutAway()
    {
        Collider col = GetComponent<Collider>();
        col.enabled = false;
    }




}

