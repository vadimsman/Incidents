using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class IncidentManager : NetworkBehaviour
{
    // Синглтон для глобального доступа
    public static IncidentManager Instance;

    [Tooltip("Названия сцен инцидентов (без расширения .unity)")]
    public List<string> allIncidentScenes = new List<string>();

    // Список уже сыгранных инцидентов (сброс при поражении)
    private List<string> completedIncidents = new List<string>();

    private void Awake()
    {
        // Установка синглтона
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Попытка загрузить случайный новый инцидент
    public void TryLoadRandomIncident()
    {
        if (!IsServer) return; // Только сервер управляет сценами

        List<string> available = new List<string>(allIncidentScenes);
        foreach (string played in completedIncidents)
        {
            available.Remove(played); // Убираем уже сыгранные
        }

        // Если закончились инциденты, сбрасываем
        if (available.Count == 0)
        {
            Debug.Log("Все инциденты сыграны. Сброс...");
            completedIncidents.Clear();
            available = new List<string>(allIncidentScenes);
        }

        // Выбираем случайный инцидент
        string selectedScene = available[Random.Range(0, available.Count)];
        completedIncidents.Add(selectedScene);

        // Загружаем сцену как аддон (additive), чтобы не сбросить игроков
        Debug.Log($"Загружается инцидент: {selectedScene}");
        NetworkManager.Singleton.SceneManager.LoadScene(selectedScene, LoadSceneMode.Additive);
    }

    // Сброс прогресса вручную (например, при поражении)
    public void ResetCompletedIncidents()
    {
        if (!IsServer) return;

        Debug.Log("Инциденты сброшены вручную.");
        completedIncidents.Clear();
    }
}

