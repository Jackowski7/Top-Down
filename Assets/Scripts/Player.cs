using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{


    GameManager gameManager;

    public Transform[] equipmentSlots;
    public Transform WeaponSlot1;
    public Transform WeaponSlot2;
    public Transform ShieldSlot;
    public Transform HelmetSlot;

    public GameObject[] equipment;
    public GameObject Weapon1;
    public GameObject Weapon2;
    public GameObject Shield;
    public GameObject Helmet;

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

    void SetUpEquipment()
    {
        equipment = new GameObject[4];

        // 0 = weapon1, 1 = weapon 2, 2 = weapon 3, 3 = helmet;

        if (equipment[0] == null)
        {
            equipment[0] = Weapon1;
        }
        if (equipment[1] == null)
        {
            equipment[1] = Weapon2;
        }
        if (equipment[2] == null)
        {
            equipment[2] = Shield;
        }
        if (equipment[3] == null)
        {
            equipment[3] = Helmet;
        }
    }


    void SetUpSlots()
    {
        equipmentSlots = new Transform[4];

        // 0 = weapon1, 1 = weapon 2, 2 = weapon 3, 3 = helmet;
        equipmentSlots[0] = WeaponSlot1;
        equipmentSlots[1] = WeaponSlot2;
        equipmentSlots[2] = ShieldSlot;
        equipmentSlots[3] = HelmetSlot;
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
