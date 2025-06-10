using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Platform"))
        {
            Destroy(gameObject); // 플랫폼 충돌 시 제거
        }
        else if (other.CompareTag("Player"))
        {
            Debug.Log("플레이어에게 명중!"); // 데미지 주는 시스템은 추후 추가
            Destroy(gameObject);
        }
    }

}
