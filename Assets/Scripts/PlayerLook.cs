using UnityEngine;
using Unity.Netcode; // Для сетевой логики (определение владельца)

public class PlayerLook : NetworkBehaviour
{
    public Transform playerBody;        // Объект тела игрока (вращается по горизонтали)
    public Transform cameraRoot;        // Камера или её контейнер (вращается по вертикали)
    public float mouseSensitivity = 2f; // Чувствительность мыши

    private float xRotation = 0f; // Текущее вертикальное вращение камеры (вверх/вниз)

    void Update()
    {
        // Управление только у владельца объекта
        if (!IsOwner) return;

        // Получаем движение мыши
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Поворачиваем тело игрока по горизонтали (оси Y)
        playerBody.Rotate(Vector3.up * mouseX);

        // Обновляем вертикальное вращение камеры
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f); // Ограничиваем угол обзора

        // Применяем поворот к "голове" (камера наклоняется вверх/вниз)
        cameraRoot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}

