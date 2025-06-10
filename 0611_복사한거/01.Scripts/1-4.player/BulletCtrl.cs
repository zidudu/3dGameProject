using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float rotateSpeed = 720f;
    public float destroyTime = 1.5f;
    public float damage = 10f;

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

        rb.velocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 깨지는 블록은 OnTrigger에도 대응해야 할 경우
        if (other.CompareTag("BreakableBlock"))
        {
            BreakableObject breakable = other.GetComponent<BreakableObject>();
            if (breakable != null) breakable.Break();
            else Destroy(other.gameObject);

            Destroy(gameObject);
            return;
        }

        // 적 또는 보스에 명중하면 데미지
        if (other.CompareTag("Enemy") || other.CompareTag("Boss"))
        {
            other.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
            Destroy(gameObject);
        }
    }
}
