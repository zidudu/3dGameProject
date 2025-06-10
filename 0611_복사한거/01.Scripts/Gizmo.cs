using UnityEngine;

public class LadderGizmos : MonoBehaviour
{
    public float width = 1f;  // ��ٸ� �ʺ�
    public float height = 5f; // ��ٸ� ����
    public Color gizmoColor = Color.green;

    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        // �ڽ� ���·� ��ٸ� ���� ǥ��
        Gizmos.DrawWireCube(transform.position, new Vector3(width, height, 0.1f));
    }
}
