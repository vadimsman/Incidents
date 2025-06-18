using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 3f;      // Скорость ходьбы
    public float runSpeed = 6f;       // Скорость бега
    public float gravity = -9.81f;    // Гравитация
    public Transform cameraTransform; // Камера игрока для направления движения

    [Header("Stamina")]
    public float maxStamina = 5f;         // Максимальный запас выносливости
    public float stamina;                // Текущая выносливость
    public float staminaRegenRate = 1f;  // Скорость восстановления выносливости
    public float staminaDrainRate = 1.5f;// Скорость расхода выносливости при беге

    [Header("Camera Shake")]
    public CinemachineVirtualCamera vcam;                   // Cinemachine камера
    private CinemachineBasicMultiChannelPerlin perlin;      // Компонент шума камеры
    public float walkShake = 0.5f;                           // Тряска при ходьбе
    public float runShake = 1.5f;                            // Тряска при беге

    private CharacterController controller; // Компонент для передвижения
    private Vector3 velocity;               // Вертикальное движение (гравитация)

    void Start()
    {
        controller = GetComponent<CharacterController>(); // Получаем контроллер
        stamina = maxStamina; // Устанавливаем полную выносливость

        // Получаем компонент тряски камеры
        if (vcam != null)
            perlin = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    void Update()
    {
        // Получаем направление ввода (влево/вправо и вперёд/назад)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = (cameraTransform.right * horizontal + cameraTransform.forward * vertical);
        direction.y = 0; // Не двигаемся по вертикали
        direction.Normalize();

        // Проверка: игрок бежит, если нажата Shift, есть выносливость и игрок движется
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && stamina > 0f && direction.magnitude > 0.1f;
        float speed = isRunning ? runSpeed : walkSpeed;

        // Движение игрока
        controller.Move(direction * speed * Time.deltaTime);

        // Применение гравитации
        if (!controller.isGrounded)
            velocity.y += gravity * Time.deltaTime;
        else
            velocity.y = -2f; // Залипаем к земле

        controller.Move(velocity * Time.deltaTime);

        // Выносливость: тратим при беге, восстанавливаем в покое
        if (isRunning)
        {
            stamina -= staminaDrainRate * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
        }
        else if (direction.magnitude < 0.1f)
        {
            stamina += staminaRegenRate * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
        }

        // Тряска камеры в зависимости от движения и бега
        if (perlin != null)
        {
            float shake = direction.magnitude > 0.1f ? (isRunning ? runShake : walkShake) : 0f;
            perlin.m_AmplitudeGain = shake;          // Интенсивность тряски
            perlin.m_FrequencyGain = shake * 2f;     // Частота тряски
        }


    }
}

