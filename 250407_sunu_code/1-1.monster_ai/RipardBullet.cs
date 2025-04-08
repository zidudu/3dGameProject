using UnityEngine;

public class StraightBullet : MonoBehaviour
{
    public float lifetime = 3f; // 자동 파괴 시간
    public int damage = 1; // 데미지 양

    void Start()
    {
        Destroy(gameObject, lifetime); // 일정 시간 지나면 자동 제거
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 데미지 처리 예시 (있으면 사용)
            // other.GetComponent<PlayerHealth>()?.TakeDamage(damage);

            Destroy(gameObject); // 충돌 시 파괴
        }
    }
}
