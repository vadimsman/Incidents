using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Transform playerBody;        // ������ ������ (Player)
    public Transform cameraRoot;        // ������ ������ (CameraRoot)
    public float mouseSensitivity = 2f;

    float xRotation = 0f;

    void Update()
    {
        // �������� ���� ����
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // ������� ���� ������ �� ��� Y
        playerBody.Rotate(Vector3.up * mouseX);

        // ������������ ������� �� X (������ �����/����)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        // ������� ������ "������" (cameraRoot)
        cameraRoot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}

