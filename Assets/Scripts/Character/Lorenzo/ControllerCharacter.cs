using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public Transform cam;
    public float speed = 6f;
    private Animator animator = null;

    public float gravity = 14f;
    public float turnSmoothTime = 0.1f;
    public float turnSmoothVelocity;
    public float jumpHigh = 5f;
    public float verticalV;

    public float sprintSpeed = 15f;
    Vector3 moveDirection;

    Camera cams;

    void Start()
    {
        animator = GetComponent<Animator>();
        cams = Camera.main;

    }

    // Update is called once per frame
    void Update()
    {

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        Vector3 gravityVector = Vector3.zero;


        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            speed = 6f;
            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            //Debug.Log(speed);
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

    }
}
