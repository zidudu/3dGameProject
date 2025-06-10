using UnityEngine;

public class RisingPlatform : MonoBehaviour
{
    public float speed = 1f; // ��� �ӵ�
    public float resetHeight = 30f; // ���� ���� ����
    public float bottomY = 0f; // ���� ��ġ
    public float cycleLength = 5f; // �� ���� ���� �ð� (��)

    private float timeOffset; // �÷��� �� ���� ������ ����
    private float heightRange;

    void Start()
    {
        heightRange = resetHeight - bottomY;

        // Y �������� ���� ����ؼ� ���� �������� �ٸ���
        timeOffset = (transform.position.y - bottomY) / heightRange * cycleLength;
    }

    void Update()
    {
        float t = (Time.time + timeOffset) % cycleLength; // ���� ���ؼ� �ð� ��� �ݺ�
        float y = bottomY + (t / cycleLength) * heightRange;

        Vector3 pos = transform.position;
        pos.y = y;
        transform.position = pos;
    }
}
