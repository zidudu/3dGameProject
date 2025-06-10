using UnityEngine;

public class RisingAndDestroyingPlatform : MonoBehaviour
{
    public float speed = 1.5f; // Inspector���� ����
    public float destroyY = 40f; // �� ��ġ �̻� �ö󰡸� ����

    void Update()
    {
        transform.position += Vector3.up * speed * Time.deltaTime;

        if (transform.position.y >= destroyY)
        {
            Destroy(gameObject);
        }
    }
}
