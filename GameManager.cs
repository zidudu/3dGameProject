using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Vector3 respawnPosition;
    private string saveFilePath;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        saveFilePath = Application.persistentDataPath + "/saveData.json";
        LoadCheckpoint();
    }

    public void SaveCheckpoint(Vector3 position)
    {
        respawnPosition = position;
        SaveData data = new SaveData(position);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);

        Debug.Log("세이브 포인트 저장됨: " + position);
        Debug.Log("JSON 저장 경로: " + saveFilePath);
    }

    private void LoadCheckpoint()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            respawnPosition = new Vector3(data.respawnX, data.respawnY, data.respawnZ);
            Debug.Log("저장된 세이브 포인트 불러오기: " + respawnPosition);
        }
        else
        {
            respawnPosition = Vector3.zero; // 게임 첫 실행 시 기본 위치
            Debug.Log("기본 위치로 설정됨: " + respawnPosition);
        }
    }

    public void RespawnPlayer(GameObject player)
    {
        player.transform.position = respawnPosition;
    }
}
