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

        // 타겟 여부와 상관없이 일정 속도로 이동
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

            Destroy(gameObject); // 총알 제거 (정상 처리된 경우)
        }
        else if (hitObject.CompareTag("Enemy"))
        {
            Destroy(hitObject);
            Destroy(gameObject); // 총알 제거 (정상 처리된 경우)
        }
        // 그 외의 태그는 무시 (플레이어나 벽, 땅 등)
    }

}
