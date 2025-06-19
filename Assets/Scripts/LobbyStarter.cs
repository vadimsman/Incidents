using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class LobbyStarter : MonoBehaviour
{
    public Text statusText;

    void Start()
    {
        // Больше не запускаем Host или Client вручную, это делает RelayConnector
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

        SetStatus("Ожидание подключения...");
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
