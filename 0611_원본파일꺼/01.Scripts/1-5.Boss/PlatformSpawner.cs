using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    public GameObject platformPrefab;
    public float spawnInterval = 0.5f; // �� �ʸ��� ��������

    private float timer = 0f;

    void Start()
    {
        Instantiate(platformPrefab, transform.position, Quaternion.identity); // ���� ��� 1�� ����
    }


    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            Instantiate(platformPrefab, transform.position, Quaternion.identity); // �����տ� ������ �� ���
            timer = 0f;
        }
    }
}
