using UnityEngine;

public class OneWayPlatform3D : MonoBehaviour
{
    private Collider platformCollider; // �÷��� �ڽ��� �ݶ��̴�
    private Transform player; // �÷��̾� Transform
    private Collider[] playerColliders; // �÷��̾ ���Ե� ��� �ݶ��̴� �迭

    public float verticalOffset = 0.1f; // �÷��̾� �߽ɿ��� �� ��ġ ������

    void Start()
    {
        platformCollider = GetComponent<Collider>(); // �ڽ��� �ݶ��̴� ������

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player"); // �±׷� �÷��̾� ã��
        if (playerObj != null)
        {
            player = playerObj.transform; // �÷��̾� Transform ����
            playerColliders = playerObj.GetComponentsInChildren<Collider>(); // �ڽ� ���� ��� �ݶ��̴� ������
        }
    }

    void Update()
    {
        if (player == null || platformCollider == null || playerColliders == null) return; // ��ȿ�� üũ

        // �÷��̾� ��ġ�� �÷������� �Ʒ��� ������ �浹 ����
        bool shouldIgnore = (player.position.y + verticalOffset) < transform.position.y;

        foreach (var pCol in playerColliders)
        {
            Physics.IgnoreCollision(pCol, platformCollider, shouldIgnore); // �浹 ���� ����
        }
    }
}
