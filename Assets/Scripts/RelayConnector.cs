using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayConnector : MonoBehaviour
{
    /// <summary>
    /// Инициализация Unity Services и авторизация пользователя
    /// </summary>
    public async Task<bool> InitializeUnityServicesAsync()
    {
        try
        {
            await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            Debug.Log("[Relay] Unity Services Initialized and Authenticated");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("[Relay] Init Failed: " + e.Message);
            return false;
        }
    }

    /// <summary>
    /// Создаёт Relay-сервер и возвращает join code
    /// </summary>
    public async Task<string> CreateRelayAsync(int maxConnections = 4)
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            var relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();

            Debug.Log("[Relay] Host started with join code: " + joinCode);
            return joinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError("[Relay] Create Error: " + e.Message);
            return null;
        }
    }

    /// <summary>
    /// Подключается к Relay-серверу по join code
    /// </summary>
    public async Task<bool> StartClientAsync(string joinCode)
    {
        try
        {
            // Проверка, не запущен ли уже клиент или сервер
            if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer)
            {
                Debug.LogWarning("[Relay] Client or Server already running.");
                return false;
            }

            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            var relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();

            Debug.Log("[Relay] Client started and joined with join code: " + joinCode);
            return true;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError("[Relay] Client Join Error: " + e.Message);
            return false;
        }
    }
}