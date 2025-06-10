using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    public GameObject platformPrefab;
    public float spawnInterval = 0.5f; // 몇 초마다 생성할지

    private float timer = 0f;

    void Start()
    {
        Instantiate(platformPrefab, transform.position, Quaternion.identity); // 시작 즉시 1개 생성
    }


    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            Instantiate(platformPrefab, transform.position, Quaternion.identity); // 프리팹에 설정된 값 사용
            timer = 0f;
        }
    }
}
