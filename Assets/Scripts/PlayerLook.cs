using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Transform playerBody;        // Объект игрока (Player)
    public Transform cameraRoot;        // Объект камеры (CameraRoot)
    public float mouseSensitivity = 2f;

    float xRotation = 0f;

    void Update()
    {
        // Получаем ввод мыши
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Вращаем тело игрока по оси Y
        playerBody.Rotate(Vector3.up * mouseX);

        // Ограничиваем поворот по X (взгляд вверх/вниз)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        // Вращаем только "голову" (cameraRoot)
        cameraRoot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}

