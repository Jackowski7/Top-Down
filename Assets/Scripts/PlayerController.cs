using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    GameManager gameManager;
    Player player;

    public float speed = 6.0F;
    public float rotationSpeed = 10;
    public float gravity = 10.0F;

    public float cameraSpeed;
    public float cameraHeight;

    Vector3 moveDirection;
    Vector3 _prevPosition;
    Camera mainCamera;
    Transform playerBody;


    public static bool firing;

    private void Start()
    {
        mainCamera = Camera.main;
        gameManager = transform.parent.GetComponent<GameManager>();
        player = GetComponent<Player>();
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
        CharacterController controller = GetComponent<CharacterController>();
        if (controller.isGrounded)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;
        }

        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);

        Vector3 cameraTargetPos = this.transform.position;
        cameraTargetPos.y = cameraHeight;
        mainCamera.transform.position = Vector3.Slerp(mainCamera.transform.position, cameraTargetPos, (cameraSpeed / 10) * Time.deltaTime);

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
            StartCoroutine(_FireWeapon(2, "Fire3"));
        }

    }

    IEnumerator _FireWeapon(int weaponNumber, string button)
    {

        WeaponBehavior weapon = player.equipment[weaponNumber].GetComponent<WeaponBehavior>();

        firing = true;
        float _rotationSpeed = rotationSpeed;

        Vector4 weaponInfo = weapon.WeaponInfo();

        float fireSpeed = weaponInfo.x;
        float chargeTime = weaponInfo.y;
        float playerRotSlow = weaponInfo.z;
        float energyDrainAmount = weaponInfo.w;

        yield return new WaitForSeconds(chargeTime);

        rotationSpeed = rotationSpeed - (rotationSpeed * playerRotSlow / 100);
	

			while (Input.GetButton (button) && player.energy - energyDrainAmount >= 0) {

				Vector3 playerVelocity;

				playerVelocity = ((transform.position - _prevPosition) / Time.fixedDeltaTime).normalized * 1.25f;
				_prevPosition = transform.position;

				weapon.FireWeapon (playerBody, playerVelocity);
				player.energy -= energyDrainAmount;

				yield return new WaitForSeconds (fireSpeed);
			}

        rotationSpeed = _rotationSpeed;
        firing = false;

    }

}
