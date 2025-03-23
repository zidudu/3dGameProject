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

        Debug.Log("���̺� ����Ʈ �����: " + position);
        Debug.Log("JSON ���� ���: " + saveFilePath);
    }

    private void LoadCheckpoint()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            respawnPosition = new Vector3(data.respawnX, data.respawnY, data.respawnZ);
            Debug.Log("����� ���̺� ����Ʈ �ҷ�����: " + respawnPosition);
        }
        else
        {
            respawnPosition = Vector3.zero; // ���� ù ���� �� �⺻ ��ġ
            Debug.Log("�⺻ ��ġ�� ������: " + respawnPosition);
        }
    }

    public void RespawnPlayer(GameObject player)
    {
        player.transform.position = respawnPosition;
    }

    // ��������������������������������������������������������������������������������������������������������������������������������������������������
    // [�߰�] ��ư���� ���� ȣ���� �޼���
    // ��������������������������������������������������������������������������������������������������������������������������������������������������
    public void OnClick_Retry()
    {
        // 1) �ð� �ٽ� �帣��
        Time.timeScale = 1;

        // 2) �÷��̾� ã��
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // �÷��̾ ���̺�����Ʈ�� �̵�
            RespawnPlayer(player);

            // ���⼭ ü�� ȸ�� ���� �߰�
            Health healthScript = player.GetComponent<Health>();
            if (healthScript != null)
            {
                //ü�¸��� �Լ�
                //healthScript.ResetHealth();

                // ��) �ִ� ü�¸�ŭ ȸ���ϰ� UI ����
                healthScript.health = healthScript.numOfHearts; // ��Ʈ ����ŭ ü�� ȸ��
                                                                // Ȥ�� healthScript.health = healthScript.numOfHearts * 2; // 1��Ʈ=2ü���̸� �̷���

                // ȭ�� �ٽ� ���/UI ����
                healthScript.fadeCanvasGroup.alpha = 0f;
                healthScript.gameOverUI.SetActive(false);

                // ü�� UI ��� ����
                healthScript.UpdateHealthUI();
            }
        }

        Debug.Log("���̺� ����Ʈ�� �̵� �Ϸ� & ü�� �ʱ�ȭ");
    }
    public void OnClick_MainMenu()
    {

        // �ð� ����
        Time.timeScale = 1;
        // �� ��ȯ �� ����
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
        Debug.Log("���� �޴��� �̵�");
    }
}
