using UnityEngine;

public class CroaAI : MonoBehaviour, IDamager
{
    public enum State { Idle, Detect, Charge, Rolling, Cooldown, Die }
    public State currentState = State.Idle;

    [Header("순찰 구간(오브젝트 참조)")]
    [Tooltip("순찰 시작 지점(Transform)")]
    public Transform patrolStart;          // 빈 오브젝트를 드래그
    [Tooltip("순찰 끝 지점(Transform)")]
    public Transform patrolEnd;            // 빈 오브젝트를 드래그

    [Header("이동/구르기 설정")]
    [Tooltip("순찰 이동 속도")]
    public float moveSpeed = 2f;
    [Tooltip("구르기 속도")]
    public float rollSpeed = 8f;
    [Tooltip("플레이어 감지 거리")]
    public float detectionRange = 5f;
    [Tooltip("구르기 유지 시간")]
    public float rollDuration = 2f;
    [Tooltip("구르기 후 대기 시간")]
    public float cooldownTime = 2f;
    [Tooltip("구르기 전 충전 대기 시간")]
    public float chargeTime = 1f;

    [Header("색상 설정")]
    [Tooltip("충전 중 표시할 색상")]
    public Color chargeColor = Color.yellow;
    [Tooltip("구르는 중 표시할 색상")]
    public Color rollColor = Color.red;

    [Header("지형/플랫폼 감지용 마스크")]
    public LayerMask platformMask; // 플랫폼 레이어 설정

    [Header("데미지")]
    [Tooltip("한 번 구르기 충돌 시 입힐 피해량")]
    public float damage = 10f;
    [Tooltip("동일 대상 재충돌까지 쿨다운(초)")]
    public float damageCooldown = 0.5f;
    /*────────────────┤ 데미지 쿨다운 관리 ├────────────────*/
    private float lastHitTime = -999f;     // 최근 플레이어를 때린 시각


    [Header("체력 설정")]
    [Tooltip("이 몬스터의 최대 체력")]
    public float maxHealth = 100f;
    [Tooltip("현재 체력 (자동 초기화됨)")]
    public float currentHealth;

    [Header("Persistence (씬 재진입 시 사라진 몬스터 유지)")]
    [Tooltip("이 몬스터의 고유 ID (씬 당 유일)")]
    public string monsterID = "Croa_001";
    [Tooltip("테스트용: 저장된 파괴 상태 무시")]
    public bool ignoreSaved = false;


    /*────────────────┤ IDamager 구현 ├────────────────*/
    public float GetDamage() => damage;
    public float GetCooldown() => damageCooldown;

    [Header("체력바 UI")]
    [Tooltip("체력바 페이로드 프리팹 경로 (Resources 폴더 내부)")]
    public string healthBarResourcePath = "Prefabs/HealthBarCanvas";
    // 체력바 UI 인스턴스
    private HealthBarUI healthBarUI;

    // ────── 내부 변수 ──────
    private Rigidbody rb;
    private Transform player;
    private Animator anim;
    private Renderer rend; // 렌더러 캐싱
    private Color originalColor; // 원래 색상 저장

    private Vector3 rollDirection; // 구르기 방향
    private float rollTimer;
    private float cooldownTimer;
    private float chargeTimer;
    private int moveDir = 1; // 1: 오른쪽, -1: 왼쪽
    private int pendingRollDir; // 충전→구르기 방향

  
 

