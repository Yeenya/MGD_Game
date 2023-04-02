using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    [Header("Variables based around movement")]
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    [Header("The rest of the variables")]
    public int cash = 100;

    private int fireballCooldown = 2;
    private bool fireballOnCooldown = false;
    private MainManager manager;
    private InGameMenuController menuController;

    public Joystick leftJoystick;
    public Joystick rightJoystick;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    [SerializeField]
    private GameObject missile;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        menuController = GameObject.FindGameObjectWithTag("UI").GetComponent<InGameMenuController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<MainManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1") && !fireballOnCooldown)
        {
            ShootFireball();
        }

        if (Input.GetButtonDown("Fire2"))
        {
            TryBuyAlly();
        }

        //Movement code used from here: https://sharpcoderblog.com/blog/unity-3d-fps-controller
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        if (leftJoystick != null) // If on mobile, input must be handled differently
        {
            verticalInput = leftJoystick.Vertical;
            horizontalInput = leftJoystick.Horizontal;
        }
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * verticalInput : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * horizontalInput : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            float xInput = Input.GetAxis("Mouse X");
            float yInput = Input.GetAxis("Mouse Y");
            if (rightJoystick != null) // Same as above with the mobile input
            {
                xInput = rightJoystick.Horizontal;
                yInput = rightJoystick.Vertical;
            }
            rotationX += -yInput * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, xInput * lookSpeed, 0);
        }
    }

    IEnumerator FireballCooldown()
    {
        fireballOnCooldown = true;
        yield return new WaitForSeconds(fireballCooldown);
        fireballOnCooldown = false;
    }

    public void TryBuyAlly() // Checks if the player is aiming at the ally spawner, and if possible, spawns the ally
    {
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hitInfo))
        {
            if (hitInfo.collider.tag == "Spawner")
            {
                AllySpawner spawner = hitInfo.collider.GetComponent<AllySpawner>();
                if (cash >= spawner.cost && !spawner.allySpawned)
                {
                    UpdateCash(-spawner.cost);
                    spawner.SpawnAlly();
                }
                else // Make a small red flash so that the player notices he is short of cash
                {
                    spawner.GetComponent<MeshRenderer>().material.color = Color.red;
                    spawner.GetComponent<MeshRenderer>().material.DOColor(Color.white, 1);
                }
            }
        }
    }

    public void UpdateCash(int value)
    {
        cash += value;
        if (leftJoystick == null)
        {
            menuController.UpdateCash(cash);
        }
    }

    public void ShootFireball()
    {
        if (fireballOnCooldown) return;
        GameObject newMissile = Instantiate(missile, transform.position, playerCamera.transform.rotation);
        newMissile.AddComponent<Missile>();
        newMissile.GetComponent<AudioSource>().volume = manager.soundVolume;
        menuController.fireballIcon.style.unityBackgroundImageTintColor = Color.black;

        // Visualize cooldown with a simple tween
        DOTween.To(() => menuController.fireballIcon.style.unityBackgroundImageTintColor.value, x => menuController.fireballIcon.style.unityBackgroundImageTintColor = x, Color.white, fireballCooldown);
        
        StartCoroutine(FireballCooldown());
    }
}
