using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Platform"))
        {
            Destroy(gameObject); // �÷��� �浹 �� ����
        }
        else if (other.CompareTag("Player"))
        {
            Debug.Log("�÷��̾�� ����!"); // ������ �ִ� �ý����� ���� �߰�
            Destroy(gameObject);
        }
    }

}
