using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class LobbyConnector : MonoBehaviour
{
    private Lobby _lobby;
    private string _joinCode;

    public string JoinCode => _joinCode; // добавили публичный доступ

    [SerializeField] private int maxPlayers = 4;
    [SerializeField] private string lobbyName = "Incidents Lobby";

    public async void CreateLobby()
    {
        try
        {
            // Создаем выделение сервера в Relay
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers - 1);
            _joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            // Создаем лобби и сохраняем JoinCode в Data
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = false,
                Data = new Dictionary<string, DataObject>
                {
                    { "joinCode", new DataObject(DataObject.VisibilityOptions.Public, _joinCode) }
                }
            };

            _lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            Debug.Log($"Создано лобби с кодом: {_lobby.LobbyCode}, JoinCode для Relay: {_joinCode}");

            // Запускаем хост
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData);

            NetworkManager.Singleton.StartHost();
        }
        catch (Exception e)
        {
            Debug.LogError($"Ошибка создания лобби: {e.Message}");
        }
    }

    public async void JoinLobby(string lobbyCode)
    {
        try
        {
            // Подключаемся к лобби
            _lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

            string joinCode = _lobby.Data["joinCode"].Value;

            // Получаем данные Relay
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData);

            NetworkManager.Singleton.StartClient();
        }
        catch (Exception e)
        {
            Debug.LogError($"Ошибка подключения к лобби: {e.Message}");
        }
    }
}

