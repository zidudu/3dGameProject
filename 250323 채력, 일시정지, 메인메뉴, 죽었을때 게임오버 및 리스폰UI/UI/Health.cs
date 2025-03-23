using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class Health : MonoBehaviour
{
    // 현재 체력 (5)
    public float health = 5.0f;
    // 최대 체력에 맞는 하트 수 (체력 5에 맞춰서 5개의 하트)
    public int numOfHearts = 5;

    // 하트 이미지들
    public Image[] hearts;
    // 꽉 찬 하트
    public Sprite fullHeart;
    // 반 하트
    public Sprite halfHeart;
    // 빈 하트
    public Sprite emptyHeart;
    // =========================================
    // 게임 오버 UI 및 페이드 관련
    // =========================================
    [Header("Game Over / Fade UI")]
    public CanvasGroup fadeCanvasGroup; // 화면을 가리는 검은 패널(알파값 조절) 
    public GameObject gameOverUI;       // 게임오버 UI 패널(버튼 포함)
    public float fadeDuration = 1.5f;   // 화면이 서서히 어두워지는 데 걸리는 시간

    // 이미 게임오버 시퀀스를 실행 중인지 체크할 변수
    private bool isDead = false;


    public void TakeDamage(float damage)
    {
        // 이미 사망 중인 상태라면 데미지 계산을 무시하거나,
        // 혹은 추가로 생기는 데미지만 막을 수도 있음
        if (isDead) return;

        health -= damage;
        if (health < 0)
        {
            health = 0;
        }
        // 체력이 0이 되면 게임 오버 처리
        if (health <= 0 && !isDead)
        {
            isDead = true; // 사망 처리 플래그 설정
            StartCoroutine(GameOverSequence());
        }
        UpdateHealthUI();
    }

    private void Update()
    {
        // 최대 체력을 넘지 않도록 보정
        if (health > numOfHearts * 2) // numOfHearts는 체력 반 칸을 표시하므로 최대는 2배
        {
            health = numOfHearts * 2;  // 최대 체력을 넘지 않도록
        }
        UpdateHealthUI();
    }

    // 체력 UI 업데이트 함수
    public void UpdateHealthUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            // health가 0.5 단위로 줄어들 때마다 반 칸을 표시할 수 있도록 처리
            if (i < Mathf.FloorToInt(health)) // 꽉 찬 하트
            {
                hearts[i].sprite = fullHeart;
            }
            else if (i == Mathf.FloorToInt(health) && health % 1 != 0) // 반 하트 (0.5 단위로 반 칸을 표시)
            {
                hearts[i].sprite = halfHeart;
            }
            else // 빈 하트
            {
                hearts[i].sprite = emptyHeart;
            }

            // 하트의 활성화 여부 설정
            if (i < numOfHearts)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("MONSTER"))
        {
            Monster monster = collision.gameObject.GetComponent<Monster>();
            if (monster != null)
            {
                float damage = monster.GetDamage();
                TakeDamage(damage);
            }
        }
    }
    // =========================================
    // 게임 오버 시퀀스(코루틴)
    // =========================================
    IEnumerator GameOverSequence()
    {
        // 1) 화면을 서서히 어둡게 만들기
        if (fadeCanvasGroup)
        {
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                float alpha = Mathf.Clamp01(t / fadeDuration);
                fadeCanvasGroup.alpha = alpha;
                yield return null;
            }
            fadeCanvasGroup.alpha = 1f; // 완전히 어두워짐
        }

        // 2) 어두워진 뒤, 게임 오버 UI 표시
        if (gameOverUI)
        {
            gameOverUI.SetActive(true);
            
        }
        // 3) 시간 정지
        Time.timeScale = 0;
        // 이후는 버튼 클릭 시 로직(GameManager 통해 세이브포인트 로드 or 메인메뉴)에서 처리
    }
    //체력회복 코드
    //public void ResetHealth()
    //{
    //    // ex) 최대 체력(5하트라면 5)이 아니라 5하트 * 2 = 10으로 쓸 수도 있음
    //    health = numOfHearts;
    //    // 또는 health = numOfHearts * 2;
    //    UpdateHealthUI();
    //    // 페이드 패널 및 UI 리셋은 필요하다면 여기서도 처리 가능
    //}
}
