using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{

    Player player;
    Stats playerStats;
    Stats stats;

    public float thornsDamage;
    public float thornsKnockback;
    public float seeDistance;
    public float speed;
    public float rotationSpeed;

    int lookingMask;

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


    Rigidbody rb;
    Vector3 moveDirection;
    Vector3 _prevPosition;
    Transform EnemyBody;

    bool applyThorns;
    bool firing;
    int activeWeapon = 0;
    int firingWeapon = 3;

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<Stats>();
        stats = GetComponent<Stats>();
        rb = GetComponent<Rigidbody>();
        EnemyBody= transform.Find("EnemyBody");

        lookingMask = LayerMask.GetMask("Player", "Environment");


        SetUpEquipment();
        SetUpSlots();
        EquipGear();
    }

    // Update is called once per frame
    void Update()
    {

        if (stats.health <= 0)
        {
            Destroy(this.gameObject);
            Debug.Log("Enemy Killed");
        }

        Vector3 playerDirection = (player.transform.position - transform.position);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDirection, out hit, seeDistance, lookingMask) && (hit.transform.tag == "Player" || hit.transform.tag == "Shield" ))
        {

            playerDirection.y = 0;
            Quaternion rotation = Quaternion.LookRotation(playerDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);

            moveDirection = playerDirection.normalized;
            moveDirection *= speed * 100;

            if (hit.distance > 3f && firing == false)
            {
                StartCoroutine(_FireWeapon(0, "Fire1"));
                firingWeapon = 0;
            }

            if (hit.distance <= 3f && firing == false)
            {
                StartCoroutine(_FireWeapon(1, "Fire2"));
                firingWeapon = 1;
            }


            if (hit.distance > .5f)
            {
                rb.AddForce(moveDirection * Time.deltaTime, ForceMode.Acceleration);
            }
            

        }
        else
        {
            //not firing a weapon
            firingWeapon = 3;
        }
    }

    IEnumerator _FireWeapon(int weaponNumber, string button)
    {
        //we're firing, dont fire again until we're done
        firing = true;

        //swap weapons on fire
        if (activeWeapon != weaponNumber && weaponNumber < 2)
        {
            GameObject oldWeaponSlot = equipmentSlots[activeWeapon].gameObject;
            oldWeaponSlot.SetActive(false);

            GameObject weaponSlot = equipmentSlots[weaponNumber].gameObject;
            weaponSlot.SetActive(true);
            activeWeapon = weaponNumber;
        }

        WeaponBehavior weapon = equipment[weaponNumber].GetComponent<WeaponBehavior>();

        float _rotationSpeed = rotationSpeed;

        Vector4 weaponInfo = weapon.WeaponInfo();

        float fireSpeed = weaponInfo.x;
        float chargeTime = weaponInfo.y;
        float playerRotSlow = weaponInfo.z;
        float energyDrainAmount = weaponInfo.w;

        yield return new WaitForSeconds(chargeTime);

        rotationSpeed = rotationSpeed - (rotationSpeed * playerRotSlow / 100);


        while (firingWeapon == weaponNumber && stats.energy - energyDrainAmount >= 0)
        {

            Vector3 velocity;

            velocity = ((transform.position - _prevPosition) / Time.fixedDeltaTime).normalized * 1.25f;
            _prevPosition = transform.position;

            weapon.FireWeapon(EnemyBody, velocity);
            stats.energy -= energyDrainAmount;

            yield return new WaitForSeconds(fireSpeed);
        }

        rotationSpeed = _rotationSpeed;
        firing = false;

    }



    public void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            if (applyThorns == false)
            {
                applyThorns = true;
                StartCoroutine(ThornsDamage(col.gameObject));
            }
        }
    }

    public void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            applyThorns = false;
        }
    }

    IEnumerator ThornsDamage(GameObject damagee)
    {
        while (applyThorns == true && player.invincible != true && player.dead != true)
        {
            playerStats.health -= thornsDamage;
            Debug.Log(thornsDamage + " thorns damage applied" + damagee.name);
            Rigidbody rb = damagee.GetComponent<Rigidbody>();
            Vector3 dir = (transform.position - damagee.transform.position).normalized;
            dir.y = 0;
            rb.AddForce(-dir * thornsKnockback, ForceMode.Impulse);
            yield return new WaitForSeconds(1f);
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
