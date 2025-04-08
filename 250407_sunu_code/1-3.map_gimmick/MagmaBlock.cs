using System.Collections;
using UnityEngine;

public class MagmaBlock : MonoBehaviour
{
    public float damageInterval = 0.5f; // �������� �޴� ����
    private bool isPlayerOn = false; // �÷��̾ ���� �ִ��� Ȯ��

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOn = true;
            StartCoroutine(DamagePlayer());
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOn = false;
        }
    }

    private IEnumerator DamagePlayer()
    {
        while (isPlayerOn)
        {
            Debug.Log("���׸� ��� ���� ���� ������!");
            yield return new WaitForSeconds(damageInterval);
        }
    }
}
