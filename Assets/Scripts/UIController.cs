using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    GameObject healthBar;
    GameObject energyBar;
    Stats playerStats;

    // Use this for initialization
    void Start () {

        playerStats = GameObject.Find("Player").GetComponent<Stats>();

        healthBar = GameObject.Find("PlayerHealthBar");
        energyBar = GameObject.Find("PlayerEnergyBar");

    }

    // Update is called once per frame
    void Update () {

        float healthPercent = Mathf.Clamp((playerStats.health / playerStats.maxHealth), 0, 1);
        healthBar.GetComponent<RectTransform>().localScale = Vector3.Slerp(healthBar.GetComponent<RectTransform>().localScale, (new Vector3(healthPercent, 1, 1)), 1f * Time.time);

        float energyPercent = Mathf.Clamp((playerStats.energy / playerStats.maxEnergy), 0, 1);
        energyBar.GetComponent<RectTransform>().localScale = Vector3.Slerp(energyBar.GetComponent<RectTransform>().localScale, (new Vector3(energyPercent, 1, 1)), 1f * Time.time);
    }
}
