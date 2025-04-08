using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float rotateSpeed = 720f;
    public float destroyTime = 1.5f;

    private Rigidbody rb;
    private Transform target;

    public void SetTarget(Transform t)
    {
        target = t;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, destroyTime);
    }

    private void FixedUpdate()
    {
        Vector3 direction;

        if (target != null)
        {
            direction = (target.position - transform.position).normalized;
            Quaternion look = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, look, rotateSpeed * Time.fixedDeltaTime);
        }

        // Ÿ�� ���ο� ������� ���� �ӵ��� �̵�
        rb.velocity = transform.forward * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject hitObject = collision.gameObject;

        if (hitObject.CompareTag("BreakableBlock"))
        {
            BreakableObject breakable = hitObject.GetComponent<BreakableObject>();
            if (breakable != null)
            {
                breakable.Break();
            }
            else
            {
                Destroy(hitObject);
            }

            Destroy(gameObject); // �Ѿ� ���� (���� ó���� ���)
        }
        else if (hitObject.CompareTag("Enemy"))
        {
            Destroy(hitObject);
            Destroy(gameObject); // �Ѿ� ���� (���� ó���� ���)
        }
        // �� ���� �±״� ���� (�÷��̾ ��, �� ��)
    }

}
