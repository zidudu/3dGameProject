using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // ���� �÷��̾��� Transform
    public Vector3 offset = new Vector3(0, 2, -10); // ī�޶� ��ġ ������
    public float smoothSpeed = 5f; // �ε巯�� �̵� �ӵ�

    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 targetPosition = player.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        }
    }
}
