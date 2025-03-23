using UnityEngine;
using UnityEngine.SceneManagement; // SceneManager 임포트

public class MainMenu : MonoBehaviour
{
    // Play 버튼 클릭 시 호출되는 함수
    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene"); // Play 씬으로 이동
    }

    // 게임 종료 함수 (옵션)
    public void QuitGame()
    {
        Debug.Log("게임을 종료합니다...");
        Application.Quit(); // 게임 종료
    }
}
