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

    // ─────────────────────────────────────────────────────────────────────────
    // [추가] 버튼에서 직접 호출할 메서드
    // ─────────────────────────────────────────────────────────────────────────
    public void OnClick_Retry()
    {
        // 1) 시간 다시 흐르게
        Time.timeScale = 1;

        // 2) 플레이어 찾기
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // 플레이어를 세이브포인트로 이동
            RespawnPlayer(player);

            // 여기서 체력 회복 로직 추가
            Health healthScript = player.GetComponent<Health>();
            if (healthScript != null)
            {
                //체력리셋 함수
                //healthScript.ResetHealth();

                // 예) 최대 체력만큼 회복하고 UI 갱신
                healthScript.health = healthScript.numOfHearts; // 하트 수만큼 체력 회복
                                                                // 혹은 healthScript.health = healthScript.numOfHearts * 2; // 1하트=2체력이면 이렇게

                // 화면 다시 밝게/UI 리셋
                healthScript.fadeCanvasGroup.alpha = 0f;
                healthScript.gameOverUI.SetActive(false);

                // 체력 UI 즉시 갱신
                healthScript.UpdateHealthUI();
            }
        }

        Debug.Log("세이브 포인트로 이동 완료 & 체력 초기화");
    }
    public void OnClick_MainMenu()
    {

        // 시간 복구
        Time.timeScale = 1;
        // 씬 전환 등 구현
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
        Debug.Log("메인 메뉴로 이동");
    }
}
