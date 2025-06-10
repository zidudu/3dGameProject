using UnityEngine;

public class CroaAI : MonoBehaviour
{
    public enum State { Idle, Detect, Charge, Rolling, Cooldown, Die }
    public State currentState = State.Idle;

    public float moveSpeed = 2f; // 순찰 속도
    public float rollSpeed = 8f; // 구르기 속도
    public float detectionRange = 5f; // 플레이어 감지 거리
    public float rollDuration = 2f; // 구르기 유지 시간
    public float cooldownTime = 2f; // 구르기 후 대기 시간
    public float chargeTime = 1f; // 구르기 전 대기 시간

    public Color chargeColor = Color.yellow; // 충전 시 표시할 색상
    public Color rollColor = Color.red; // 구르기 중 색상

    public LayerMask platformMask; // 플랫폼 레이어 설정

    private Rigidbody rb;
    private Transform player;
    private Animator anim;
    private Renderer rend; // 렌더러 캐싱
    private Color originalColor; // 원래 색상 저장

    private Vector3 rollDirection; // 구르기 방향
    private float rollTimer;
    private float cooldownTimer;
    private float chargeTimer;
    private int moveDir = 1;
    private int pendingRollDir;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        rend = GetComponentInChildren<Renderer>();

        if (rend != null)
            originalColor = rend.material.color;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                Patrol(); // 순찰 이동
                if (PlayerInRange()) currentState = State.Detect;
                break;

            case State.Detect:
                EvaluatePlayerPosition(); // 공격 조건 평가
                break;

            case State.Charge:
                chargeTimer -= Time.deltaTime;
                rb.velocity = Vector3.zero; // 정지 상태 유지

                if (chargeTimer <= 0f)
                    StartRolling(pendingRollDir); // 구르기 시작
                break;

            case State.Rolling:
                rb.velocity = rollDirection * rollSpeed; // 지정 방향으로 고속 이동
                rollTimer -= Time.deltaTime;

                if (rollTimer <= 0f || !HasGroundInDirection(moveDir))
                    EnterCooldown();
                break;

            case State.Cooldown:
                rb.velocity = Vector3.zero;
                cooldownTimer -= Time.deltaTime;
                if (cooldownTimer <= 0f) currentState = State.Idle;
                break;

            case State.Die:
                rb.velocity = Vector3.zero;
                break;
        }
    }

    void Patrol()
    {
        if (PlayerInRange()) return; // 플레이어 감지되면 정지

        rb.velocity = new Vector3(moveDir * moveSpeed, rb.velocity.y, 0); // 순찰 방향 이동

        if (!HasGroundInDirection(moveDir))
        {
            moveDir *= -1;
            transform.rotation = Quaternion.Euler(0, moveDir > 0 ? 180 : 0, 0);
        }
    }

    bool PlayerInRange()
    {
        if (player == null) return false;
        return Vector3.Distance(transform.position, player.position) < detectionRange;
    }

    void EvaluatePlayerPosition()
    {
        int forwardDir = moveDir;
        int backDir = -moveDir;

        bool inFront = IsPlayerInDirection(forwardDir);
        bool inBack = IsPlayerInDirection(backDir);

        if (inFront && HasGroundInDirection(forwardDir))
        {
            EnterCharge(forwardDir); // 바로 구를 수 있음 → 충전 시작
        }
        else if (inFront && !HasGroundInDirection(forwardDir))
        {
            PlayRollAnimationOnly(); // 애니메이션만 실행
            EnterCooldown();
        }
        else if (inBack && HasGroundInDirection(backDir))
        {
            EnterCharge(backDir); // 뒤에 있고 공간 있음 → 방향 전환 후 충전
        }
        else
        {
            currentState = State.Cooldown;
            cooldownTimer = 0.5f; // 대기 후 다시 Idle
        }
    }

    void EnterCharge(int dir)
    {
        currentState = State.Charge;
        pendingRollDir = dir;
        chargeTimer = chargeTime;
        rb.velocity = Vector3.zero;

        if (rend != null)
            rend.material.color = chargeColor; // 색상 변경
    }

    void StartRolling(int dir)
    {
        currentState = State.Rolling;
        rollDirection = new Vector3(dir, 0f, 0f);
        rollTimer = rollDuration;

        moveDir = dir;
        transform.rotation = Quaternion.Euler(0, dir > 0 ? 180 : 0, 0);

        if (rend != null)
            rend.material.color = rollColor; // 구르기 중 색상 적용

        if (anim != null)
            anim.SetTrigger("Roll");
    }


    void PlayRollAnimationOnly()
    {
        if (anim != null)
            anim.SetTrigger("Roll");
    }

    void EnterCooldown()
    {
        currentState = State.Cooldown;
        cooldownTimer = cooldownTime;
        rb.velocity = Vector3.zero;

        if (rend != null)
            rend.material.color = originalColor; // 색상 복원
    }

    bool HasGroundInDirection(int dir)
    {
        Vector3 origin = transform.position + Vector3.right * dir * 0.8f;
        Ray ray = new Ray(origin, Vector3.down);

        Debug.DrawRay(origin, Vector3.down * 2f, Color.red); // Scene 창에서 확인용

        return Physics.Raycast(ray, 2f, platformMask);
    }

    bool IsPlayerInDirection(int dir)
    {
        float dx = player.position.x - transform.position.x;
        return (dir > 0 && dx > 0) || (dir < 0 && dx < 0);
    }

    public void Die()
    {
        currentState = State.Die;
        rb.velocity = Vector3.zero;

        if (rend != null)
            rend.material.color = originalColor;

        if (anim != null)
            anim.SetTrigger("Die");
    }
}
