using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossController : MonoBehaviour
{
    public enum BossState { Fireball, Meteor, SpreadFireball, Protecting, Overheat, Stunned, Phase4Attack }
    private float fireballInterval = 1.5f; // 발사 주기
    private float fireballTimer = 0f;
    public enum BossPhase { Phase1, Phase2, Phase3, Phase4 }

    public BossState currentState = BossState.Fireball;
    private BossPhase currentPhase = BossPhase.Phase1;

    public float maxHP = 100f;
    private float currentHP;

    public float stateDuration = 5f;
    private float stateTimer = 0f;

    public GameObject fireballPrefab;
    public Transform firePoint;
    private Transform player;

    public GameObject redIndicatorPrefab;
    public GameObject meteorPrefab;
    public float meteorWarningDelay = 1.2f;
    public float meteorDropHeight = 10f;

    public float spreadFireballDelay = 0.5f;
    public int spreadFireballCount = 5;

    public Vector3 centerPos = new Vector3(0f, 1f, 0f);
    public Vector3 phase3Position = new Vector3(0f, 5f, 0f);

    public GameObject barrierGizmo;
    public bool isProtected = false;

    private bool hasFired = false;
    private bool meteorStarted = false;
    private bool spreadStarted = false;
    private bool hasEnteredPhase3 = false;
    private bool isStunStarted = false;
    private bool isPhase4Started = false;

    private Coroutine currentRoutine = null;

    void Start()
    {
        currentHP = maxHP;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        ChangeState(BossState.Fireball);
    }

    void Update()
    {
        if (player != null)
            LookAtPlayer();

        // 상태별로 정확하게 분리
        switch (currentState)
        {
            case BossState.Protecting:
                ProtectingUpdate();
                return;

            case BossState.Stunned:
                StunnedUpdate();
                return;

            case BossState.Phase4Attack:
                Phase4AttackUpdate();
                return;

            case BossState.Overheat:
                OverheatUpdate();
                break;

            case BossState.Fireball:
                FireballUpdate();
                break;

            case BossState.Meteor:
                MeteorUpdate();
                break;

            case BossState.SpreadFireball:
                SpreadFireballUpdate();
                break;
        }

        if (currentPhase == BossPhase.Phase4)
            return;

        stateTimer += Time.deltaTime;

        if (currentState != BossState.SpreadFireball && stateTimer >= stateDuration)
            NextPatternSequence();
    }




    void ChangeState(BossState newState)
    {
        if (currentState == newState) return; // 같은 상태면 무시

        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }

        currentState = newState;
        stateTimer = 0f;
        hasFired = false;
        meteorStarted = false;
        spreadStarted = false;
        fireballTimer = 0f; // ← 이거 추가

        Debug.Log("상태 전환 → " + newState);

        if (newState == BossState.Protecting)
            StartProtectingPhase();
        else if (newState == BossState.Stunned)
            StartCoroutine(EndStunThenPhase4());
    }



    void NextPatternSequence()
    {
        if (currentPhase == BossPhase.Phase2)
        {
            if (currentState == BossState.Fireball) ChangeState(BossState.Meteor);
            else if (currentState == BossState.Meteor) ChangeState(BossState.SpreadFireball);
            else ChangeState(BossState.Fireball);
        }
        else
        {
            if (currentState == BossState.Fireball) ChangeState(BossState.Meteor);
            else ChangeState(BossState.Fireball);
        }
    }

    void FireballUpdate()
    {
        if (!hasFired)
        {
            FireSpreadProjectiles();
            hasFired = true;
        }
    }

    void MeteorUpdate()
    {
        if (!meteorStarted)
        {
            StartCoroutine(DropMeteorSequence(3, 0.2f));
            meteorStarted = true;
        }
    }

    void SpreadFireballUpdate()
    {
        if (spreadStarted) return;

        currentRoutine = StartCoroutine(SpreadFireballRoutine());
        spreadStarted = true;
    }


    void ProtectingUpdate()
    {
        if (currentPhase != BossPhase.Phase3 || !isProtected) return; // Phase3일 때만

        fireballTimer += Time.deltaTime;
        if (fireballTimer >= fireballInterval)
        {
            FireSpreadProjectiles();
            fireballTimer = 0f;
        }
    }


    void StartProtectingPhase()
    {
        isProtected = true;
        StartCoroutine(MoveToPositionSmooth(phase3Position, 1.5f));

        if (barrierGizmo != null)
            barrierGizmo.SetActive(true);

        BossBarrierManager.Instance?.EnableTorches();
    }
    IEnumerator MoveToPositionSmooth(Vector3 targetPos, float moveTime)
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;

        while (elapsed < moveTime)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / moveTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
    }
    void OverheatUpdate()
    {
        if (!hasFired)
        {
            Debug.Log("보스가 보호막을 잃고 오버히트 상태입니다.");
            hasFired = true;
            StartCoroutine(EndOverheat());
        }
    }

    IEnumerator EndOverheat()
    {
        yield return new WaitForSeconds(2f);
        ChangeState(BossState.Fireball);
    }


    public void OnAllTorchesLit()
    {
        isProtected = false;
        if (barrierGizmo != null) barrierGizmo.SetActive(false);
        BossBarrierManager.Instance?.DisableTorches();
        StartCoroutine(MoveToAfterBarrierBreak(new Vector3(0f, -2f, 0f)));
    }

    IEnumerator MoveToAfterBarrierBreak(Vector3 targetPos)
    {
        float moveTime = 1.5f;
        float elapsed = 0f;
        Vector3 startPos = transform.position;

        while (elapsed < moveTime)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / moveTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        ChangePhase(BossPhase.Phase4);
    }

    void StunnedUpdate()
    {
        if (!isStunStarted)
        {
            isStunStarted = true;
            StartCoroutine(StunThenPhase4Attack());
        }
    }

    IEnumerator StunThenPhase4Attack()
    {
        yield return new WaitForSeconds(5f);
        ChangeState(BossState.Phase4Attack);
    }

    void Phase4AttackUpdate()
    {
        if (!isPhase4Started)
        {
            isPhase4Started = true;
            StartCoroutine(Phase4SlamPattern());
        }
    }

    IEnumerator Phase4SlamPattern()
    {
        int repeatCount = 3;

        for (int i = 0; i < repeatCount; i++)
        {
            // 1단계: 상승
            Vector3 riseStart = transform.position; 
            Vector3 riseTarget = new Vector3(transform.position.x, 2f, transform.position.z);

            float riseTime = 0.5f;
            float elapsed = 0f;

            while (elapsed < riseTime)
            {
                transform.position = Vector3.Lerp(riseStart, riseTarget, elapsed / riseTime);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = riseTarget;
            yield return new WaitForSeconds(0.2f);

            // 2단계: 플레이어 x 기준으로 y = -2 돌진
            Vector3 fallStart = transform.position;
            Vector3 fallTarget = new Vector3(player.position.x, -2f, 0f);
            float fallTime = 0.3f;
            elapsed = 0f;

            while (elapsed < fallTime)
            {
                transform.position = Vector3.Lerp(fallStart, fallTarget, elapsed / fallTime);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = fallTarget;
            yield return new WaitForSeconds(0.5f);
        }

        isPhase4Started = false;
    }

    IEnumerator SpreadFireballRoutine()
    {
        float moveTime = 1f;
        Vector3 startPos = transform.position;
        float elapsed = 0f;

        while (elapsed < moveTime)
        {
            transform.position = Vector3.Lerp(startPos, centerPos, elapsed / moveTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = centerPos;

        for (int cycle = 0; cycle < spreadFireballCount; cycle++)
        {
            float offsetAngle = cycle * 15f;

            for (int i = 0; i < 12; i++)
            {
                float angle = offsetAngle + i * 30f;
                float rad = angle * Mathf.Deg2Rad;

                Vector3 dir = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f);
                GameObject proj = Instantiate(fireballPrefab, transform.position, Quaternion.identity);
                Rigidbody rb = proj.GetComponent<Rigidbody>();
                rb.velocity = dir.normalized * 7f;
                Destroy(proj, 5f);
            }
            


            yield return new WaitForSeconds(spreadFireballDelay);
        }

        yield return new WaitForSeconds(1f);
        if (currentState == BossState.SpreadFireball) // 중간에 상태가 바뀌었을 수도 있으므로
            ChangeState(BossState.Fireball);

        spreadStarted = false;
         // 상태 직접 전환
    }


    IEnumerator DropMeteorSequence(int count, float interval)
    {
        List<Vector3> dropPositions = new List<Vector3>();
        List<GameObject> warnObjects = new List<GameObject>();

        for (int i = 0; i < count; i++)
        {
            Vector3 playerPos = player.position;
            playerPos.z = 0f;
            Vector3 groundPos = FindGroundBelow(playerPos);
            Vector3 warnPos = new Vector3(playerPos.x, groundPos.y + 0.01f, 0f);

            GameObject warn = Instantiate(redIndicatorPrefab, warnPos, Quaternion.Euler(90f, 0f, 0f));
            warnObjects.Add(warn);

            Vector3 dropPos = new Vector3(playerPos.x, playerPos.y + meteorDropHeight, 0f);
            dropPositions.Add(dropPos);

            yield return new WaitForSeconds(interval);
        }

        yield return new WaitForSeconds(meteorWarningDelay);

        for (int i = 0; i < dropPositions.Count; i++)
        {
            Instantiate(meteorPrefab, dropPositions[i], Quaternion.identity);
            if (warnObjects[i] != null) Destroy(warnObjects[i]);
            yield return new WaitForSeconds(interval);
        }
    }
    IEnumerator EndStunThenPhase4()
    {
        yield return new WaitForSeconds(5f);
        ChangeState(BossState.Phase4Attack); // 4페이즈 전용 상태
    }

    Vector3 FindGroundBelow(Vector3 origin)
    {
        RaycastHit hit;
        Vector3 rayStart = origin + Vector3.up * 1f;

        if (Physics.Raycast(rayStart, Vector3.down, out hit, 100f))
        {
            if (hit.collider.CompareTag("Ground"))
                return hit.point;
        }

        return new Vector3(origin.x, 0f, 0f);
    }

    void LookAtPlayer()
    {
        float diff = player.position.x - transform.position.x;
        Vector3 scale = transform.localScale;
        scale.x = (diff > 0) ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    void FireSpreadProjectiles()
    {
        if (player == null || fireballPrefab == null) return;

        Vector3 toPlayer = (player.position - transform.position).normalized;
        Vector2 baseDir2D = new Vector2(toPlayer.x, toPlayer.y).normalized;
        float angleOffset = 30f;

        for (int i = -1; i <= 1; i++)
        {
            float angle = Mathf.Atan2(baseDir2D.y, baseDir2D.x) * Mathf.Rad2Deg + i * angleOffset;
            float rad = angle * Mathf.Deg2Rad;
            Vector2 spreadDir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            Vector3 dir = new Vector3(spreadDir.x, spreadDir.y, 0f);

            GameObject proj = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
            Rigidbody rb = proj.GetComponent<Rigidbody>();
            rb.velocity = dir * 10f;
            Destroy(proj, 5f);
        }
    }

    public void TakeDamage(float dmg)
    {
        if (isProtected || currentState == BossState.Protecting || currentState == BossState.Stunned)
            return;

        currentHP -= dmg;
        CheckPhaseTransition();
        if (currentHP <= 0f) Die();
    }

    void CheckPhaseTransition()
    {
        float hpPercent = currentHP / maxHP;

        if (hpPercent <= 0.2f && currentPhase != BossPhase.Phase4)
            ChangePhase(BossPhase.Phase4);
        else if (hpPercent <= 0.5f && currentPhase != BossPhase.Phase3 && !hasEnteredPhase3)
            ChangePhase(BossPhase.Phase3);
        else if (hpPercent <= 0.7f && currentPhase != BossPhase.Phase2)
            ChangePhase(BossPhase.Phase2);
    }

    void ChangePhase(BossPhase newPhase)
    {
        if (currentPhase == newPhase) return;

        currentPhase = newPhase;

        if (newPhase == BossPhase.Phase2)
            ChangeState(BossState.SpreadFireball);
        else if (newPhase == BossPhase.Phase3)
        {
            if (hasEnteredPhase3) return;
            hasEnteredPhase3 = true;
            ChangeState(BossState.Protecting);
        }
        else if (newPhase == BossPhase.Phase4)
        {
            StartCoroutine(LoadPhase4SceneWithFade()); // 스턴 없이 바로 페이드 → 씬 이동
        }
    }

    IEnumerator LoadPhase4SceneWithFade()
    {
        if (ScreenFader.instance != null)
            yield return ScreenFader.instance.FadeOut(1.2f);

        GameManager.instance.SetPendingWarp(
    new WarpPointData("Phase4", "Phase4Start", "BossPhase4Scene", new Vector3(-10f, 10f, 0f))
);

        SceneManager.LoadScene("Boss4Phase");
    }


    void Die()
    {
        Debug.Log("보스 사망");
        Destroy(gameObject);
    }
}
