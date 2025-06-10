using UnityEngine;

public class RisingPlatform : MonoBehaviour
{
    public float speed = 1f; // 상승 속도
    public float resetHeight = 30f; // 리셋 기준 높이
    public float bottomY = 0f; // 시작 위치
    public float cycleLength = 5f; // 한 바퀴 도는 시간 (초)

    private float timeOffset; // 플랫폼 간 간격 조절용 위상
    private float heightRange;

    void Start()
    {
        heightRange = resetHeight - bottomY;

        // Y 기준으로 위상 계산해서 각자 오프셋을 다르게
        timeOffset = (transform.position.y - bottomY) / heightRange * cycleLength;
    }

    void Update()
    {
        float t = (Time.time + timeOffset) % cycleLength; // 위상 더해서 시간 기반 반복
        float y = bottomY + (t / cycleLength) * heightRange;

        Vector3 pos = transform.position;
        pos.y = y;
        transform.position = pos;
    }
}
