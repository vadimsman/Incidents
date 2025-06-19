using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class LobbyStarter : MonoBehaviour
{
    public Text statusText; // Можно не указывать, если UI не нужен

    void Start()
    {
        switch (MainMenuUI.ChosenRole)
        {
            case NetworkRole.Host:
                NetworkManager.Singleton.StartHost();
                SetStatus("Запущен Host");
                break;

            case NetworkRole.Client:
                NetworkManager.Singleton.StartClient();
                SetStatus("Подключение как клиент...");
                break;
        }

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    void OnClientConnected(ulong clientId)
    {
        SetStatus($"Подключён: {clientId}");
    }

    void OnClientDisconnected(ulong clientId)
    {
        SetStatus($"Отключён: {clientId}");
    }

    void SetStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
        else
            Debug.Log(message);
    }
}
