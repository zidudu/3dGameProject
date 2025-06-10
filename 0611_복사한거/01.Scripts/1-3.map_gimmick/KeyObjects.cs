using UnityEngine;

public class KeyObject : MonoBehaviour
{
    [Header("열쇠 설정")]
    [Tooltip("저장용 고유 ID (씬당 유일)")]
    public string keyID = "Key_001";

    private void Start()
    {
        if (GameManager.instance.IsObjectDestroyed(keyID))
            Destroy(gameObject);        // 이미 먹은 열쇠는 숨김
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // ① 세이브에 ‘파괴됨’ 기록
        GameManager.instance.RegisterDestroyedObject(keyID);

        // ② 인벤토리에 열쇠 추가
        KeyInventory.instance.AddKey();

        // ③ 열쇠 오브젝트 제거
        Destroy(gameObject);
    }
}
