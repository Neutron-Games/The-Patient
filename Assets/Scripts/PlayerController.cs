using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour
{
    public bool canMove;public bool canRun; public bool canJump; public bool canCrouch;
    bool isRunning => Input.GetKey(sprintKey) && canRun && !isCrouching;
    bool isJumping => Input.GetKeyDown(jumpKey) && characterController.isGrounded;

    bool shouldCrouch => Input.GetKeyDown(crouchKey) && !duringCrouch && characterController.isGrounded;

    [Header("Controls")]
    KeyCode sprintKey = KeyCode.LeftShift;
    KeyCode jumpKey = KeyCode.Space;
    KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintSpeed = 5.0f;
    [SerializeField] private float crouchingSpeed = 2.0f;
    [SerializeField] public float currentStamina = 10.0f;
    [SerializeField] public float maxStamina = 10.0f;
    [SerializeField] private float staminaGain = .2f;
    [SerializeField] private float staminaLose = .1f;
    private float notRunningTime;
    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce = 8.0f;
    [SerializeField] private float gravity = 30.0f;

    [Header("Look Parameters")]
    [SerializeField, Range(0,3)] private float lookSpeedX = 2;
    [SerializeField, Range(0,3)] private float lookSpeedY = 2;
    [SerializeField, Range(1,90)] private float LookLimit = 50;

    [Header("Crouch Parameters")]
    [SerializeField] private float standingHeight = 2;
    [SerializeField] private float crouchingHeight = 0.5f;
    [SerializeField] private float timeToCrouch = 0.25f;
    [SerializeField] private Vector3 standingCenter = new Vector3(0,1,0);
    [SerializeField] private Vector3 crouchingCenter = new Vector3(0,1,0);
    private bool isCrouching;
    private bool duringCrouch;

    [Header("HeadBob Parameters")]
    [SerializeField] private float walkBobSpeed = 14;
    [SerializeField] private float walkBobAmount = .05f;
    [SerializeField] private float runBobSpeed = 18;
    [SerializeField] private float runBobAmount = .1f;
    [SerializeField] private float crouchBobSpeed = 8;
    [SerializeField] private float crouchBobAmount = .025f;
    private float defatultYPos = 0;
    private float timer = 0;

    Camera playerCamera;
    CharacterController characterController;

    Vector2 currentInput;
    Vector3 moveDirection;

    float rotationX;

    private void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();
        defatultYPos = playerCamera.transform.position.y;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (canMove)
        {
            HandleMovementInput();
            HandleMouseInput();

            if (canJump)
            {
                HandleJump();
            }
            if (canCrouch)
            {
                HandleCrouch();
            }
            HandleHeadBob();

            ApplyFinalMovement();
        }
    }

    private void HandleHeadBob()
    {
        if (!characterController.isGrounded) return;

        if (Mathf.Abs(moveDirection.x) > .1f || Mathf.Abs(moveDirection.z) > .1f)
        {
            timer += Time.deltaTime * (isCrouching ? crouchBobSpeed : isRunning ? runBobSpeed : walkBobSpeed);
            playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x,
                defatultYPos + Mathf.Sin(timer) * (isCrouching ? crouchBobAmount : isRunning ? runBobAmount : walkBobAmount), playerCamera.transform.localPosition.z);
        }
    }

    private void HandleCrouch()
    {
        if (shouldCrouch)
        {
            StartCoroutine(CrouchStand());
        }
    }

    private void HandleJump()
    {
        if (isJumping)
        moveDirection.y = jumpForce;
    }

    void HandleMovementInput()
    {
        currentInput = new Vector2 ((isRunning ? sprintSpeed : isCrouching ? crouchingSpeed :walkSpeed) * Input.GetAxis("Vertical"), (isRunning ? sprintSpeed : isCrouching ? crouchingSpeed : walkSpeed) * Input.GetAxis("Horizontal"));
        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection.y = moveDirectionY;
        if (currentStamina < 0.01f)
        {
            canRun = false;
        }
        else
        {
            canRun = true;
        }
        if (notRunningTime > 3)
        {
            currentStamina += staminaGain * Time.deltaTime;            
        }
        if (isRunning)
        {
            notRunningTime = 0;
            currentStamina -= staminaLose * Time.deltaTime;
        }
        else if (!isRunning && currentStamina < maxStamina && notRunningTime <4) { notRunningTime += Time.deltaTime; }
        else if (currentStamina > maxStamina)
        {
            currentStamina = maxStamina;
        }
    }
    void HandleMouseInput()
    {
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -LookLimit, LookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);
    }
    IEnumerator CrouchStand()
    {
        if (isCrouching && Physics.Raycast(playerCamera.transform.position, Vector3.up, 1))
        duringCrouch = true;
        float timeElapsed = 0;
        float targerHeight = isCrouching ? standingHeight : crouchingHeight;
        float currentHeight = characterController.height;
        Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
        Vector3 currentCenter = characterController.center;

        while (timeElapsed < timeToCrouch)
        {
            characterController.height = Mathf.Lerp(currentHeight, targerHeight, timeElapsed / timeToCrouch);
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        characterController.height = targerHeight;
        characterController.center = targetCenter;
        isCrouching = !isCrouching;
        duringCrouch = false;
    }
    void ApplyFinalMovement()
    {
        if (!characterController.isGrounded)
            moveDirection.y -= gravity * Time.deltaTime;
        characterController.Move(moveDirection * Time.deltaTime);
    }
}
