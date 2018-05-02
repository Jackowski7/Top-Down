using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    GameObject healthBar;
    GameObject energyBar;
    Stats playerStats;
    GameObject damageScreen;

    // Use this for initialization
    void Start () {

        playerStats = GameObject.Find("Player").GetComponent<Stats>();

        healthBar = GameObject.Find("PlayerHealthBar");
        energyBar = GameObject.Find("PlayerEnergyBar");

        damageScreen = GameObject.Find("CameraScreen");


    }

    // Update is called once per frame
    void Update () {

        float healthPercent = Mathf.Clamp((playerStats.health / playerStats.maxHealth), 0, 1);
        healthBar.GetComponent<RectTransform>().localScale = Vector3.Slerp(healthBar.GetComponent<RectTransform>().localScale, (new Vector3(1, healthPercent, 1)), 1f * Time.time);

        float healthPercentGone = 1 - healthPercent;
        float damageScreenOpacity = healthPercentGone;
        damageScreen.GetComponent<RawImage>().color = new Color(255,0,0, damageScreenOpacity);

        float energyPercent = Mathf.Clamp((playerStats.energy / playerStats.maxEnergy), 0, 1);
        energyBar.GetComponent<RectTransform>().localScale = Vector3.Slerp(energyBar.GetComponent<RectTransform>().localScale, (new Vector3(1, energyPercent, 1)), 1f * Time.time);
    }
}
