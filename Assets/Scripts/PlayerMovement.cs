using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 3f;      // �������� ������
    public float runSpeed = 6f;       // �������� ����
    public float gravity = -9.81f;    // ����������
    public Transform cameraTransform; // ������ ������ ��� ����������� ��������

    [Header("Stamina")]
    public float maxStamina = 5f;         // ������������ ����� ������������
    public float stamina;                // ������� ������������
    public float staminaRegenRate = 1f;  // �������� �������������� ������������
    public float staminaDrainRate = 1.5f;// �������� ������� ������������ ��� ����

    [Header("Camera Shake")]
    public CinemachineVirtualCamera vcam;                   // Cinemachine ������
    private CinemachineBasicMultiChannelPerlin perlin;      // ��������� ���� ������
    public float walkShake = 0.5f;                           // ������ ��� ������
    public float runShake = 1.5f;                            // ������ ��� ����

    private CharacterController controller; // ��������� ��� ������������
    private Vector3 velocity;               // ������������ �������� (����������)

    void Start()
    {
        controller = GetComponent<CharacterController>(); // �������� ����������
        stamina = maxStamina; // ������������� ������ ������������

        // �������� ��������� ������ ������
        if (vcam != null)
            perlin = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    void Update()
    {
        // �������� ����������� ����� (�����/������ � �����/�����)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = (cameraTransform.right * horizontal + cameraTransform.forward * vertical);
        direction.y = 0; // �� ��������� �� ���������
        direction.Normalize();

        // ��������: ����� �����, ���� ������ Shift, ���� ������������ � ����� ��������
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && stamina > 0f && direction.magnitude > 0.1f;
        float speed = isRunning ? runSpeed : walkSpeed;

        // �������� ������
        controller.Move(direction * speed * Time.deltaTime);

        // ���������� ����������
        if (!controller.isGrounded)
            velocity.y += gravity * Time.deltaTime;
        else
            velocity.y = -2f; // �������� � �����

        controller.Move(velocity * Time.deltaTime);

        // ������������: ������ ��� ����, ��������������� � �����
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

        // ������ ������ � ����������� �� �������� � ����
        if (perlin != null)
        {
            float shake = direction.magnitude > 0.1f ? (isRunning ? runShake : walkShake) : 0f;
            perlin.m_AmplitudeGain = shake;          // ������������� ������
            perlin.m_FrequencyGain = shake * 2f;     // ������� ������
        }


    }
}

