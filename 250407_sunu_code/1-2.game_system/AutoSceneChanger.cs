using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoSceneChange : MonoBehaviour
{
    public string targetSceneName = "Stage2";
    public Vector3 targetPositionInNewScene = new Vector3(0, 1, 0); // Stage2���� �÷��̾� ���� ��ġ

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ��ġ ���
            GameManager.instance.SetPendingWarp(
                new WarpPointData("testWarp", "Stage2Test", targetSceneName, targetPositionInNewScene)
            );
            if (WarpManager.instance != null && WarpManager.instance.isOpen)
            {
                WarpManager.instance.CloseWarpUI();
            }

            // �� ��ȯ
            SceneManager.LoadScene(targetSceneName);
        }
    }
}
