using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{


    GameManager gameManager;

    public Transform[] equipmentSlots;
    public Transform slot0;
    public Transform slot1;
    public Transform slot2;

    public GameObject[] equipment;
    public GameObject slot0Equipped;
    public GameObject slot1Equipped;
    public GameObject slot2Equipped;

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
            equipment[0] = slot0Equipped;
        }
        if (equipment[1] == null)
        {
            equipment[1] = slot1Equipped;
        }
        if (equipment[2] == null)
        {
            equipment[2] = slot2Equipped;
        }
    }


    void SetUpSlots()
    {
        equipmentSlots = new Transform[3];

        // 0 = weapon1, 1 = weapon 2, 2 = weapon 3, 3 = helmet;
        equipmentSlots[0] = slot0;
        equipmentSlots[1] = slot1;
        equipmentSlots[2] = slot2;
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
