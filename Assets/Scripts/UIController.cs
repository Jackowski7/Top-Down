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

        healthBar = GameObject.Find("HealthBar");
        energyBar = GameObject.Find("EnergyBar");
        playerStats = GameObject.Find("Player").GetComponent<Stats>();

    }
	
	// Update is called once per frame
	void Update () {

        float healthPercent = Mathf.Max((playerStats.health / playerStats.maxHealth), 0);
        healthBar.GetComponent<RectTransform>().localScale = (new Vector3(healthPercent, 1, 1));

        float manaPercent = Mathf.Max((playerStats.energy / playerStats.maxEnergy), 0);
        energyBar.GetComponent<RectTransform>().localScale = (new Vector3(manaPercent, 1, 1));

    }
}
