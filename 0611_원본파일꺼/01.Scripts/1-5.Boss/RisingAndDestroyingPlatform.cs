using UnityEngine;

public class RisingAndDestroyingPlatform : MonoBehaviour
{
    public float speed = 1.5f; // Inspector에서 설정
    public float destroyY = 40f; // 이 위치 이상 올라가면 삭제

    void Update()
    {
        transform.position += Vector3.up * speed * Time.deltaTime;

        if (transform.position.y >= destroyY)
        {
            Destroy(gameObject);
        }
    }
}
