using UnityEngine;
using UnityEngine.UI;

public class KeyInventory : MonoBehaviour
{
    [Header("�̱���")]
    public static KeyInventory instance;

    [Header("���� UI")]
    [Tooltip("���� ������ŭ �� ������(Image) �迭")]
    public Image[] keyIcons;          // inspector�� 4�� �巡��

    private int keyCount = 0;         // ���� ���� ����

    private void Awake()
    {
        if (instance == null) instance = this;
        else { Destroy(gameObject); return; }
    }

    /// <summary> ���踦 1�� ȹ���� �� ȣ�� </summary>
    public void AddKey()
    {
        keyCount = Mathf.Clamp(keyCount + 1, 0, keyIcons.Length);
        RefreshUI();
    }

    /// <summary> ���� ���� 1�� ���� ���� �� ȣ�� </summary>
    public bool ConsumeKey()
    {
        if (keyCount <= 0) return false;
        keyCount--;
        RefreshUI();
        return true;
    }

    public bool HasAllKeys() => keyCount >= keyIcons.Length;
    public bool HasKey() => keyCount > 0;

    private void RefreshUI()
    {
        for (int i = 0; i < keyIcons.Length; i++)
            keyIcons[i].color = i < keyCount ? Color.white : Color.gray;  // ä��/��� ǥ��
    }
}
