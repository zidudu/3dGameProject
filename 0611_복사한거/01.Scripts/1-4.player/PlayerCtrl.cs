using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCtrl : MonoBehaviour
{
    public enum PlayerState { Idle, Walking, Jumping, WallSliding, WallJumping } // 플레이어 상태 정의
    public PlayerState state = PlayerState.Idle; // 기본 상태는 Idle

    public float moveSpeed = 10.0f; // 이동 속도
    public float jumpForce = 10.0f; // 점프 힘
    public float wallSlideSpeed = 2.0f; // 벽 슬라이드 속도
    public float wallJumpForce = 15.0f; // 벽 점프 힘
    private bool stopJump = false; // 점프 도중 키 뗐는지 여부

    private Vector2 moveDir; // 입력 방향
    private bool canMove = true; // 이동 가능 여부
    public bool isGrounded = true; // 바닥에 있는지 여부
    private Rigidbody rb; // 물리 처리용 리지드바디
    private Animator animator; // 애니메이터
    public Collider playerCollider; // 플레이어 콜라이더

    public GameObject bulletPrefab; // 총알 프리팹
    public Transform firePoint; // 총알 발사 위치
    public float bulletForce = 20f; // 총알 발사 힘

    private PlayerControls controls; // New Input System 컨트롤
    private bool isDropping = false; // 아래로 통과 중인지 여부

    void Awake()
    {
        rb = GetComponent<Rigidbody>(); // 리지드바디 할당
        animator = GetComponent<Animator>(); // 애니메이터 할당

        controls = new PlayerControls(); // 입력 컨트롤 초기화
        playerCollider = GetComponent<Collider>(); // 콜라이더 할당

        controls.Gameplay.Move.performed += ctx => moveDir = ctx.ReadValue<Vector2>(); // 이동 입력 처리
        controls.Gameplay.Move.canceled += ctx => moveDir = Vector2.zero; // 입력 해제 시 방향 초기화

        //controls.Gameplay.Jump.performed += ctx => TryJump(); // 점프 입력
        controls.Gameplay.Jump.performed += ctx => StartCoroutine(DelayedJump());

        controls.Gameplay.Jump.canceled += ctx => stopJump = true; // 점프 키 뗐을 때

        controls.Gameplay.Attack.performed += ctx => FireBullet(); // 공격 입력
    }

    void OnEnable() => controls.Gameplay.Enable(); // 입력 활성화
    void OnDisable() => controls.Gameplay.Disable(); // 입력 비활성화

    void Start()
    {
        transform.position = GameManager.instance.respawnPosition; // 리스폰 위치로 이동
    }

    void Update()
    {
        if (canMove)
        {
            animator.SetBool("isWalking", moveDir.x != 0); // 걷는 애니메이션 갱신
        }

        if (stopJump && rb.velocity.y > 0) // 점프 도중 점프 키 뗐을 때
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * 0.5f, rb.velocity.z); // 상승 속도 줄임
            stopJump = false;
        }

        if (state == PlayerState.Jumping && rb.velocity.y <= 0 && isGrounded) // 점프 후 착지
        {
            state = PlayerState.Idle;
            animator.SetBool("isJumping", false);
        }


        if (transform.position.y < -10) // 낙사 처리
        {
            Die();
        }
    }


    void FixedUpdate()
    {
        if (canMove && state != PlayerState.WallJumping) // 이동 가능하고 벽 점프 중이 아닐 때만
        {
            Move();
        }
    }

    void Move()
    {
        rb.velocity = new Vector3(moveDir.x * moveSpeed, rb.velocity.y, 0); // 수평 속도 설정

        if (moveDir.x > 0)
            transform.rotation = Quaternion.Euler(0, 90, 0); // 오른쪽 바라보게
        else if (moveDir.x < 0)
            transform.rotation = Quaternion.Euler(0, -90, 0); // 왼쪽 바라보게
    }

    void TryJump()
    {
        if (moveDir.y < -0.5f && isGrounded)
        {
            OneWayPlatform3D platform = GetCurrentPlatform();
            if (platform != null && !isDropping)
            {
                isDropping = true;
                //platform.StartDropThrough();
                Invoke(nameof(ResetDrop), 0.6f);
            }
            return;
        }

        if (state == PlayerState.Jumping) return;

        if (state == PlayerState.WallSliding)
        {
            WallJump();
            return;
        }

        if (isGrounded)
        {
            Jump();
        }
    }
    private IEnumerator DelayedJump()
    {
        yield return null; // 한 프레임 대기 → moveDir.y 반영 기다림
        TryJump(); // 진짜 점프 시도
    }




    void Jump()
    {
        state = PlayerState.Jumping; // 상태 변경
        isGrounded = false;
        animator.SetBool("isJumping", true);
        animator.SetBool("grounded", false);

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); // 수직 속도 초기화
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // 점프 힘 추가
        stopJump = false;
    }

    void WallJump()
    {
        state = PlayerState.WallJumping; // 상태 전환
        canMove = false;
        isGrounded = false;
        animator.SetBool("isJumping", true);
        animator.SetBool("grounded", false);

        float wallJumpDir = moveDir.x <= 0 ? 1 : -1; // 반대 방향으로 점프
        moveDir.x = wallJumpDir;

        transform.rotation = Quaternion.Euler(0, wallJumpDir > 0 ? 90 : -90, 0); // 방향 전환

        Vector3 jumpDir = new Vector3(wallJumpDir * 3.5f, 5.5f, 0).normalized; // 점프 방향 계산

        rb.velocity = Vector3.zero;
        rb.AddForce(jumpDir * wallJumpForce, ForceMode.Impulse); // 힘 가해 점프
    }


    void FireBullet()
    {
        if (firePoint == null || bulletPrefab == null) return; // 발사 불가 조건

        Transform target = FindTargetInFront(); // 정면 적 탐지

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation); // 총알 생성
        Bullet bulletScript = bullet.GetComponent<Bullet>();

        if (bulletScript != null && target != null)
        {
            bulletScript.SetTarget(target); // 타겟 지정
        }

        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>(); // 리지드바디 가져옴 (힘은 안 씀)
    }

    Transform FindTargetInFront()
    {
        float range = 10f;
        float height = 3f;
        float width = 1f;

        Vector3 forward = transform.forward; // ← 원래 기준
        Vector3 center = transform.position + forward * range * 0.5f + Vector3.up * height * 0.5f;
        Vector3 halfExtents = new Vector3(width / 2f, height / 2f, range / 2f);

        Collider[] hits = Physics.OverlapBox(center, halfExtents, transform.rotation); // 원래 회전 기준
        float minDist = Mathf.Infinity;
        Transform nearest = null;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy")||hit.CompareTag("Boss"))
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = hit.transform;
                }
            }
        }

        return nearest;
    }


    



    OneWayPlatform3D GetCurrentPlatform()
    {
        float radius = 0.1f;
        Vector3 pos = transform.position;
        Debug.Log($"OverlapSphere 체크 위치: {pos}, 반경: {radius}");

        Collider[] colliders = Physics.OverlapSphere(pos, radius); // 주변 플랫폼 감지
        foreach (Collider col in colliders)
        {
            Debug.Log("감지된 콜라이더: " + col.name + ", Layer: " + col.gameObject.layer + ", 태그: " + col.tag);
            if (col.gameObject.layer == LayerMask.NameToLayer("Platform"))
                return col.GetComponent<OneWayPlatform3D>(); // 플랫폼 반환
        }
        return null;
    }

    void ResetDrop()
    {
        isDropping = false; // 드롭 플래그 해제
    }

    void Die()
    {
        GameManager.instance.RespawnPlayer(gameObject); // 리스폰
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Platform"))
        {
            state = PlayerState.Idle;
            isGrounded = true;
            canMove = true;
            animator.SetBool("isJumping", false);
            animator.SetBool("grounded", true);
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            state = PlayerState.WallSliding;
            rb.velocity = new Vector3(rb.velocity.x, -wallSlideSpeed, 0); // 벽 타기 속도 설정
            animator.SetBool("isJumping", false);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            animator.SetBool("grounded", false); // 공중 상태로
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall") && state != PlayerState.WallJumping)
        {
            state = PlayerState.WallSliding;
            rb.velocity = new Vector3(rb.velocity.x, -wallSlideSpeed, 0); // 계속 벽 타기 유지
        }
    }

    private void OnDrawGizmos()
    {
        float range = 10f;
        float height = 3f;
        float width = 1f;

        Vector3 forward = transform.forward; // ← 원래 기준
        Vector3 center = transform.position + forward * range * 0.5f + Vector3.up * height * 0.5f;
        Vector3 size = new Vector3(width, height, range);

        Gizmos.color = Color.cyan;
        Matrix4x4 prev = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(center, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, size);
        Gizmos.matrix = prev;
    }
    


}
