using UnityEngine;

public class StraightBullet : MonoBehaviour
{
    public float lifetime = 3f; // �ڵ� �ı� �ð�
    public int damage = 1; // ������ ��

    void Start()
    {
        Destroy(gameObject, lifetime); // ���� �ð� ������ �ڵ� ����
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ������ ó�� ���� (������ ���)
            // other.GetComponent<PlayerHealth>()?.TakeDamage(damage);

            Destroy(gameObject); // �浹 �� �ı�
        }
    }
}
