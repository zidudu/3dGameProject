using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class WarpPoint : MonoBehaviour
{
    public string warpID = "default"; // ���� ���� ID
    public string warpName = "Unnamed"; // ���� �̸�
    public string sceneName = ""; // ������ ���� �� �̸�

    private bool playerInRange = false; // �÷��̾ ���� ���� �ִ��� ����
    private PlayerControls controls; // �Է� �ý��� ����

    private void Awake()
    {
        if (string.IsNullOrEmpty(sceneName)) // �� �̸��� ���������
        {
            sceneName = SceneManager.GetActiveScene().name; // ���� �� �̸� ����
        }

        controls = new PlayerControls(); // �Է� ��Ʈ�� �ʱ�ȭ
        controls.Gameplay.Interact.performed += ctx => TryOpenWarpUI(); // Interact �Է¿� UI ���� ����
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable(); // �Է� Ȱ��ȭ
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable(); // �Է� ��Ȱ��ȭ
    }

    private void Start()
    {
        WarpPointData data = new WarpPointData(warpID, warpName, sceneName, transform.position); // ���� ������ ����
        GameManager.instance.RegisterWarpPointData(data); // ���̺�� ������ ���
        if (GameManager.instance.IsWarpUnlocked(warpID)) // ������ �رݵ� ���
        {
            WarpManager.instance.RegisterWarpPoint(data); // UI�� ���� ���
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return; // �÷��̾ �ƴϸ� ����

        playerInRange = true; // ���� ���� ����
        WarpPointData data = new WarpPointData(warpID, warpName, sceneName, transform.position); // ���� ������ ����

        if (!GameManager.instance.IsWarpUnlocked(warpID)) // ó�� �رݵǴ� ���
        {
            GameManager.instance.UnlockWarp(warpID); // ���� �ر�
        }

        WarpManager.instance.RegisterWarpPoint(data); // UI ���ſ� ���� ���
        GameManager.instance.SaveCheckpoint(transform.position); // üũ����Ʈ ����
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // �÷��̾ ������
        {
            playerInRange = false; // ���� �÷��� ����
        }
    }

    private void TryOpenWarpUI()
    {
        if (playerInRange && WarpManager.instance != null) // ���� ���̰� ���� �Ŵ��� �����ϸ�
        {
            WarpManager.instance.OpenWarpUI(); // ���� UI ����
        }
    }
}
