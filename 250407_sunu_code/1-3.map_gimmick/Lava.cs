using UnityEngine;
using System.Collections;

public class Lava : MonoBehaviour
{
    private void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Lava�� Rigidbody�� �����ϴ�! Rigidbody�� �߰��ϼ���.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("��Ͽ� ����!");
            Destroy(gameObject); // �÷��̾ ��Ƶ� ����
        }
        else if (other.CompareTag("Ground"))
        {
            Destroy(gameObject); // �ٴڿ� ������ ����
        }
    }
}
