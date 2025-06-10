using UnityEngine;

public class Meteor : MonoBehaviour
{
    public float fallSpeed = 10f; // ���� �ӵ� (Inspector���� ���� ����)
    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
    void Update()
    {
        // �� ������ y�� �������� �����ϰ� �̵�
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Platform") || other.CompareTag("Ground"))
        {
            Destroy(gameObject); // �ٴڿ� ������ ����
        }
        else if (other.CompareTag("Player"))
        {
            Debug.Log("�÷��̾ ���׿��� ����!");
            Destroy(gameObject);
        }
    }
}
