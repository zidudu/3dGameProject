using UnityEngine;
using System.Collections;

public class FirePillarSpawner : MonoBehaviour
//스포너라서 나중에 배열로 바꿔야함


{
    public GameObject firePillarPrefab;
    public float spawnInterval = 5f; // 5초 간격으로 스폰
    
    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            GameObject pillar = Instantiate(firePillarPrefab, transform.position, Quaternion.identity);
            // 필요한 경우, 일정 시간 후 제거하는 로직 추가
            Destroy(pillar, 2f);
        }
    }
}
