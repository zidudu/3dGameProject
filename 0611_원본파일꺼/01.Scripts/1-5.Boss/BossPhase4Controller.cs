using UnityEngine;
using System.Collections;

public class BossPhase4Controller : MonoBehaviour
{
    public float chargeSpeed = 12f;
    public float knockbackForce = 5f;
    public float stunDuration = 1.5f;
    public GameObject fireballPrefab;
    public Transform firePoint;

    public float fireballSpeed = 30f; // 속도 3배
    public int maxHP = 100;
    private int currentHP;

    private Transform player;
    private Rigidbody rb;
    private Vector3 chargeDir;
    private bool isCharging = false;
    private bool isStunned = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody>();
        currentHP = maxHP;

        StartCoroutine(StartWithDelay());
    }


    IEnumerator PatternLoop()
    {
        while (true)
        {
            yield return StartCoroutine(ChargeAndStun());
            yield return StartCoroutine(SpreadFireballAttack());
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator ChargeAndStun()
    {
        if (player == null) yield break;

        chargeDir = (player.position - transform.position).normalized;
        isCharging = true;
        rb.velocity = chargeDir * chargeSpeed;

        while (isCharging)
            yield return null;

        Vector3 bounceDir = -new Vector3(chargeDir.x, 0f, 0f).normalized;
        rb.velocity = bounceDir * knockbackForce;
        yield return new WaitForSeconds(0.2f);

        rb.velocity = Vector3.zero;
        isStunned = true;
        yield return new WaitForSeconds(stunDuration);
        isStunned = false;
    }

    IEnumerator SpreadFireballAttack()
    {
        if (fireballPrefab == null || firePoint == null || player == null) yield break;

        Vector3 baseDir = (player.position - firePoint.position).normalized;
        float baseAngle = Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg;

        int count = 3; // 3방향 스프레드
        float spread = 15f;

        for (int i = 0; i < count; i++)
        {
            float angle = baseAngle + (i - 1) * spread;
            float rad = angle * Mathf.Deg2Rad;
            Vector3 dir = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f);

            GameObject proj = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
            Rigidbody prb = proj.GetComponent<Rigidbody>();
            if (prb != null)
                prb.velocity = dir.normalized * fireballSpeed;

            Destroy(proj, 5f);
        }

        yield return new WaitForSeconds(0.5f);
    }
    void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        Debug.Log($"보스 체력: {currentHP}");

        if (currentHP <= 0)
        {
            Debug.Log("보스 사망");
            Destroy(gameObject);
        }
    }

    IEnumerator StartWithDelay()
    {
        yield return new WaitForSeconds(3f); // 딱 한 번 3초 대기
        StartCoroutine(PatternLoop()); // 이후 패턴 루프 시작
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BossWall") && isCharging)
        {
            isCharging = false;
        }
        if (other.CompareTag("Bullet"))
        {
            TakeDamage(10); // 총알에 맞으면 10 깎기
            Destroy(other.gameObject); // 총알 제거
        }
    
        if (other.CompareTag("Player"))
        {
            Debug.Log("플레이어와 충돌");
        }
    }
}
