using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayConnector : MonoBehaviour
{
    public async Task<string> StartHostAsync()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4); // до 4 игроков
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log("[Relay] Join Code: " + joinCode);

            var relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();

            return joinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError("Relay Host Error: " + e.Message);
            return null;
        }
    }

    public async Task<bool> StartClientAsync(string joinCode)
    {
        try
        {
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            var relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient();

            return true;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError("Relay Client Error: " + e.Message);
            return false;
        }
    }
}

