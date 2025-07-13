using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Threading.Tasks;

public class LobbyConnector : MonoBehaviour
{
    private Lobby currentLobby;
    private string playerId => AuthenticationService.Instance.PlayerId;
    public int maxPlayers = 4;
    public GameObject HostLobbyUI;
    public GameObject ClientLobbyUI;

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    // Создание лобби хостом
    public async void CreateLobby()
    {
        try
        {
            string lobbyName = "Incidents Lobby";
            CreateLobbyOptions options = new CreateLobbyOptions { IsPrivate = false };

            currentLobby = await Lobbies.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

            // Создание Relay-сервера
            Allocation alloc = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(alloc.AllocationId);

            // Обновляем данные лобби (JoinCode для клиентов)
            await Lobbies.Instance.UpdateLobbyAsync(currentLobby.Id, new UpdateLobbyOptions
            {
                Data = new System.Collections.Generic.Dictionary<string, DataObject>
                {
                    {"joinCode", new DataObject(DataObject.VisibilityOptions.Member, joinCode)}
                }
            });

            // Настройка транспорта и запуск хоста
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetHostRelayData(alloc.RelayServer.IpV4, (ushort)alloc.RelayServer.Port, alloc.AllocationIdBytes, alloc.Key, alloc.ConnectionData);
            NetworkManager.Singleton.StartHost();
            HostLobbyUI.SetActive(true);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка создания лобби: {e.Message}");
        }
    }

    // Подключение к существующему лобби по JoinCode
    public async void JoinLobby(string joinCode)
    {
        try
        {
            // Получаем allocation по join-коду
            JoinAllocation joinAlloc = await RelayService.Instance.JoinAllocationAsync(joinCode);

            // Настройка транспорта и запуск клиента
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetClientRelayData(joinAlloc.RelayServer.IpV4, (ushort)joinAlloc.RelayServer.Port, joinAlloc.AllocationIdBytes, joinAlloc.Key, joinAlloc.ConnectionData, joinAlloc.HostConnectionData);
            NetworkManager.Singleton.StartClient();
            ClientLobbyUI.SetActive(true);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка подключения к лобби: {e.Message}");
        }
    }
}
