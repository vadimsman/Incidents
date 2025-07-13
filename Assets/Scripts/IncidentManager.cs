using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class IncidentManager : NetworkBehaviour
{
    // �������� ��� ����������� �������
    public static IncidentManager Instance;

    [Tooltip("�������� ���� ���������� (��� ���������� .unity)")]
    public List<string> allIncidentScenes = new List<string>();

    // ������ ��� ��������� ���������� (����� ��� ���������)
    private List<string> completedIncidents = new List<string>();

    private void Awake()
    {
        // ��������� ���������
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // ������� ��������� ��������� ����� ��������
    public void TryLoadRandomIncident()
    {
        if (!IsServer) return; // ������ ������ ��������� �������

        List<string> available = new List<string>(allIncidentScenes);
        foreach (string played in completedIncidents)
        {
            available.Remove(played); // ������� ��� ���������
        }

        // ���� ����������� ���������, ����������
        if (available.Count == 0)
        {
            Debug.Log("��� ��������� �������. �����...");
            completedIncidents.Clear();
            available = new List<string>(allIncidentScenes);
        }

        // �������� ��������� ��������
        string selectedScene = available[Random.Range(0, available.Count)];
        completedIncidents.Add(selectedScene);

        // ��������� ����� ��� ����� (additive), ����� �� �������� �������
        Debug.Log($"����������� ��������: {selectedScene}");
        NetworkManager.Singleton.SceneManager.LoadScene(selectedScene, LoadSceneMode.Additive);
    }

    // ����� ��������� ������� (��������, ��� ���������)
    public void ResetCompletedIncidents()
    {
        if (!IsServer) return;

        Debug.Log("��������� �������� �������.");
        completedIncidents.Clear();
    }
}

