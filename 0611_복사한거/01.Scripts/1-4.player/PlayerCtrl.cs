using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCtrl : MonoBehaviour
{
    public enum PlayerState { Idle, Walking, Jumping, WallSliding, WallJumping } // �÷��̾� ���� ����
    public PlayerState state = PlayerState.Idle; // �⺻ ���´� Idle

    public float moveSpeed = 10.0f; // �̵� �ӵ�
    public float jumpForce = 10.0f; // ���� ��
    public float wallSlideSpeed = 2.0f; // �� �����̵� �ӵ�
    public float wallJumpForce = 15.0f; // �� ���� ��
    private bool stopJump = false; // ���� ���� Ű �ô��� ����

    private Vector2 moveDir; // �Է� ����
    private bool canMove = true; // �̵� ���� ����
    public bool isGrounded = true; // �ٴڿ� �ִ��� ����
    private Rigidbody rb; // ���� ó���� ������ٵ�
    private Animator animator; // �ִϸ�����
    public Collider playerCollider; // �÷��̾� �ݶ��̴�

    public GameObject bulletPrefab; // �Ѿ� ������
    public Transform firePoint; // �Ѿ� �߻� ��ġ
    public float bulletForce = 20f; // �Ѿ� �߻� ��

    private PlayerControls controls; // New Input System ��Ʈ��
    private bool isDropping = false; // �Ʒ��� ��� ������ ����

    void Awake()
    {
        rb = GetComponent<Rigidbody>(); // ������ٵ� �Ҵ�
        animator = GetComponent<Animator>(); // �ִϸ����� �Ҵ�

        controls = new PlayerControls(); // �Է� ��Ʈ�� �ʱ�ȭ
        playerCollider = GetComponent<Collider>(); // �ݶ��̴� �Ҵ�

        controls.Gameplay.Move.performed += ctx => moveDir = ctx.ReadValue<Vector2>(); // �̵� �Է� ó��
        controls.Gameplay.Move.canceled += ctx => moveDir = Vector2.zero; // �Է� ���� �� ���� �ʱ�ȭ

        //controls.Gameplay.Jump.performed += ctx => TryJump(); // ���� �Է�
        controls.Gameplay.Jump.performed += ctx => StartCoroutine(DelayedJump());

        controls.Gameplay.Jump.canceled += ctx => stopJump = true; // ���� Ű ���� ��

        controls.Gameplay.Attack.performed += ctx => FireBullet(); // ���� �Է�
    }

    void OnEnable() => controls.Gameplay.Enable(); // �Է� Ȱ��ȭ
    void OnDisable() => controls.Gameplay.Disable(); // �Է� ��Ȱ��ȭ

    void Start()
    {
        transform.position = GameManager.instance.respawnPosition; // ������ ��ġ�� �̵�
    }

    void Update()
    {
        if (canMove)
        {
            animator.SetBool("isWalking", moveDir.x != 0); // �ȴ� �ִϸ��̼� ����
        }

        if (stopJump && rb.velocity.y > 0) // ���� ���� ���� Ű ���� ��
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * 0.5f, rb.velocity.z); // ��� �ӵ� ����
            stopJump = false;
        }

        if (state == PlayerState.Jumping && rb.velocity.y <= 0 && isGrounded) // ���� �� ����
        {
            state = PlayerState.Idle;
            animator.SetBool("isJumping", false);
        }


        if (transform.position.y < -10) // ���� ó��
        {
            Die();
        }
    }


    void FixedUpdate()
    {
        if (canMove && state != PlayerState.WallJumping) // �̵� �����ϰ� �� ���� ���� �ƴ� ����
        {
            Move();
        }
    }

    void Move()
    {
        rb.velocity = new Vector3(moveDir.x * moveSpeed, rb.velocity.y, 0); // ���� �ӵ� ����

        if (moveDir.x > 0)
            transform.rotation = Quaternion.Euler(0, 90, 0); // ������ �ٶ󺸰�
        else if (moveDir.x < 0)
            transform.rotation = Quaternion.Euler(0, -90, 0); // ���� �ٶ󺸰�
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
        yield return null; // �� ������ ��� �� moveDir.y �ݿ� ��ٸ�
        TryJump(); // ��¥ ���� �õ�
    }




    void Jump()
    {
        state = PlayerState.Jumping; // ���� ����
        isGrounded = false;
        animator.SetBool("isJumping", true);
        animator.SetBool("grounded", false);

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); // ���� �ӵ� �ʱ�ȭ
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // ���� �� �߰�
        stopJump = false;
    }

    void WallJump()
    {
        state = PlayerState.WallJumping; // ���� ��ȯ
        canMove = false;
        isGrounded = false;
        animator.SetBool("isJumping", true);
        animator.SetBool("grounded", false);

        float wallJumpDir = moveDir.x <= 0 ? 1 : -1; // �ݴ� �������� ����
        moveDir.x = wallJumpDir;

        transform.rotation = Quaternion.Euler(0, wallJumpDir > 0 ? 90 : -90, 0); // ���� ��ȯ

        Vector3 jumpDir = new Vector3(wallJumpDir * 3.5f, 5.5f, 0).normalized; // ���� ���� ���

        rb.velocity = Vector3.zero;
        rb.AddForce(jumpDir * wallJumpForce, ForceMode.Impulse); // �� ���� ����
    }


    void FireBullet()
    {
        if (firePoint == null || bulletPrefab == null) return; // �߻� �Ұ� ����

        Transform target = FindTargetInFront(); // ���� �� Ž��

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation); // �Ѿ� ����
        Bullet bulletScript = bullet.GetComponent<Bullet>();

        if (bulletScript != null && target != null)
        {
            bulletScript.SetTarget(target); // Ÿ�� ����
        }

        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>(); // ������ٵ� ������ (���� �� ��)
    }

    Transform FindTargetInFront()
    {
        float range = 10f;
        float height = 3f;
        float width = 1f;

        Vector3 forward = transform.forward; // �� ���� ����
        Vector3 center = transform.position + forward * range * 0.5f + Vector3.up * height * 0.5f;
        Vector3 halfExtents = new Vector3(width / 2f, height / 2f, range / 2f);

        Collider[] hits = Physics.OverlapBox(center, halfExtents, transform.rotation); // ���� ȸ�� ����
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
        Debug.Log($"OverlapSphere üũ ��ġ: {pos}, �ݰ�: {radius}");

        Collider[] colliders = Physics.OverlapSphere(pos, radius); // �ֺ� �÷��� ����
        foreach (Collider col in colliders)
        {
            Debug.Log("������ �ݶ��̴�: " + col.name + ", Layer: " + col.gameObject.layer + ", �±�: " + col.tag);
            if (col.gameObject.layer == LayerMask.NameToLayer("Platform"))
                return col.GetComponent<OneWayPlatform3D>(); // �÷��� ��ȯ
        }
        return null;
    }

    void ResetDrop()
    {
        isDropping = false; // ��� �÷��� ����
    }

    void Die()
    {
        GameManager.instance.RespawnPlayer(gameObject); // ������
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
            rb.velocity = new Vector3(rb.velocity.x, -wallSlideSpeed, 0); // �� Ÿ�� �ӵ� ����
            animator.SetBool("isJumping", false);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            animator.SetBool("grounded", false); // ���� ���·�
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall") && state != PlayerState.WallJumping)
        {
            state = PlayerState.WallSliding;
            rb.velocity = new Vector3(rb.velocity.x, -wallSlideSpeed, 0); // ��� �� Ÿ�� ����
        }
    }

    private void OnDrawGizmos()
    {
        float range = 10f;
        float height = 3f;
        float width = 1f;

        Vector3 forward = transform.forward; // �� ���� ����
        Vector3 center = transform.position + forward * range * 0.5f + Vector3.up * height * 0.5f;
        Vector3 size = new Vector3(width, height, range);

        Gizmos.color = Color.cyan;
        Matrix4x4 prev = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(center, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, size);
        Gizmos.matrix = prev;
    }
    


}
