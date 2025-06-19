using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        statusText.text = "�������� �����...";

        string joinCode = await relayConnector.StartHostAsync();
        Debug.Log("[UI] Join Code: " + joinCode);

        if (!string.IsNullOrEmpty(joinCode))
        {
            statusText.text = $"Join Code: {joinCode}";
            await Task.Delay(1500);
            CameraUI.SetActive(false);
            StatusLobbyUI.SetActive(true);
            statusTextLobby.text = $"Join Code: {joinCode}";
        }
        else
        {
            statusText.text = "������ �������� �����.";
        }
    }


    public async void OnClickJoinClient()
    {
        string code = joinCodeInput.text.Trim();
        if (string.IsNullOrEmpty(code))
        {
            statusText.text = "������� Join Code!";
            return;
        }

        statusText.text = "�����������...";
        bool success = await relayConnector.StartClientAsync(code);

        if (success)
        {
            statusText.text = "������� ����������!";

            // ��������� ��������, ����� ������������ ������ ���������
            await Task.Delay(1500);
            CameraUI.SetActive(false);
        }
        else
        {
            statusText.text = "������ �����������.";
        }
    }

}

