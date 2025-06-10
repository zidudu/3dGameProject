using UnityEngine;

public class SlopePhysics : MonoBehaviour
{
    public float slideSpeed = 2.0f; // �̲������� �ӵ� ���� ����
    public float slopeAngleThreshold = 10f; // �̲����� ���� �ּ� ����

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // **ȸ�� ����**
    }

    void OnCollisionStay(Collision collision)
    {
        Vector3 normal = collision.contacts[0].normal;
        float slopeAngle = Vector3.Angle(normal, Vector3.up);

        if (slopeAngle > slopeAngleThreshold)
        {
            Vector3 slideDirection = new Vector3(normal.x, -normal.y, 0).normalized;
            rb.velocity = new Vector3(slideDirection.x * slideSpeed, rb.velocity.y, 0);
            // **Y�� �ӵ� ���� �� �߷��� ���� �۵�**
        }
    }
}
