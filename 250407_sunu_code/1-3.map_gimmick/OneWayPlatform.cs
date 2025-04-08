using UnityEngine;

public class OneWayPlatform3D : MonoBehaviour
{
    private Collider platformCollider; // 플랫폼 자신의 콜라이더
    private Transform player; // 플레이어 Transform
    private Collider[] playerColliders; // 플레이어에 포함된 모든 콜라이더 배열

    public float verticalOffset = 0.1f; // 플레이어 중심에서 발 위치 보정값

    void Start()
    {
        platformCollider = GetComponent<Collider>(); // 자신의 콜라이더 가져옴

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player"); // 태그로 플레이어 찾음
        if (playerObj != null)
        {
            player = playerObj.transform; // 플레이어 Transform 저장
            playerColliders = playerObj.GetComponentsInChildren<Collider>(); // 자식 포함 모든 콜라이더 가져옴
        }
    }

    void Update()
    {
        if (player == null || platformCollider == null || playerColliders == null) return; // 유효성 체크

        // 플레이어 위치가 플랫폼보다 아래에 있으면 충돌 무시
        bool shouldIgnore = (player.position.y + verticalOffset) < transform.position.y;

        foreach (var pCol in playerColliders)
        {
            Physics.IgnoreCollision(pCol, platformCollider, shouldIgnore); // 충돌 무시 적용
        }
    }
}
