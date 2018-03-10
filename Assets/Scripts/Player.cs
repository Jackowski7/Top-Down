using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{


    GameManager gameManager;

    public float health;
    public float energy;

    public Transform[] equipmentSlots;
    public Transform weaponSlot1;
    public Transform weaponSlot2;
    public Transform weaponSlot3;

    public GameObject[] equipment;
    public GameObject startWeapon1;
    public GameObject startWeapon2;
    public GameObject startWeapon3;

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

    void Update()
    {
        if (health <= 0)
        {
            Debug.Log("oh dear, you have died!");
        }
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
            equipment[0] = startWeapon1;
        }
        if (equipment[1] == null)
        {
            equipment[1] = startWeapon2;
        }
        if (equipment[2] == null)
        {
            equipment[2] = startWeapon3;
        }
    }


    void SetUpSlots()
    {
        equipmentSlots = new Transform[3];

        // 0 = weapon1, 1 = weapon 2, 2 = weapon 3, 3 = helmet;
        equipmentSlots[0] = weaponSlot1;
        equipmentSlots[1] = weaponSlot2;
        equipmentSlots[2] = weaponSlot3;
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
