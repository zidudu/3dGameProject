using UnityEngine;

public class RipardAI : MonoBehaviour
{
    public enum State { Idle, Attack, Cooldown, Die }
    public State currentState = State.Idle;

    public float moveSpeed = 2f; // 순찰 속도
    public float detectionRange = 6f; // X축 감지 거리
    public float fireInterval = 2f; // 공격 간격
    public GameObject bullet; // 발사체 프리팹
    public Transform firePoint; // 발사 위치

    private Transform player;
    private Animator anim;
    private Rigidbody rb;

    private float cooldownTimer = 0f;
    private int moveDir = 1; // 1: 오른쪽, -1: 왼쪽

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                Patrol();
                if (PlayerInRange()) currentState = State.Attack;
                break;

            case State.Attack:
                rb.velocity = Vector3.zero;
                Attack();
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
        rb.velocity = new Vector3(moveDir * moveSpeed, rb.velocity.y, 0);

        if (!HasGroundInDirection(moveDir))
        {
            moveDir *= -1;
            transform.rotation = Quaternion.Euler(0, moveDir > 0 ? 180 : 0, 0);
        }
    }

    bool PlayerInRange()
    {
        if (player == null) return false;

        float xDist = Mathf.Abs(player.position.x - transform.position.x);
        float yDist = Mathf.Abs(player.position.y - transform.position.y);

        return xDist <= detectionRange && yDist <= 1f; // Y축 차이 1 이하만 감지
    }

    void Attack()
    {
        if (player == null) return;

        float dx = player.position.x - transform.position.x;
        moveDir = dx >= 0 ? 1 : -1;

        transform.rotation = Quaternion.Euler(0, moveDir > 0 ? 180 : 0, 0);

        if (bullet != null && firePoint != null)
        {
            GameObject obj = Instantiate(bullet, firePoint.position, Quaternion.identity);
            Rigidbody rbBullet = obj.GetComponent<Rigidbody>();

            if (rbBullet != null)
            {
                rbBullet.velocity = new Vector3(moveDir, 0f, 0f) * 10f; // X축으로만 발사
            }
        }

        if (anim != null)
            anim.SetTrigger("Attack");

        cooldownTimer = fireInterval;
        currentState = State.Cooldown;
    }

    bool HasGroundInDirection(int dir)
    {
        Vector3 origin = transform.position + Vector3.right * dir * 0.8f;
        Ray ray = new Ray(origin, Vector3.down);

        Debug.DrawRay(origin, Vector3.down * 2f, Color.red);

        return Physics.Raycast(ray, 2f, LayerMask.GetMask("Platform"));
    }

    public void Die()
    {
        currentState = State.Die;
        rb.velocity = Vector3.zero;

        if (anim != null)
            anim.SetTrigger("Die");
    }
}
