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


    public static bool firing;
    int activeWeapon = 0;

    private void Start()
    {
        gameManager = transform.parent.GetComponent<GameManager>();
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

        float fireSpeed = weaponInfo.x;
        float chargeTime = weaponInfo.y;
        float playerRotSlow = weaponInfo.z;
        float energyDrainAmount = weaponInfo.w;

        yield return new WaitForSeconds(chargeTime);

        rotationSpeed = rotationSpeed - (rotationSpeed * playerRotSlow / 100);

        while (Input.GetButton(button) && stats.energy - energyDrainAmount >= 0)
        {

            Vector3 playerVelocity;

            playerVelocity = ((transform.position - _prevPosition) / Time.fixedDeltaTime).normalized * 1.25f;
            _prevPosition = transform.position;

                weapon.FireWeapon(playerBody, playerVelocity);
                stats.energy -= energyDrainAmount;
                yield return new WaitForSeconds(fireSpeed);
        }
       

        rotationSpeed = _rotationSpeed;
        firing = false;

    }


    IEnumerator _DrawShield(int weaponNumber, string button)
    {
        //we're firing, dont fire again until we're done
        firing = true;

        ShieldBehavior shield = player.transform.Find("Player Body").Find("Equipment").GetChild(weaponNumber).GetChild(0).GetComponent<ShieldBehavior>();

        float _rotationSpeed = rotationSpeed;

        Vector4 weaponInfo = shield.ShieldInfo();

        float fireSpeed = weaponInfo.x;
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
                }

                yield return new WaitForSeconds(fireSpeed);
                stats.energy -= energyDrainAmount;
            
        }

        shield.PutAway();

        rotationSpeed = _rotationSpeed;
        firing = false;

    }


}
