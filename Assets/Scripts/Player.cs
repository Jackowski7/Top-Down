using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public float speed = 6.0F;
    public float gravity = 10.0F;
    private Vector3 moveDirection = Vector3.zero;
    public float cameraSpeed;

    public static Vector3 lastEnterPoint;

    public GameObject mainCamera;
    

    private void Start()
    {
        if (GameManager.hasCamera == false)
        {
            mainCamera = Instantiate(mainCamera, transform.position, Quaternion.Euler(90, 0, 0));
            mainCamera.name = "Main Camera";
            GameManager.hasCamera = true;
        }
        else
        {
            mainCamera.transform.position = transform.position;
        }
    }

    void Update()
    {
        CharacterController controller = GetComponent<CharacterController>();
        if (controller.isGrounded)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;
            //if (Input.GetButton("Jump"))
            //    moveDirection.y = jumpSpeed;

        }
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);

        Vector3 cameraTargetPos = this.transform.position;
        cameraTargetPos.y = 20;
        mainCamera.transform.position = Vector3.Slerp(mainCamera.transform.position, cameraTargetPos, (cameraSpeed/10) * Time.deltaTime);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Lava")
        {
            ResetPlayer();
            Debug.Log("you're on fire!");
        }
    }

    public void ResetPlayer()
    {
        transform.position = lastEnterPoint;
    }

}
