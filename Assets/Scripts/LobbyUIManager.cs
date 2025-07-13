using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.Netcode;
using TMPro;

public class LobbyUIManager : MonoBehaviour
{
    [Header("UI Ссылки")]
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject hostPanel;
    [SerializeField] private GameObject clientPanel;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button joinLobbyButton;
    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private Toggle readyToggle;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Transform playerListContainer;
    [SerializeField] private GameObject playerListEntryPrefab;

    [Header("Логика")]
    [SerializeField] private LobbyConnector lobbyConnector;

    // Список готовности по ID игроков
    private Dictionary<ulong, bool> readyStates = new Dictionary<ulong, bool>();
    // UI элементы игроков
    private Dictionary<ulong, GameObject> playerUIEntries = new Dictionary<ulong, GameObject>();

    private void Awake()
    {
        createLobbyButton.onClick.AddListener(() =>
        {
            lobbyConnector.CreateLobby();
            ActivateLobbyUI(true);
        });

        joinLobbyButton.onClick.AddListener(() =>
        {
            string code = joinCodeInputField.text;
            if (!string.IsNullOrWhiteSpace(code))
            {
                lobbyConnector.JoinLobby(code);
                ActivateLobbyUI(false);
            }
        });

        readyToggle.onValueChanged.AddListener(OnReadyToggled);
        startGameButton.interactable = false;

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }

    private void ActivateLobbyUI(bool isHost)
    {
        lobbyPanel.SetActive(false);
        hostPanel.SetActive(isHost);
        clientPanel.SetActive(!isHost);
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!readyStates.ContainsKey(clientId))
            readyStates[clientId] = false;

        if (!playerUIEntries.ContainsKey(clientId))
        {
            GameObject entry = Instantiate(playerListEntryPrefab, playerListContainer);
            entry.GetComponentInChildren<Text>().text = $"Игрок {clientId} - НЕ ГОТОВ";
            playerUIEntries[clientId] = entry;
        }

        UpdateStartButtonState();
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (playerUIEntries.ContainsKey(clientId))
        {
            Destroy(playerUIEntries[clientId]);
            playerUIEntries.Remove(clientId);
        }

        if (readyStates.ContainsKey(clientId))
        {
            readyStates.Remove(clientId);
        }

        UpdateStartButtonState();
    }

    private void OnReadyToggled(bool isReady)
    {
        ulong localId = NetworkManager.Singleton.LocalClientId;
        readyStates[localId] = isReady;
        UpdatePlayerUI(localId, isReady);
        UpdateStartButtonState();
    }

    private void UpdatePlayerUI(ulong clientId, bool isReady)
    {
        if (playerUIEntries.TryGetValue(clientId, out GameObject entry))
        {
            entry.GetComponentInChildren<Text>().text = $"Игрок {clientId} - {(isReady ? "ГОТОВ" : "НЕ ГОТОВ")}";
        }
    }

    private void UpdateStartButtonState()
    {
        if (!NetworkManager.Singleton.IsHost) return;

        foreach (var kvp in readyStates)
        {
            if (!kvp.Value)
            {
                startGameButton.interactable = false;
                return;
            }
        }

        startGameButton.interactable = true;
    }
}

