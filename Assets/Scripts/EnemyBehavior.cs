using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{

    GameObject player;
    Stats stats;
    Equipment equipment;

    GameObject healthBar;
    GameObject energyBar;

    public float seeDistance;

    int lookingMask;

    [HideInInspector]
    public bool dead;
    [HideInInspector]
    public bool invincible;


    Rigidbody rb;
    Vector3 moveDirection;
    Vector3 _prevPosition;
    Vector3 playerVelocity;

    private Animator playerAnimator;
    private string Idle_Animation = "Idle";
    private string Casting_Animation = "Casting";
    private string Meleeing_Animation = "Meleeing";
    private string Shielding_Animation = "Shielding";
    private float speed_Animation;

    private bool firing;
    private bool charged = false;
    private int activeWeapon = 0;
    private int firingWeapon;
    private bool applyThorns;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        stats = GetComponent<Stats>();
        equipment = GetComponent<Equipment>();
        playerAnimator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        healthBar = transform.Find("Stats/HealthPanel").GetChild(0).gameObject;
        energyBar = transform.Find("Stats/EnergyPanel").GetChild(0).gameObject;

        lookingMask = LayerMask.GetMask("Player", "Environment");
    }

    void Update()
    {

        float healthPercent = Mathf.Clamp((stats.health / stats.maxHealth), 0, 1);
        healthBar.GetComponent<RectTransform>().localScale = Vector3.Slerp(healthBar.GetComponent<RectTransform>().localScale, (new Vector3(healthPercent, 1, 1)), 1f * Time.time);

        float energyPercent = Mathf.Clamp((stats.energy / stats.maxEnergy), 0, 1);
        energyBar.GetComponent<RectTransform>().localScale = Vector3.Slerp(energyBar.GetComponent<RectTransform>().localScale, (new Vector3(energyPercent, 1, 1)), 1f * Time.time);

        if (stats.health <= 0)
        {
            Destroy(this.gameObject);
            Debug.Log("Enemy Killed");
        }

        playerVelocity = ((transform.position - _prevPosition) / Time.fixedDeltaTime).normalized * 1.25f;
        playerVelocity.y = 0;
        speed_Animation = Vector3.Distance(transform.position, _prevPosition);
        _prevPosition = transform.position;

        playerAnimator.SetFloat("MovementSpeed", speed_Animation);

        Vector3 playerDirection = (player.transform.position - transform.position);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDirection, out hit, seeDistance, lookingMask) && (hit.transform.tag == "Player" || hit.transform.tag == "Shield"))
        {

            playerDirection.y = 0;
            Quaternion rotation = Quaternion.LookRotation(playerDirection);
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * stats.rotationSpeed));

            moveDirection = playerDirection.normalized;
            moveDirection *= stats.movementSpeed * 200;

            if (hit.distance > 3f && firing == false)
            {
                StartCoroutine(_FireWeapon(0));
                firingWeapon = 0;
            }

            if (hit.distance <= 5f && firing == false)
            {
                StartCoroutine(_FireWeapon(1));
                firingWeapon = 1;
            }


            if (hit.distance > 3f && firing == true && firingWeapon == 1)
            {
                StartCoroutine(_FireWeapon(0));
                firingWeapon = 0;
            }

            if (hit.distance <= 5f && firing == true && firingWeapon == 0)
            {
                StartCoroutine(_FireWeapon(1));
                firingWeapon = 1;
            }


            if (hit.distance > .5f)
            {
                rb.AddForce(moveDirection * Time.deltaTime, ForceMode.Force);
            }


        }
        else
        {
            //not firing a weapon
            firingWeapon = 3;
        }
    }

    IEnumerator _FireWeapon(int weaponNumber)
    {
        //we're firing, dont fire again until we're done
        firing = true;
        Transform weapon = transform.Find("Equipment").Find("WeaponSlot").GetChild(weaponNumber).transform.GetChild(0);
        string weaponType = weapon.tag;

        WeaponBehavior weaponBehavior = weapon.GetComponent<WeaponBehavior>();
        Vector4 weaponInfo = weaponBehavior.WeaponInfo();

        float _rotationSpeed = stats.rotationSpeed;
        float fireSpeed = weaponInfo.x;
        float chargeSpeed = weaponInfo.y;
        float playerRotSlow = weaponInfo.z;
        float energyDrainAmount = weaponInfo.w;

        Animator weaponAnimator = null;
        if (weapon.GetComponent<Animator>())
        {
            weaponAnimator = weapon.GetComponent<Animator>();
        }

        //swap weapons on fire
        if (activeWeapon != weaponNumber)
        {
            GameObject oldWeaponSlot = equipment.equipmentSlots[activeWeapon].gameObject;
            oldWeaponSlot.SetActive(false);

            GameObject weaponSlot = equipment.equipmentSlots[weaponNumber].gameObject;
            weaponSlot.SetActive(true);

            activeWeapon = weaponNumber;
        }

        playerAnimator.SetFloat("FireSpeed", fireSpeed);
        playerAnimator.SetFloat("ChargeSpeed", chargeSpeed);

        if (weaponType == "Staff")
        {
            Animate(Casting_Animation);
        }
        if (weaponType == "Sword" || weaponType == "Dagger") // or any melee type..
        {
            Animate(Meleeing_Animation); // animate appropriate weapon type animation
        }

        if (weaponAnimator != null)
        {
            weaponAnimator.SetBool("Firing", true);
            weaponAnimator.SetFloat("ChargeSpeed", chargeSpeed);
            weaponAnimator.SetFloat("FireSpeed", fireSpeed);
        }

        //slow player rotation by weapon amount
        stats.rotationSpeed = stats.rotationSpeed - (stats.rotationSpeed * playerRotSlow / 100);

        float lastFireTime = 0;
        while (firingWeapon == weaponNumber && stats.energy - energyDrainAmount >= 0)
        {

            if (Time.time >= lastFireTime + (1 / fireSpeed) && charged == true)
            {
                lastFireTime = Time.time;
                playerAnimator.SetTrigger("Fire");

                if (weaponAnimator != null)
                {
                    weaponAnimator.SetTrigger("Fire");
                }

            }
            yield return new WaitForFixedUpdate();

        }

        //set rotation speed to normal
        stats.rotationSpeed = _rotationSpeed;
        Animate(Idle_Animation);
        playerAnimator.SetFloat("FireSpeed", 1);

        if (weaponAnimator != null)
        {
            weaponAnimator.SetFloat("FireSpeed", 1);
            weaponAnimator.SetBool("Firing", false);
        }

        firing = false;
        charged = false;

    }

    IEnumerator _FireShield(int weaponNumber, string button)
    {
        //we're firing, dont fire again until we're done
        firing = true;
        Transform shield = transform.Find("Equipment").Find("ShieldSlot").GetChild(0).transform;
        string shieldType = shield.tag;

        ShieldBehavior shieldBehavior = shield.GetComponent<ShieldBehavior>();

        float _rotationSpeed = stats.rotationSpeed;

        Vector4 weaponInfo = shieldBehavior.ShieldInfo();

        float fireSpeed = weaponInfo.x;
        float chargeSpeed = weaponInfo.y;
        float playerRotSlow = weaponInfo.z;
        float energyDrainAmount = weaponInfo.w;

        Animate(Shielding_Animation);
        stats.rotationSpeed = stats.rotationSpeed - (stats.rotationSpeed * playerRotSlow / 100);

        playerAnimator.SetFloat("FireSpeed", fireSpeed);
        playerAnimator.SetFloat("ChargeSpeed", chargeSpeed);

        float lastFireTime = 0;
        while ((Input.GetButton(button) && stats.energy - energyDrainAmount >= 0))
        {

            if (Time.time >= lastFireTime + (1 / fireSpeed) && charged == true)
            {
                playerAnimator.SetTrigger("Fire");
                lastFireTime = Time.time;
            }
            yield return new WaitForFixedUpdate();

        }

        //set rotation speed to normal
        stats.rotationSpeed = _rotationSpeed;
        Animate(Idle_Animation);
        firing = false;

    }

    public void SetCharged()
    {
        charged = true;
    }

    void FireWeapon()
    {
        WeaponBehavior weapon = transform.Find("Equipment").Find("WeaponSlot").GetChild(activeWeapon).GetChild(0).GetComponent<WeaponBehavior>();
        stats.energy -= weapon.EnergyDrainAmount;
        stats.lastEnergyUsedTime = Time.time;
        weapon.FireWeapon(transform, playerVelocity);
    }

    void DeactivateSword()
    {
        WeaponBehavior weapon = transform.Find("Equipment").Find("WeaponSlot").GetChild(activeWeapon).GetChild(0).GetComponent<WeaponBehavior>();
        if (weapon.GetComponent<Collider>())
        {
            weapon.GetComponent<Collider>().enabled = false;
        }
    }

    void DrawShield()
    {
        ShieldBehavior shield = transform.Find("Equipment").Find("ShieldSlot").GetChild(0).GetComponent<ShieldBehavior>();
        shield.DrawShield(transform);
        shield.GetComponent<Collider>().enabled = true;

    }

    void FireShield()
    {
        ShieldBehavior shield = transform.Find("Equipment").Find("ShieldSlot").GetChild(0).GetComponent<ShieldBehavior>();
        stats.energy -= shield.EnergyDrainAmount;
        stats.lastEnergyUsedTime = Time.time;
        shield.FireShield();
    }

    void PutAwayShield()
    {
        ShieldBehavior shield = transform.Find("Equipment").Find("ShieldSlot").GetChild(0).GetComponent<ShieldBehavior>();
        shield.PutAway();
        shield.GetComponent<Collider>().enabled = false;
    }

    private void Animate(string boolName)
    {
        DisableOtherAnimations(playerAnimator, boolName);
        playerAnimator.SetBool(boolName, true);
    }

    private void DisableOtherAnimations(Animator animator, string animation)
    {
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.name != animation && parameter.name != "MovementSpeed" && parameter.name != "FireSpeed" && parameter.name != "ChargeSpeed")
            {
                animator.SetBool(parameter.name, false);
            }
        }

    }

    bool ShieldDirection(float shieldDir, float bulletDir)
    {
        float a = Mathf.Max(shieldDir, bulletDir);
        float b = Mathf.Min(shieldDir, bulletDir);

        if (a - b > 140 && a - b < 220)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
