using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuFlow : MonoBehaviour
{
    public GameObject lobbyUI;
    public GameObject MainMenuUI;

    // �������� ������ �������� �����
    public string firstLevelSceneName = "Level_01";

    // ������ "������ ������"
    public void OnClickSingleplayer()
    {
        SceneManager.LoadScene(firstLevelSceneName);
    }

    // ������ "������ � ��������"
    public void OnClickMultiplayer()
    {
        lobbyUI.SetActive(true); // ���������� UI ����� �� ��� �� �����
        MainMenuUI.SetActive(false);
    }

    // ������ "������ ����" �� UI ����� (���������� ������)
    public void OnClickStartGameMultiplayer()
    {
        if (Unity.Netcode.NetworkManager.Singleton.IsServer)
        {
            Unity.Netcode.NetworkManager.Singleton.SceneManager.LoadScene(firstLevelSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }

    // ������ "����� �� ����"
    public void OnClickExit()
    {
        Application.Quit();
    }
}

