using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Простая ручная синхронизация позиции и вращения игрока через NetworkVariable
/// </summary>
public class PositionSync : NetworkBehaviour
{
    // Синхронизация позиции
    private NetworkVariable<Vector3> netPosition = new NetworkVariable<Vector3>(writePerm: NetworkVariableWritePermission.Owner);

    // Синхронизация вращения
    private NetworkVariable<Quaternion> netRotation = new NetworkVariable<Quaternion>(writePerm: NetworkVariableWritePermission.Owner);

    void Update()
    {
        if (IsOwner)
        {
            // Отправляем текущую позицию и вращение владельца объекта на сервер
            netPosition.Value = transform.position;
            netRotation.Value = transform.rotation;
        }
        else
        {
            // Остальные игроки получают и плавно применяют синхронизированную позицию и вращение
            transform.position = Vector3.Lerp(transform.position, netPosition.Value, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Slerp(transform.rotation, netRotation.Value, Time.deltaTime * 10f);
        }
    }
}

