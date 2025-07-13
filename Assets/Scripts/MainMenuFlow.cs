using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuFlow : MonoBehaviour
{
    public GameObject lobbyUI;
    public GameObject MainMenuUI;

    // Название первой сюжетной сцены
    public string firstLevelSceneName = "Level_01";

    // Кнопка "Играть одному"
    public void OnClickSingleplayer()
    {
        SceneManager.LoadScene(firstLevelSceneName);
    }

    // Кнопка "Играть с друзьями"
    public void OnClickMultiplayer()
    {
        lobbyUI.SetActive(true); // Показываем UI лобби на той же сцене
        MainMenuUI.SetActive(false);
    }

    // Кнопка "Начать игру" из UI лобби (вызывается хостом)
    public void OnClickStartGameMultiplayer()
    {
        if (Unity.Netcode.NetworkManager.Singleton.IsServer)
        {
            Unity.Netcode.NetworkManager.Singleton.SceneManager.LoadScene(firstLevelSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }

    // Кнопка "Выйти из игры"
    public void OnClickExit()
    {
        Application.Quit();
    }
}

