using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {


    GameManager gameManager;


    public float speed = 6.0F;
    public float rotationSpeed = 10;

    public float gravity = 10.0F;
    public Vector3 moveDirection;

    public float cameraSpeed;
    public float cameraHeight;

    public static Vector3 lastEnterPoint;

    Camera mainCamera;
    GameObject playerBody;

    private void Start()
    {
        mainCamera = Camera.main;
        gameManager = transform.parent.GetComponent<GameManager>();

        playerBody = transform.Find("Player Body").gameObject;
    }

    void Update()
    {
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
        mainCamera.transform.position = Vector3.Slerp(mainCamera.transform.position, cameraTargetPos, (cameraSpeed/10) * Time.deltaTime);
    }


    void FixedUpdate()
    {
        // rotate player body towards mouse
        Plane playerPlane = new Plane(Vector3.up, playerBody.transform.position);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float hitdist = 0.0f;
        if (playerPlane.Raycast(ray, out hitdist))
        {
            Vector3 targetPoint = ray.GetPoint(hitdist);
            Quaternion targetRotation = Quaternion.LookRotation(targetPoint - playerBody.transform.position);
            playerBody.transform.rotation = Quaternion.Slerp(playerBody.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }


    public void OnCollisionEnter(Collision collision)
    {
            if (collision.gameObject.tag == "Lava")
        {
            ResetPlayer();
            Debug.Log("you're on fire!");
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


    public void ResetPlayer()
    {
        transform.position = lastEnterPoint;
    }

}
