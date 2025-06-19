using UnityEngine;
using Cinemachine;
using Unity.Netcode;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 3f;       // Скорость ходьбы
    public float runSpeed = 6f;        // Скорость бега
    public float gravity = -9.81f;     // Сила гравитации
    public Transform cameraTransform;  // Камера игрока (для направления движения)

    [Header("Stamina")]
    public float maxStamina = 5f;          // Максимальная выносливость
    public float stamina;                 // Текущая выносливость
    public float staminaRegenRate = 1f;   // Скорость восстановления
    public float staminaDrainRate = 1.5f; // Расход при беге

    private CharacterController controller;   // Компонент CharacterController
    private Vector3 verticalVelocity;         // Вертикальная составляющая (гравитация)

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            Debug.LogError($"[{name}] CharacterController отсутствует на префабе!");
        }

    }

    void Start()
    {
        stamina = maxStamina;

        // Попытка найти камеру, если не назначена
        if (cameraTransform == null)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null)
            {
                cameraTransform = cam.transform;
                Debug.Log($"[{name}] Камера найдена автоматически.");
            }
            else
            {
                Debug.LogWarning($"[{name}] Камера не найдена!");
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log($"[{name}] OnNetworkSpawn | Owner: {IsOwner}");

        if (IsOwner)
        {
            if (controller == null)
                controller = GetComponent<CharacterController>();

            transform.position = new Vector3(Random.Range(1f, 3f), 2f, Random.Range(1f, 3f));
            Debug.Log($"[{name}] Owner assigned random position: {transform.position}");

            // Пинок вниз, чтобы сразу активировать гравитацию
            controller.Move(Vector3.down * 0.1f);
        }
    }



    void Update()
    {
        if (!IsOwner)
        {
            // Отключаем камеру у невладельца
            if (cameraTransform != null)
                cameraTransform.gameObject.SetActive(false);
            return;
        }

        // Получаем ввод игрока
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = Vector3.zero;

        if (cameraTransform != null)
        {
            direction = (cameraTransform.right * horizontal + cameraTransform.forward * vertical);
            direction.y = 0f;
            direction.Normalize();
        }

        bool isMoving = direction.magnitude > 0.1f;
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && stamina > 0f && isMoving;
        float speed = isRunning ? runSpeed : walkSpeed;

        // Объединяем горизонтальное движение и гравитацию
        Vector3 totalMove = direction * speed;

        // Применяем гравитацию
        if (!controller.isGrounded)
            verticalVelocity.y += gravity * Time.deltaTime;
        else
            verticalVelocity.y = -2f;

        totalMove += verticalVelocity;
        controller.Move(totalMove * Time.deltaTime);

        // Выносливость
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

