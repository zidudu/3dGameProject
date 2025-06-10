using UnityEngine;
using UnityEngine.UI;

public class KeyInventory : MonoBehaviour
{
    [Header("싱글톤")]
    public static KeyInventory instance;

    [Header("열쇠 UI")]
    [Tooltip("열쇠 개수만큼 원 아이콘(Image) 배열")]
    public Image[] keyIcons;          // inspector에 4개 드래그

    private int keyCount = 0;         // 현재 보유 열쇠

    private void Awake()
    {
        if (instance == null) instance = this;
        else { Destroy(gameObject); return; }
    }

    /// <summary> 열쇠를 1개 획득할 때 호출 </summary>
    public void AddKey()
    {
        keyCount = Mathf.Clamp(keyCount + 1, 0, keyIcons.Length);
        RefreshUI();
    }

    /// <summary> 문에 열쇠 1개 삽입 성공 시 호출 </summary>
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
            keyIcons[i].color = i < keyCount ? Color.white : Color.gray;  // 채움/비움 표시
    }
}