    void Start()
    {
        // ───────────────── Persistence 체크 ─────────────────
        if (!ignoreSaved && GameManager.instance.IsObjectDestroyed(monsterID))
        {
            // 이미 파괴된 상태라면, 몬스터 오브젝트 자체를 제거하고 Start() 종료
            Destroy(gameObject);
            return;
        }
        // ──────────────────────────────────────────────────
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        rend = GetComponentInChildren<Renderer>();

        if (rend != null)
            originalColor = rend.material.color;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // 3) 체력 초기화
        currentHealth = maxHealth;

        // 4) 체력바 UI 프리팹 불러와서 인스턴스화
        GameObject barPrefab = Resources.Load<GameObject>(healthBarResourcePath);
        if (barPrefab != null)
        {
            GameObject barInstance = Instantiate(barPrefab);
            healthBarUI = barInstance.GetComponent<HealthBarUI>();

            // 5) 체력바 UI에 타겟, 카메라, 초기 체력값 설정
            healthBarUI.target = this.transform;
            healthBarUI.mainCamera = Camera.main;
            healthBarUI.SetHealth(currentHealth, maxHealth);
        }
        else
        {
            Debug.LogError($"[{nameof(CroaAI)}] Resources/{healthBarResourcePath} 경로에 체력바 프리팹을 찾을 수 없습니다.");
        }
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
    #region ───── 순찰 및 행동 ─────
    void Patrol()
    {
        if (PlayerInRange()) return; // 플레이어 감지되면 정지

        rb.velocity = new Vector3(moveDir * moveSpeed, rb.velocity.y, 0); // 순찰 방향 이동

        //if (!HasGroundInDirection(moveDir))
        //{
        //    moveDir *= -1;
        //    transform.rotation = Quaternion.Euler(0, moveDir > 0 ? 180 : 0, 0);
        //}
        // 순찰 Start/End 지점 사이에서 왕복
        float startX = patrolStart ? patrolStart.position.x : transform.position.x;
        float endX = patrolEnd ? patrolEnd.position.x : transform.position.x;

        // 왼쪽→오른쪽 범위 정렬
        if (startX > endX) (startX, endX) = (endX, startX);

        // 범위 끝에 도달하면 방향 반전
        if (transform.position.x <= startX)
        {
            moveDir = 1;
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (transform.position.x >= endX)
        {
            moveDir = -1;
            transform.rotation = Quaternion.Euler(0, 0, 0);
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
        if (player == null) return false;
        float dx = player.position.x - transform.position.x;
        return (dir > 0 && dx > 0) || (dir < 0 && dx < 0);
    }
    #endregion

    #region ───── IDamager 구현 ─────
    // 예시: 구르기 상태에서 플레이어와 충돌하면 데미지를 주도록 구현
    private void OnCollisionEnter(Collision collision)
    {
        // “Bullet” 태그의 Collider과 물리 충돌했을 때
        if (collision.collider.CompareTag("Bullet"))
        {
            var bulletScript = collision.collider.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                // 총알에 설정된 damage값만큼 체력 감소
                TakeDamage(bulletScript.damage);
            }

            Destroy(collision.collider.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // “Bullet” 태그가 붙은 오브젝트와 충돌했을 때
        if (other.CompareTag("Bullet"))
        {
            // IDamager 인터페이스로부터 데미지 값을 읽어옴
            if (other.TryGetComponent<IDamager>(out var damager))
            {
                // TakeDamage(…) 호출하여 체력 감소
                TakeDamage(damager.GetDamage());
            }
            // 필요하다면 총알 오브젝트 파괴
            Destroy(other.gameObject);
        }

        if (currentState == State.Rolling && other.CompareTag("Player"))
        {
            if (Time.time - lastHitTime > damageCooldown)
            {
                var playerHealth = other.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                    lastHitTime = Time.time;
                }
            }
        }
    }
    #endregion

    #region ───── 체력 관리 ─────

    /// <summary>
    /// 이 몬스터가 피해를 받을 때 호출되는 함수
    /// </summary>
    /// <param name="amount">입힐 데미지</param>
    public void TakeDamage(float amount)
    {
        if (currentState == State.Die) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        // 체력바 UI 갱신
        if (healthBarUI != null)
            healthBarUI.SetHealth(currentHealth, maxHealth);

        // 체력이 0 이하일 경우 사망 처리
        if (currentHealth <= 0f)
        {
            Die();
        }
    }



    public void Die()
    {
        currentState = State.Die;
        rb.velocity = Vector3.zero;

        if (rend != null)
            rend.material.color = originalColor;

        if (anim != null)
            anim.SetTrigger("Die");

        // ───────────────── Persistence 등록 ─────────────────
        GameManager.instance.RegisterDestroyedObject(monsterID);
        // ──────────────────────────────────────────────────

        // 체력바 UI 제거
        if (healthBarUI != null)
            Destroy(healthBarUI.gameObject);

        // 필요한 경우, 몬스터 오브젝트를 삭제하거나 애니메이션 종료 후 파괴
        Destroy(gameObject, 0.5f);
    }
    #endregion





#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (patrolStart && patrolEnd)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(patrolStart.position, patrolEnd.position);
            Gizmos.DrawSphere(patrolStart.position, 0.1f);
            Gizmos.DrawSphere(patrolEnd.position, 0.1f);
        }
    }
#endif
}
