using System.Collections;
using UnityEngine;

public class BreakableBlock : MonoBehaviour
{
    public float disappearTime = 1.5f; // ����� ���������� �ɸ��� �ð�
    public float reappearTime = 3.0f; // ����� �ٽ� �����Ǳ���� �ɸ��� �ð�

    private Renderer blockRenderer;
    private Collider blockCollider;
    private Animator animator;

    void Start()
    {
        blockRenderer = GetComponent<Renderer>();
        blockCollider = GetComponent<Collider>();
        animator = GetComponent<Animator>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) // �÷��̾ ������ Ÿ�̸� ����
        {
            StartCoroutine(BreakBlock());
        }
    }

    private IEnumerator BreakBlock()
    {
        animator.SetTrigger("Shake"); // �ִϸ��̼� ���� (Idle �� ShakeBlock)
        yield return new WaitForSeconds(disappearTime);

        blockRenderer.enabled = false;
        blockCollider.enabled = false;

        animator.Play("Idle"); // �ִϸ��̼��� Idle ���·� ���� ��ȯ

        yield return new WaitForSeconds(reappearTime);

        blockRenderer.enabled = true;
        blockCollider.enabled = true;
    }

}
