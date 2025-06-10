using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform pointA; // �̵��� ���� ����
    public Transform pointB; // �̵��� �� ����
    public float speed = 3f; // �̵� �ӵ�

    private Vector3 targetPosition;

    private void Start()
    {
        targetPosition = pointB.position; // ó������ pointB�� �̵�
    }

    private void Update()
    {
        // �÷����� ��ǥ ��ġ�� �̵�
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // ��ǥ ��ġ�� �����ϸ� �ݴ� �������� �̵�
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            targetPosition = (targetPosition == pointA.position) ? pointB.position : pointA.position;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform); // �÷��̾ �÷����� �ڽ����� ����
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null); // �÷��̾ ������ �θ� ���� ����
        }
    }
}
