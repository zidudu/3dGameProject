using UnityEngine;

public class CroaAI : MonoBehaviour
{
    public enum State { Idle, Detect, Charge, Rolling, Cooldown, Die }
    public State currentState = State.Idle;

    public float moveSpeed = 2f; // ���� �ӵ�
    public float rollSpeed = 8f; // ������ �ӵ�
    public float detectionRange = 5f; // �÷��̾� ���� �Ÿ�
    public float rollDuration = 2f; // ������ ���� �ð�
    public float cooldownTime = 2f; // ������ �� ��� �ð�
    public float chargeTime = 1f; // ������ �� ��� �ð�

    public Color chargeColor = Color.yellow; // ���� �� ǥ���� ����
    public Color rollColor = Color.red; // ������ �� ����

    public LayerMask platformMask; // �÷��� ���̾� ����

    private Rigidbody rb;
    private Transform player;
    private Animator anim;
    private Renderer rend; // ������ ĳ��
    private Color originalColor; // ���� ���� ����

    private Vector3 rollDirection; // ������ ����
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
                Patrol(); // ���� �̵�
                if (PlayerInRange()) currentState = State.Detect;
                break;

            case State.Detect:
                EvaluatePlayerPosition(); // ���� ���� ��
                break;

            case State.Charge:
                chargeTimer -= Time.deltaTime;
                rb.velocity = Vector3.zero; // ���� ���� ����

                if (chargeTimer <= 0f)
                    StartRolling(pendingRollDir); // ������ ����
                break;

            case State.Rolling:
                rb.velocity = rollDirection * rollSpeed; // ���� �������� ��� �̵�
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
        if (PlayerInRange()) return; // �÷��̾� �����Ǹ� ����

        rb.velocity = new Vector3(moveDir * moveSpeed, rb.velocity.y, 0); // ���� ���� �̵�

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
            EnterCharge(forwardDir); // �ٷ� ���� �� ���� �� ���� ����
        }
        else if (inFront && !HasGroundInDirection(forwardDir))
        {
            PlayRollAnimationOnly(); // �ִϸ��̼Ǹ� ����
            EnterCooldown();
        }
        else if (inBack && HasGroundInDirection(backDir))
        {
            EnterCharge(backDir); // �ڿ� �ְ� ���� ���� �� ���� ��ȯ �� ����
        }
        else
        {
            currentState = State.Cooldown;
            cooldownTimer = 0.5f; // ��� �� �ٽ� Idle
        }
    }

    void EnterCharge(int dir)
    {
        currentState = State.Charge;
        pendingRollDir = dir;
        chargeTimer = chargeTime;
        rb.velocity = Vector3.zero;

        if (rend != null)
            rend.material.color = chargeColor; // ���� ����
    }

    void StartRolling(int dir)
    {
        currentState = State.Rolling;
        rollDirection = new Vector3(dir, 0f, 0f);
        rollTimer = rollDuration;

        moveDir = dir;
        transform.rotation = Quaternion.Euler(0, dir > 0 ? 180 : 0, 0);

        if (rend != null)
            rend.material.color = rollColor; // ������ �� ���� ����

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
            rend.material.color = originalColor; // ���� ����
    }

    bool HasGroundInDirection(int dir)
    {
        Vector3 origin = transform.position + Vector3.right * dir * 0.8f;
        Ray ray = new Ray(origin, Vector3.down);

        Debug.DrawRay(origin, Vector3.down * 2f, Color.red); // Scene â���� Ȯ�ο�

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
