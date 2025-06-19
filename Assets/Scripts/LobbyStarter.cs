using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class LobbyStarter : MonoBehaviour
{
    public Text statusText; // ����� �� ���������, ���� UI �� �����

    void Start()
    {
        switch (MainMenuUI.ChosenRole)
        {
            case NetworkRole.Host:
                NetworkManager.Singleton.StartHost();
                SetStatus("������� Host");
                break;

            case NetworkRole.Client:
                NetworkManager.Singleton.StartClient();
                SetStatus("����������� ��� ������...");
                break;
        }

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
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
