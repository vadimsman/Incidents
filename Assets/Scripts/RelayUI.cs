using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class RelayUI : MonoBehaviour
{
    public TMP_InputField joinCodeInput;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI statusTextLobby;
    public GameObject CameraUI;
    public GameObject StatusLobbyUI;

    private RelayConnector relayConnector;

    void Start()
    {
        relayConnector = FindObjectOfType<RelayConnector>();
    }

    public async void OnClickCreateHost()
    {
        SetStatus("�������� �����...");

        string joinCode = await relayConnector.StartHostAsync();
        Debug.Log("[UI] Join Code: " + joinCode);

        if (!string.IsNullOrEmpty(joinCode))
        {
            SetStatus($"Join Code: {joinCode}");
            await Task.Delay(1500);
            SwitchToLobbyUI(joinCode);
        }
        else
        {
            SetStatus("������ �������� �����.");
        }
    }

    public async void OnClickJoinClient()
    {
        string code = joinCodeInput.text.Trim();
        if (string.IsNullOrEmpty(code))
        {
            SetStatus("������� Join Code!");
            return;
        }

        SetStatus("�����������...");
        bool success = await relayConnector.StartClientAsync(code);

        if (success)
        {
            SetStatus("������� ����������!");
            await Task.Delay(1500);
            CameraUI.SetActive(false);
        }
        else
        {
            SetStatus("������ �����������.");
        }
    }

    private void SetStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
        Debug.Log("[Status] " + message);
    }

    private void SwitchToLobbyUI(string joinCode)
    {
        CameraUI.SetActive(false);
        StatusLobbyUI.SetActive(true);
        if (statusTextLobby != null)
            statusTextLobby.text = $"Join Code: {joinCode}";
    }
}

