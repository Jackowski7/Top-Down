using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    GameManager gameManager;
    Player player;

    public float speed = 6.0F;
    public float rotationSpeed = 10;
    public float gravity = 10.0F;

    Vector3 moveDirection;
    Vector3 _prevPosition;
    Camera mainCamera;
    Transform enemyBody;


    public static bool firing;

    private void Start()
    {
        gameManager = transform.parent.GetComponent<GameManager>();
        player = GetComponent<Player>();
        enemyBody = transform.Find("EnemyBody");
    }

    void FixedUpdate()
    {

        //move player
        CharacterController controller = GetComponent<CharacterController>();
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);

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

				weapon.FireWeapon (enemyBody, playerVelocity);
				player.energy -= energyDrainAmount;

				yield return new WaitForSeconds (fireSpeed);
			}

        rotationSpeed = _rotationSpeed;
        firing = false;

    }

}
