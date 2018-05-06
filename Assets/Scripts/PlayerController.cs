using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    GameManager gameManager;
    UIController UIController;
    Equipment equipment;
    Stats stats;
    Rigidbody rb;
    ControllerDetector controllerDetector;

    public GameObject myCamera;
    public GameObject MapCamera;
    public float cameraSpeed;
    public float cameraHeight;

    Vector3 moveDirection;
    Quaternion LookDirection;
    Vector3 _prevPosition;
    Camera mainCamera;
    Vector3 playerVelocity;

    private Animator playerAnimator;
    private string Idle_Animation = "Idle";
    private string Casting_Animation = "Casting";
    private string Meleeing_Animation = "Meleeing";
    private string Shielding_Animation = "Shielding";
    private float speed_Animation;

    public static bool firing;
    public static bool charged = false;
    public static int activeWeapon = 0;
    public static bool dashAvailable = true;

    GameObject aimLine;
    bool aimLineOn = false;
    float aimLineOpacity;


    private void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        UIController = gameManager.GetComponent<UIController>();
        controllerDetector = gameManager.GetComponent<ControllerDetector>();
        equipment = transform.GetComponent<Equipment>();
        playerAnimator = GetComponent<Animator>();
        stats = GetComponent<Stats>();
        rb = GetComponent<Rigidbody>();
        aimLine = transform.Find("AimLine").gameObject;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Menu"))
        {
            UIController.ToggleMenu();
        }
    }

    void FixedUpdate()
    {
        if (UIController.menu.activeSelf == false)
        {

            if (Input.GetButton("Fire1") && firing == false)
            {
                StartCoroutine(_FireWeapon(0, "Fire1"));
            }
            if (Input.GetButton("Fire2") && firing == false)
            {
                StartCoroutine(_FireWeapon(1, "Fire2"));
            }
            if (Input.GetButton("Shield") && firing == false)
            {
                StartCoroutine(_FireShield(2, "Shield"));
            }
            if (Input.GetButton("Dash") && dashAvailable == true)
            {
                Dash();
            }

#if UNITY_STANDALONE

            if (controllerDetector.Xbox_One_Controller == false && controllerDetector.PS4_Controller == false)
            {
                // rotate player body towards mouse
                Plane playerPlane = new Plane(Vector3.up, transform.position);

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                float hitdist = 0.0f;
                if (playerPlane.Raycast(ray, out hitdist))
                {
                    Vector3 targetPoint = ray.GetPoint(hitdist);
                    Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
                    targetRotation.x = 0;
                    targetRotation.z = 0;
                    rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, stats.rotationSpeed * Time.deltaTime));
                }
            }
            else
            {
                if (Input.GetAxis("AimVertical") > .1f || Input.GetAxis("AimHorizontal") > .1f || Input.GetAxis("AimVertical") < -.1f || Input.GetAxis("AimHorizontal") < -.1f)
                {
                    Vector3 _LookDirection = new Vector3(0, Mathf.Atan2(Input.GetAxis("AimVertical"), Input.GetAxis("AimHorizontal")) * 180 / Mathf.PI, 0);
                    LookDirection = Quaternion.Euler(0, _LookDirection.y + 90, 0);
                    rb.MoveRotation(Quaternion.Slerp(transform.rotation, LookDirection, stats.rotationSpeed * Time.deltaTime));
                }
                else if (Input.GetAxis("Vertical") > .1f || Input.GetAxis("Horizontal") > .1f || Input.GetAxis("Vertical") < -.1f || Input.GetAxis("Horizontal") < -.1f)
                {
                    Vector3 _LookDirection = new Vector3(0, Mathf.Atan2(Input.GetAxis("Vertical") * -1, Input.GetAxis("Horizontal")) * 180 / Mathf.PI, 0);
                    LookDirection = Quaternion.Euler(0, _LookDirection.y + 90, 0);
                    rb.MoveRotation(Quaternion.Slerp(transform.rotation, LookDirection, stats.rotationSpeed * Time.deltaTime));
                }
            }

#endif

            //move player
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection *= stats.movementSpeed * 100;
            rb.AddForce(moveDirection * Time.deltaTime, ForceMode.Acceleration);

            playerVelocity = ((transform.position - _prevPosition) / Time.fixedDeltaTime).normalized * 1.25f;
            playerVelocity.y = 0;
            speed_Animation = Vector3.Distance(transform.position, _prevPosition);
            _prevPosition = transform.position;

            playerAnimator.SetFloat("MovementSpeed", speed_Animation);

        }

        //move camera
        Vector3 cameraTargetPos = this.transform.position;

        Vector3 mainCameraTargetPos = cameraTargetPos;
        mainCameraTargetPos.y = cameraHeight;
        mainCameraTargetPos.z -= Screen.height / 5.69f / 100;

        myCamera.transform.position = Vector3.Slerp(myCamera.transform.position, mainCameraTargetPos, (cameraSpeed / 10) * Time.deltaTime);
        MapCamera.transform.position = cameraTargetPos;

        Vector3 targetDir = myCamera.transform.position - transform.position;

        Vector3 newDir = Vector3.RotateTowards(new Vector3(90, 0, 0), targetDir, 10, 0.0F);
        newDir.x = newDir.x * .15f;
        newDir.z = newDir.z * .15f;
        newDir.y = 90;
        myCamera.transform.rotation = Quaternion.Euler(newDir);


        if (aimLineOn == true)
        {
            aimLineOpacity = Mathf.Lerp(aimLineOpacity, 1, 2f * Time.deltaTime);
        }
        else
        {
            aimLineOpacity = Mathf.Lerp(aimLineOpacity, 0, 7f * Time.deltaTime);
        }

        aimLine.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, aimLineOpacity);


    }

    void Dash()
    {
        dashAvailable = false;
        Vector3 bashDir = transform.forward;
        rb.AddForce(bashDir * stats.dashSpeed * 100 * Time.deltaTime, ForceMode.VelocityChange);
        GameObject _dashTrail = Instantiate(stats.dashTrail, transform.position, transform.rotation);
        _dashTrail.transform.parent = this.transform;

        StartCoroutine(DashCooldown());
    }

    IEnumerator _FireWeapon(int weaponNumber, string button)
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
            aimLineOn = true;
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
        while (Input.GetButton(button) && stats.energy - energyDrainAmount >= 0)
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
        aimLineOn = false;

    }

    IEnumerator _FireShield(int weaponNumber, string button)
    {
        //we're firing, dont fire again until we're done
        firing = true;
        Transform shield = transform.Find("Equipment").Find("ShieldSlot").GetChild(0).transform;
        string shieldType = shield.tag;

        ShieldBehavior shieldBehavior = shield.GetComponent<ShieldBehavior>();

        float _rotationSpeed = stats.rotationSpeed;
        float _movementSpeed = stats.movementSpeed;

        Vector4 weaponInfo = shieldBehavior.ShieldInfo();

        float fireSpeed = weaponInfo.x;
        float chargeSpeed = weaponInfo.y;
        float playerRotSlow = weaponInfo.z;
        float energyDrainAmount = weaponInfo.w;

        Animate(Shielding_Animation);
        stats.rotationSpeed = stats.rotationSpeed - (stats.rotationSpeed * playerRotSlow / 100);
        stats.movementSpeed = stats.movementSpeed / 2;


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
        stats.movementSpeed = _movementSpeed;
        Animate(Idle_Animation);
        firing = false;
        charged = false;


    }

    IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(stats.dashRecovery);
        dashAvailable = true;
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


}
