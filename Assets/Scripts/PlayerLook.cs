using UnityEngine;
using Unity.Netcode; // ��� ������� ������ (����������� ���������)

public class PlayerLook : NetworkBehaviour
{
    public Transform playerBody;        // ������ ���� ������ (��������� �� �����������)
    public Transform cameraRoot;        // ������ ��� � ��������� (��������� �� ���������)
    public float mouseSensitivity = 2f; // ���������������� ����

    private float xRotation = 0f; // ������� ������������ �������� ������ (�����/����)

    void Update()
    {
        // ���������� ������ � ��������� �������
        if (!IsOwner) return;

        // �������� �������� ����
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // ������������ ���� ������ �� ����������� (��� Y)
        playerBody.Rotate(Vector3.up * mouseX);

        // ��������� ������������ �������� ������
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f); // ������������ ���� ������

        // ��������� ������� � "������" (������ ����������� �����/����)
        cameraRoot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}

