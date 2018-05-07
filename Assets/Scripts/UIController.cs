using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    GameObject healthBar;
    GameObject energyBar;
    GameObject dashEnergyBar;
    Stats playerStats;
    GameObject damageOverlay;

    GameObject crossFade;
    float crossFadeOpacity;

    public GameObject menu;

    // Use this for initialization
    void Start()
    {

        playerStats = GameObject.Find("Player").GetComponent<Stats>();

        healthBar = GameObject.Find("PlayerHealthBar");
        energyBar = GameObject.Find("PlayerEnergyBar");
        dashEnergyBar = GameObject.Find("PlayerDashBar");

        damageOverlay = GameObject.Find("DamageOverlay");
        crossFade = GameObject.Find("CrossFade");

    }

    // Update is called once per frame
    void Update () {

        float healthPercent = Mathf.Clamp((playerStats.health / playerStats.maxHealth), 0, 1);
        healthBar.GetComponent<RectTransform>().localScale = Vector3.Slerp(healthBar.GetComponent<RectTransform>().localScale, (new Vector3(1, healthPercent, 1)), 1f * Time.time);

        float healthPercentGone = 1 - healthPercent;
        float damageOverlayOpacity = healthPercentGone;
        damageOverlay.GetComponent<RawImage>().color = new Color(255,0,0, damageOverlayOpacity);

        crossFade.GetComponent<RawImage>().color = new Color(0, 0, 0, crossFadeOpacity);

        float energyPercent = Mathf.Clamp((playerStats.energy / playerStats.maxEnergy), 0, 1);
        energyBar.GetComponent<RectTransform>().localScale = Vector3.Slerp(energyBar.GetComponent<RectTransform>().localScale, (new Vector3(1, energyPercent, 1)), 1f * Time.time);

        float dashEnergyPercent = Mathf.Clamp((playerStats.dashEnergy / playerStats.maxDashEnergy), 0, 1);
        dashEnergyBar.GetComponent<RectTransform>().localScale = Vector3.Slerp(dashEnergyBar.GetComponent<RectTransform>().localScale, (new Vector3(dashEnergyPercent, 1, 1)), 1f * Time.time);

    }

    public void OpenMenu()
    {
        menu.SetActive(true);
        Time.timeScale = 0;

    }

    public void CloseMenu()
    {
        menu.SetActive(false);
        Time.timeScale = 1;

    }

    public void ToggleMenu()
    {
        if (menu.activeSelf == true)
        {
            menu.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            menu.SetActive(true);
            Time.timeScale = 0;

        }
    }


    public IEnumerator CrossFade()
    {
        crossFadeOpacity = 0;
        while (crossFadeOpacity < 1)
        {
            crossFadeOpacity += Time.smoothDeltaTime * 2f;
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(.25f);

        while (crossFadeOpacity > 0)
        {
            crossFadeOpacity -= Time.smoothDeltaTime * 2f;
            yield return new WaitForEndOfFrame();
        }
        crossFadeOpacity = 0;
    }




}
