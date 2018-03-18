using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{


    GameManager gameManager;

    public Transform[] equipmentSlots;
    public Transform magicWeaponSlot;
    public Transform meleeWeaponSlot;
    public Transform shieldSlot;

    public GameObject[] equipment;
    public GameObject magicWeapon;
    public GameObject meleeWeapon;
    public GameObject shield;

    [HideInInspector]
    public bool dead;
    [HideInInspector]
    public bool invincible;

    private void Start()
    {
        gameManager = transform.parent.GetComponent<GameManager>();
        SetUpEquipment();
        SetUpSlots();
        EquipGear();
    }

    public void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Exit")
        {
            gameManager.LevelFinished();
            Debug.Log("level Finished");
        }
    }

    void SetUpEquipment()
    {
        equipment = new GameObject[3];

        // 0 = weapon1, 1 = weapon 2, 2 = weapon 3, 3 = helmet;

        if (equipment[0] == null)
        {
            equipment[0] = magicWeapon;
        }
        if (equipment[1] == null)
        {
            equipment[1] = meleeWeapon;
        }
        if (equipment[2] == null)
        {
            equipment[2] = shield;
        }
    }


    void SetUpSlots()
    {
        equipmentSlots = new Transform[3];

        // 0 = weapon1, 1 = weapon 2, 2 = weapon 3, 3 = helmet;
        equipmentSlots[0] = magicWeaponSlot;
        equipmentSlots[1] = meleeWeaponSlot;
        equipmentSlots[2] = shieldSlot;
}

    void EquipGear()
    {

        for (int x = 0; x < equipmentSlots.Length; x++)
        {
            Transform slot = equipmentSlots[x];
            GameObject item = Instantiate(equipment[x], slot.position, slot.rotation);
            item.transform.parent = slot.transform;
            item.name = equipment[x].name;
        }
  
    }

}
