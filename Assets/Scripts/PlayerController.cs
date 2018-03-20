using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    GameManager gameManager;
    Player player;
    Stats stats;

    public float speed = 6.0F;
    public float rotationSpeed = 10;

    public GameObject myCamera;
    public float cameraSpeed;
    public float cameraHeight;

    Vector3 moveDirection;
    Vector3 _prevPosition;
    Camera mainCamera;
    Transform playerBody;

    Vector3 playerVelocity;

    private Animator animator;
    private const string Idle_Animation = "Idle"; 
    private const string Casting_Animation = "Casting";
    private const string Meleeing_Animation = "Meleeing";
    private const string Shielding_Animation = "Shielding";
    private float speed_Animation;

    public static bool firing;
    int activeWeapon = 0;

    private void Start()
    {
        gameManager = transform.parent.GetComponent<GameManager>();

        animator = GetComponent<Animator>();
        player = GetComponent<Player>();
        stats = GetComponent<Stats>();
        playerBody = transform.Find("Player Body");
    }

    void FixedUpdate()
    {
        // rotate player body towards mouse
        Plane playerPlane = new Plane(Vector3.up, playerBody.position);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float hitdist = 0.0f;
        if (playerPlane.Raycast(ray, out hitdist))
        {
            Vector3 targetPoint = ray.GetPoint(hitdist);
            Quaternion targetRotation = Quaternion.LookRotation(targetPoint - playerBody.position);
            playerBody.rotation = Quaternion.Slerp(playerBody.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        //move player
        Rigidbody rb = GetComponent<Rigidbody>();
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= speed * 100;
        rb.AddForce(moveDirection * Time.deltaTime, ForceMode.Acceleration);

        playerVelocity = ((transform.position - _prevPosition) / Time.fixedDeltaTime).normalized * 1.25f;
        speed_Animation = Vector3.Distance(transform.position, _prevPosition);
        _prevPosition = transform.position;

        animator.SetFloat("MovementSpeed", speed_Animation);

        //move camera
        Vector3 cameraTargetPos = this.transform.position;
        cameraTargetPos.y = cameraHeight;
        myCamera.transform.position = Vector3.Slerp(myCamera.transform.position, cameraTargetPos, (cameraSpeed / 10) * Time.deltaTime);

        Vector3 targetDir = myCamera.transform.position - transform.position;
        Vector3 newDir = Vector3.RotateTowards(new Vector3(90,0,0), targetDir, 10, 0.0F);
        newDir.x = newDir.x * .15f;
        newDir.z = newDir.z * .15f;
        newDir.y = 90;
        myCamera.transform.rotation = Quaternion.Euler(newDir);


        if (Input.GetButton("Fire1") && firing == false)
        {
            StartCoroutine(_FireWeapon(0, "Fire1"));
        }
        if (Input.GetButton("Fire2") && firing == false)
        {
            StartCoroutine(_FireWeapon(1, "Fire2"));
        }
        if (Input.GetButton("Fire3") && firing == false)
        {
            StartCoroutine(_DrawShield(2, "Fire3"));
        }

    }

    IEnumerator _FireWeapon(int weaponNumber, string button)
    {
        //we're firing, dont fire again until we're done
        firing = true;

        //swap weapons on fire
        if (activeWeapon != weaponNumber)
        {
            GameObject oldWeaponSlot = player.equipmentSlots[activeWeapon].gameObject;
            oldWeaponSlot.SetActive(false);

            GameObject weaponSlot = player.equipmentSlots[weaponNumber].gameObject;
            weaponSlot.SetActive(true);
            activeWeapon = weaponNumber;
        }

        WeaponBehavior weapon = player.transform.Find("Player Body").Find("Equipment").GetChild(weaponNumber).GetChild(0).GetComponent<WeaponBehavior>();

        float _rotationSpeed = rotationSpeed;

        Vector4 weaponInfo = weapon.WeaponInfo();

        float fireTime = weaponInfo.x;
        float chargeTime = weaponInfo.y;
        float playerRotSlow = weaponInfo.z;
        float energyDrainAmount = weaponInfo.w;
        string weaponType = player.transform.Find("Player Body").Find("Equipment").GetChild(weaponNumber).GetChild(0).tag;

        if (weaponType == "Staff")
        {
            Animate(Casting_Animation);
        }

        yield return new WaitForSeconds(chargeTime);

        //slow player rotation by weapon amount
        rotationSpeed = rotationSpeed - (rotationSpeed * playerRotSlow / 100);

        while (Input.GetButton(button) && stats.energy - energyDrainAmount >= 0)
        {

            if (weaponType == "Sword" || weaponType == "Dagger" ) // or any melee type..
            {
                Animate(Meleeing_Animation); // animate appropriate weapon type animation
                weapon.FireWeapon(playerBody, playerVelocity);
                stats.energy -= energyDrainAmount;
                yield return new WaitForSeconds(fireTime);
                Animate(Idle_Animation);
                yield return new WaitForSeconds(.01f);

            }

            if (weaponType == "Staff")
            {
                weapon.FireWeapon(playerBody, playerVelocity);
                stats.energy -= energyDrainAmount;

                yield return new WaitForSeconds(fireTime);
            }

            yield return new WaitForSeconds(.01f);


        }

        //set rotation speed to normal
        rotationSpeed = _rotationSpeed;
        Animate(Idle_Animation);
        firing = false;

    }


    IEnumerator _DrawShield(int weaponNumber, string button)
    {
        //we're firing, dont fire again until we're done
        firing = true;

        ShieldBehavior shield = player.transform.Find("Player Body").Find("Equipment").GetChild(weaponNumber).GetChild(0).GetComponent<ShieldBehavior>();

        float _rotationSpeed = rotationSpeed;

        Vector4 weaponInfo = shield.ShieldInfo();

        float fireTime = weaponInfo.x;
        float chargeTime = weaponInfo.y;
        float playerRotSlow = weaponInfo.z;
        float energyDrainAmount = weaponInfo.w;

        yield return new WaitForSeconds(chargeTime);

        rotationSpeed = rotationSpeed - (rotationSpeed * playerRotSlow / 100);

        bool shielding = false;
        while (Input.GetButton(button) && stats.energy - energyDrainAmount >= 0)
        {
                if (shielding == false)
                {
                shield.DrawShield(playerBody);
                shielding = true;
                Animate(Shielding_Animation);
                }

                yield return new WaitForSeconds(fireTime);
                stats.energy -= energyDrainAmount;
            
        }

        shield.PutAway();
        animator.SetBool(Shielding_Animation, false);


        rotationSpeed = _rotationSpeed;
        firing = false;

    }

    private void Animate(string boolName)
    {
        DisableOtherAnimations(animator, boolName);
        animator.SetBool(boolName, true);
    }

    private void DisableOtherAnimations(Animator animator, string animation)
    {
        foreach(AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.name != animation && parameter.name != "MovementSpeed")
            {
                animator.SetBool(parameter.name, false);
            }
        }

    }


}
