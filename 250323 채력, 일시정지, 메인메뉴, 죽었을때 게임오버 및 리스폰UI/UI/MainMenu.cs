using UnityEngine;
using UnityEngine.SceneManagement; // SceneManager ����Ʈ

public class MainMenu : MonoBehaviour
{
    // Play ��ư Ŭ�� �� ȣ��Ǵ� �Լ�
    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene"); // Play ������ �̵�
    }

    // ���� ���� �Լ� (�ɼ�)
    public void QuitGame()
    {
        Debug.Log("������ �����մϴ�...");
        Application.Quit(); // ���� ����
    }
}
