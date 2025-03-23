using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    // �ٸ� ��ũ��Ʈ���� ���� ������ �����ϵ��� static
    public static bool GameIsPaused = false;
    public GameObject pauseMenuCanvas;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause()
    {
        pauseMenuCanvas.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void ToSettingMenu()
    {
        Debug.Log("���� �̱����Դϴ�...");
    }

    // ���� �޴��� �̵��ϴ� �Լ�
    public void ToMain()
    {
        Debug.Log("���� �޴��� �̵��մϴ�...");
        Time.timeScale = 1f;  // ���� �ð� ����ȭ (���� �޴��� ���ư� ��)
        SceneManager.LoadScene("Main");  // MainMenu ������ �̵�
    }

    // ���� �����ϴ� �Լ�
    public void QuitGame()
    {
        Debug.Log("������ �����մϴ�...");
        Application.Quit();  // ���� ����
    }
}
