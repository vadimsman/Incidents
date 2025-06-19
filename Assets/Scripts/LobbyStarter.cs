using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class LobbyStarter : MonoBehaviour
{
    public Text statusText;

    void Start()
    {
        // ������ �� ��������� Host ��� Client �������, ��� ������ RelayConnector
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

        SetStatus("�������� �����������...");
    }

    void OnClientConnected(ulong clientId)
    {
        SetStatus($"���������: {clientId}");
    }

    void OnClientDisconnected(ulong clientId)
    {
        SetStatus($"��������: {clientId}");
    }

    void SetStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
        else
            Debug.Log(message);
    }
}
