using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float gravity = -9.81f;
    public Transform cameraTransform;

    [Header("Stamina")]
    public float maxStamina = 5f;
    public float stamina;
    public float staminaRegenRate = 1f;
    public float staminaDrainRate = 1.5f;

    private CharacterController controller;
    private Vector3 verticalVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        stamina = maxStamina;
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = (cameraTransform.right * horizontal + cameraTransform.forward * vertical);
        direction.y = 0;
        direction.Normalize();

        bool isMoving = direction.magnitude > 0.1f;
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && stamina > 0f && isMoving;
        float speed = isRunning ? runSpeed : walkSpeed;

        // Перемещение персонажа
        Vector3 move = direction * speed;
        controller.Move(move * Time.deltaTime);

        // Гравитация
        if (!controller.isGrounded)
            verticalVelocity.y += gravity * Time.deltaTime;
        else
            verticalVelocity.y = -2f;

        controller.Move(verticalVelocity * Time.deltaTime);

        // Стамина
        if (isRunning)
        {
            stamina -= staminaDrainRate * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
        }
        else if (!isMoving)
        {
            stamina += staminaRegenRate * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
        }
    }
}