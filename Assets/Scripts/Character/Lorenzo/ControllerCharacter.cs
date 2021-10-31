using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using System;
using UnityEngine.UI;

public class ControllerCharacter : MonoBehaviour
{
    //public CharacterController controller;
    //public Transform cam;

    //public float speed = 6f;

    //public float turnSmoothTime = 0.1f;
    //float turnSmoothVelocity;
    //void Update()
    //{
    //    float horizontal = Input.GetAxisRaw("Horizontal");
    //    float vertical = Input.GetAxisRaw("Vertical");
    //    Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

    //    if(direction.magnitude >= 0.1f)
    //    {
    //        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
    //        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
    //        transform.rotation = Quaternion.Euler(0f, angle, 0f);

    //        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

    //        controller.Move(moveDir.normalized * speed * Time.deltaTime);
    //    }
    //}

    public CharacterController controller;
    public GameObject mainCam, shootingCam, aimingCamRight, aimingCamLeft, aimingCam;
    public Text coreItemText, bulletCountText;
    public static bool ShootingMode = false;
    public static bool AimingMode = false;
    private bool isRight = true;
    public Transform cam;
    public float speed = 6f;
    private Animator animator = null;
    private int bulletCount = 150;

    public float gravity = 14f;
    public float turnSmoothTime = 0.1f;
    public float turnSmoothVelocity;
    public float jumpHigh = 5f;
    public float verticalV;
    public float aimDuration = 0.3f;

    public int maxAmmo = 30;
    private int currentAmmo = -1;
    public float reloadTime = 1f;

    public float sprintSpeed = 15f;
    Vector3 moveDirection;

    Camera cams;
    RaycastWeapon weapon;
    public Rig aimLayer;
    public static int coreItems;

    public Inventory inventory;
    int isAimingParam = Animator.StringToHash("isAiming");

    void Start()
    {
        //if(currentAmmo == -1) currentAmmo = maxAmmo;
        animator = GetComponent<Animator>();
        cams = Camera.main;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        weapon = GetComponentInChildren<RaycastWeapon>();
    }

    public void Reload()
    {
        Debug.Log("Reloading..");
        currentAmmo = maxAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(DialogueManager.dialogueActive == false && Player.IsAlive == true);
        //Debug.Log("Dialogue Active : " + DialogueManager.dialogueActive);
        //Debug.Log("Player Active : " + Player.IsAlive);
        //if (currentAmmo <= 0)
        //{
        //    Reload();
        //    return;
        //}
        coreItemText.text = coreItems.ToString();
        bulletCountText.text = bulletCount.ToString();
        if (DialogueManager.dialogueActive == false)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
            Vector3 gravityVector = Vector3.zero;



            if (direction.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                if (!ShootingMode)
                {
                    transform.rotation = Quaternion.Euler(0f, angle, 0f);
                }

                speed = 6f;
                moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(moveDirection.normalized * speed * Time.deltaTime);
                animator.SetFloat("move", 1f * direction.magnitude, turnSmoothTime, Time.deltaTime);
            }
            else
            {
                animator.SetFloat("move", 0f * direction.magnitude, turnSmoothTime, Time.deltaTime);
            }


            if (controller.isGrounded)
            {
                verticalV = -gravity * Time.deltaTime;
            }
            else
            {
                verticalV -= gravity * Time.deltaTime;

            }

            gravityVector.y = verticalV;
            controller.Move(gravityVector * Time.deltaTime);

            if (ShootingMode || AimingMode)
            {
                if (Input.GetButtonDown("Fire1") && bulletCount > 0)
                {
                    currentAmmo--;
                    bulletCount--;
                    SoundManager.PlaySound("ShotSFX");
                    weapon.StartFiring();
                }
                if (weapon.isFiring)
                {
                    weapon.UpdateFiring(Time.deltaTime);
                }
                weapon.UpdateBullets(Time.deltaTime);
                if (Input.GetButtonUp("Fire1"))
                {
                    weapon.StopFiring();
                }
            }

            //WeaponAiming

            if (Input.GetKeyDown(KeyCode.C))
            {
                if (ShootingMode)
                {
                    mainCam.SetActive(true);
                    shootingCam.SetActive(false);
                    ShootingMode = false;
                    StartCoroutine(Put());
                }
                else
                {
                    mainCam.SetActive(false);
                    shootingCam.SetActive(true);
                    ShootingMode = true;
                    StartCoroutine(Shoot());
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                if (AimingMode)
                {
                    aimingCam.SetActive(false);
                    shootingCam.SetActive(true);
                    AimingMode = false;
                    ShootingMode = true;
                }
                else if (ShootingMode)
                {
                    aimingCam.SetActive(true);
                    if (isRight)
                    {
                        aimingCamRight.SetActive(true);
                        aimingCamLeft.SetActive(false);
                    }
                    else
                    {
                        aimingCamLeft.SetActive(true);
                        aimingCamRight.SetActive(false);
                    }
                    shootingCam.SetActive(false);
                    AimingMode = true;
                    ShootingMode = false;
                }
            }

            if (AimingMode)
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    Debug.Log("Tab & Right");
                    if (isRight)
                    {
                        aimingCamRight.SetActive(false);
                        aimingCamLeft.SetActive(true);
                        isRight = false;
                    }
                    else
                    {
                        aimingCamRight.SetActive(true);
                        aimingCamLeft.SetActive(false);
                        isRight = true;
                    }
                }
            }

            //bool isAiming = Input.GetMouseButton(1);
            //animator.SetBool(isAimingParam, isAiming);

        }
    }

    IEnumerator Shoot()
    {
        while(aimLayer.weight < 1)
        {
            yield return null;
            aimLayer.weight += Time.deltaTime / aimDuration;
        }
        yield return null;
    }

    IEnumerator Put()
    {
        while (aimLayer.weight > 0)
        {
            yield return null;
            aimLayer.weight -= Time.deltaTime / aimDuration;
        }
        yield return null;

    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        GameObject item = hit.gameObject;
        if (item.tag.Equals("Item"))
        {
            if (item.name.Contains("CoreItem"))
            {
                coreItems++;
                Debug.Log(coreItems);
            }
            else
            {
                inventory.AddItem(item.name);
            }
            Destroy(item);
        }
    }
}
