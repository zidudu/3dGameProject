using System.Collections;
using UnityEngine;

public class FirePillarSpawner : MonoBehaviour
{
    public GameObject firePillarPrefab; // �ұ�� ������
    public float spawnInterval = 5f; // �ұ�� ���� ����
    public Transform spawnPoint; // �ұ���� ������ ��ġ

    void Start()
    {
        StartCoroutine(SpawnFirePillar());
    }

    private IEnumerator SpawnFirePillar()
    {
        while (true)
        {
            GameObject firePillar = Instantiate(firePillarPrefab, spawnPoint.position, Quaternion.identity);
            firePillar.GetComponent<FirePillar>().Activate();

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
