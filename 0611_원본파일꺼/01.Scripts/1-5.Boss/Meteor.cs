using UnityEngine;

public class Meteor : MonoBehaviour
{
    public float fallSpeed = 10f; // 낙하 속도 (Inspector에서 조절 가능)
    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
    void Update()
    {
        // 매 프레임 y축 방향으로 일정하게 이동
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Platform") || other.CompareTag("Ground"))
        {
            Destroy(gameObject); // 바닥에 닿으면 제거
        }
        else if (other.CompareTag("Player"))
        {
            Debug.Log("플레이어가 메테오에 맞음!");
            Destroy(gameObject);
        }
    }
}
