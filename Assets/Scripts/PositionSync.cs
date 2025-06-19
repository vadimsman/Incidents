using Unity.Netcode;
using UnityEngine;

/// <summary>
/// ������� ������ ������������� ������� � �������� ������ ����� NetworkVariable
/// </summary>
public class PositionSync : NetworkBehaviour
{
    // ������������� �������
    private NetworkVariable<Vector3> netPosition = new NetworkVariable<Vector3>(writePerm: NetworkVariableWritePermission.Owner);

    // ������������� ��������
    private NetworkVariable<Quaternion> netRotation = new NetworkVariable<Quaternion>(writePerm: NetworkVariableWritePermission.Owner);

    void Update()
    {
        if (IsOwner)
        {
            // ���������� ������� ������� � �������� ��������� ������� �� ������
            netPosition.Value = transform.position;
            netRotation.Value = transform.rotation;
        }
        else
        {
            // ��������� ������ �������� � ������ ��������� ������������������ ������� � ��������
            transform.position = Vector3.Lerp(transform.position, netPosition.Value, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Slerp(transform.rotation, netRotation.Value, Time.deltaTime * 10f);
        }
    }
}

