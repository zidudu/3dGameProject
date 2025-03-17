using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    //상태
    public enum PlayerState { Idle, Walking, Jumping, WallSliding, WallJumping }
    //기본 상태 IDLE
    public PlayerState state = PlayerState.Idle;

    //이동속도
    public float moveSpeed = 10.0f;
    //점프
    public float jumpForce = 10.0f;
    //벽 내려가는 속도
    public float wallSlideSpeed = 2.0f;
    //벽에서 점프하는 속도
    public float wallJumpForce = 15.0f;
    //멈춰서 점프
    private bool stopJump = false;

    //움직임 float
    private float moveDir;
    //움직이는 상태
    private bool canMove = true;
    //바닥 있는 상태
    private bool isGrounded = true;

    //리지드바디
    private Rigidbody rb;
    //애니메이션
    private Animator animator;

    void Awake()
    {
        //
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        //움직일 수 있는 상태이면
        if (canMove)
        {
            //좌우 이동
            moveDir = Input.GetAxis("Horizontal");
            //움직이는 애니메이션
            animator.SetBool("isWalking", moveDir != 0);
        }
        //점프 버튼을 땠고, 상태가 점핑 상태일때
        if (Input.GetButtonUp("Jump") && state == PlayerState.Jumping)
        {
            //멈춰서 점프 가능
            stopJump = true;
        }
        //멈춰서 점프 상태와 가속도 y가 위쪽일때
        if (stopJump && rb.velocity.y > 0)
        {
            //점프 절반 감소
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * 0.5f, rb.velocity.z); // 점프 높이 감소
            //멈춰서 점프 불가능
            stopJump = false;
        }

        //상태 정리
        switch (state)
        {
            case PlayerState.Idle:
                //걷는 상태
            case PlayerState.Walking:
                //바닥에 있고, 점프 버튼 눌렀을때
                if (isGrounded && Input.GetButtonDown("Jump"))
                {
                    //점프 함수
                    Jump();
                }
                break;

            case PlayerState.WallSliding:
                if (Input.GetButtonDown("Jump"))
                {
                    WallJump();
                }
                break;
        }
        //점핑 상태와 가속도 내려갈때와 바닥상태이면
        if (state == PlayerState.Jumping && rb.velocity.y <= 0 && isGrounded)
        {
            //기본 상태
            state = PlayerState.Idle;
            animator.SetBool("isJumping", false);
        }
    }

    void FixedUpdate()
    {
        //움직일 수 있고, 상태가 벽점프가 아닐때
        if (canMove && state != PlayerState.WallJumping)
        {
            //움직이는 함수
            Move();
        }
    }

    void Move()
    {
        //가속도 따라 움직임
        rb.velocity = new Vector3(moveDir * moveSpeed, rb.velocity.y, 0);

        //보는 각도 변환
        if (moveDir > 0)
            transform.rotation = Quaternion.Euler(0, 90, 0);
        else if (moveDir < 0)
            transform.rotation = Quaternion.Euler(0, -90, 0);
    }

    void Jump()
    {
        //점핑 상태이면 끝냄. 점핑 상태가 아닐때 점프 함수가 실행되게 함
        if (state == PlayerState.Jumping) return;
        //점핑 상태
        state = PlayerState.Jumping;
        //바닥 x
        isGrounded = false;
        //애니메이션
        animator.SetBool("isJumping", true);
        animator.SetBool("grounded", false);

        //점프할때 좌우값 이동까지 받음. 
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        //jumpforce만큼 점프됨
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        //정지 점프 x
        stopJump = false;
    }

    //벽점프일땐 움직이지 못하고(canMove false)
    void WallJump()
    {

        state = PlayerState.WallJumping;
        canMove = false;
        isGrounded = false;
        animator.SetBool("isJumping", true);
        animator.SetBool("grounded", false);

        //움직임 방향을 벽에 닿았으니 반대 방향으로 돌림
        float wallJumpDir = moveDir <= 0 ? 1 : -1;
        moveDir = wallJumpDir;
        transform.rotation = Quaternion.Euler(0, wallJumpDir > 0 ? 90 : -90, 0);

        //가속도 없애고, 위에서 반대방향 돌린걸로 그 방향대로(대각선 점프) 점프가 됨.
        Vector3 jumpDir = new Vector3(wallJumpDir * 3.5f, 5.5f, 0).normalized;
        rb.velocity = Vector3.zero;
        rb.AddForce(jumpDir * wallJumpForce, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            state = PlayerState.Idle;
            isGrounded = true;
            canMove = true;
            animator.SetBool("isJumping", false);
            animator.SetBool("grounded", true);
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            //점핑 상태이면
            if (state == PlayerState.WallJumping)
            {
                //점핑슬라이드 상태로 바꿈
                state = PlayerState.WallSliding;
                //가속도 감소
                rb.velocity = new Vector3(rb.velocity.x, -wallSlideSpeed, 0);
                //
                animator.SetBool("isJumping", false);
            }
            //점핑 상태 아니면
            else if (state != PlayerState.WallJumping)
            {
                // 벽 슬라이딩 상태
                state = PlayerState.WallSliding;
                //벽 방향 바꾸기
                rb.velocity = new Vector3(rb.velocity.x, -wallSlideSpeed, 0);
                animator.SetBool("isJumping", false);
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            animator.SetBool("grounded", false);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            if (state != PlayerState.WallJumping)
            {
                state = PlayerState.WallSliding;
                rb.velocity = new Vector3(rb.velocity.x, -wallSlideSpeed, 0);
            }
        }
    }
}
