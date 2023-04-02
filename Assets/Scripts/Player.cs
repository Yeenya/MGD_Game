using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;
    public int cash = 100;

    private int fireballCooldown = 2;
    private bool fireballOnCooldown = false;
    private MainManager manager;
    private InGameMenuController menuController;

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
            GameObject newMissile = Instantiate(missile, transform.position, playerCamera.transform.rotation);
            newMissile.AddComponent<Missile>();
            newMissile.GetComponent<AudioSource>().volume = manager.soundVolume;
            menuController.fireballIcon.style.unityBackgroundImageTintColor = Color.black;
            DOTween.To(()=> menuController.fireballIcon.style.unityBackgroundImageTintColor.value, x => menuController.fireballIcon.style.unityBackgroundImageTintColor = x, Color.white, fireballCooldown);
            StartCoroutine(FireballCooldown());
        }

        if (Input.GetButtonDown("Fire2"))
        {
            TryBuyAlly();
        }

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
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
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    IEnumerator FireballCooldown()
    {
        fireballOnCooldown = true;
        yield return new WaitForSeconds(fireballCooldown);
        fireballOnCooldown = false;
    }

    void TryBuyAlly()
    {
        //Tweenovat barvu bilyho ctverecku s buy warrior z cervene na bilou, kdyz nejsou prachy
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
                else
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
        menuController.UpdateCash(cash);
    }
}