using UnityEngine;
using UnityEngine.SceneManagement;

public enum NetworkRole
{
    Host,
    Client
}

public class MainMenuUI : MonoBehaviour
{
    public static NetworkRole ChosenRole; // сохранится при переходе в сцену Lobby

    public void OnClickHost()
    {
        ChosenRole = NetworkRole.Host;
        SceneManager.LoadScene(1);
    }

    public void OnClickClient()
    {
        ChosenRole = NetworkRole.Client;
        SceneManager.LoadScene(1);
    }
}
