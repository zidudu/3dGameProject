using UnityEngine;
using System.Collections;

public class FirePillar : MonoBehaviour
{
    public float activeDuration = 3f;
    public float damageCooldown = 1f;
    private bool canDamage = true;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && canDamage)
        {
            Debug.Log("�ҿ� ����!");
            StartCoroutine(DamageCooldown());
        }
    }

    private IEnumerator DamageCooldown()
    {
        canDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        canDamage = true;
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        StartCoroutine(DeactivateAfterTime());
    }

    private IEnumerator DeactivateAfterTime()
    {
        yield return new WaitForSeconds(activeDuration);
        Destroy(gameObject); // �ұ�� ����
    }


    //  ������ �ұ�� �׵θ� ǥ��
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // ����� ���� ����
        Collider col = GetComponent<Collider>();

        if (col is BoxCollider box) // BoxCollider�� ���
        {
            Gizmos.DrawWireCube(transform.position + box.center, box.size);
        }
        else if (col is CapsuleCollider capsule) // CapsuleCollider�� ���
        {
            Gizmos.DrawWireSphere(transform.position, capsule.radius);
        }
    }
}
