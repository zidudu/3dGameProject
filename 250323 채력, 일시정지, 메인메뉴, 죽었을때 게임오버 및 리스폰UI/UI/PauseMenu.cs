using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    // 다른 스크립트에서 쉽게 접근이 가능하도록 static
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
        Debug.Log("아직 미구현입니다...");
    }

    // 메인 메뉴로 이동하는 함수
    public void ToMain()
    {
        Debug.Log("메인 메뉴로 이동합니다...");
        Time.timeScale = 1f;  // 게임 시간 정상화 (메인 메뉴로 돌아갈 때)
        SceneManager.LoadScene("Main");  // MainMenu 씬으로 이동
    }

    // 게임 종료하는 함수
    public void QuitGame()
    {
        Debug.Log("게임을 종료합니다...");
        Application.Quit();  // 게임 종료
    }
}
