using System.Collections;
using UnityEngine;

public class LavaSpawner : MonoBehaviour
{
    public GameObject lavaPrefab; // ��� ������
    public float spawnInterval = 5f; // ��� ���� ����
    public Transform spawnPoint; // ����� ������ ��ġ

    void Start()
    {
        StartCoroutine(SpawnLava());
    }

    private IEnumerator SpawnLava()
    {
        while (true)
        {
            GameObject lava = Instantiate(lavaPrefab, spawnPoint.position, Quaternion.identity);
            Destroy(lava, 10f); // ����� ���� �ð� �� �ڵ� ���� (�� ������)
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
