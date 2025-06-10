using UnityEngine;

public class KeyObject : MonoBehaviour
{
    [Header("���� ����")]
    [Tooltip("����� ���� ID (���� ����)")]
    public string keyID = "Key_001";

    private void Start()
    {
        if (GameManager.instance.IsObjectDestroyed(keyID))
            Destroy(gameObject);        // �̹� ���� ����� ����
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // �� ���̺꿡 ���ı��ʡ� ���
        GameManager.instance.RegisterDestroyedObject(keyID);

        // �� �κ��丮�� ���� �߰�
        KeyInventory.instance.AddKey();

        // �� ���� ������Ʈ ����
        Destroy(gameObject);
    }
}
