using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class Health : MonoBehaviour
{
    // ���� ü�� (5)
    public float health = 5.0f;
    // �ִ� ü�¿� �´� ��Ʈ �� (ü�� 5�� ���缭 5���� ��Ʈ)
    public int numOfHearts = 5;

    // ��Ʈ �̹�����
    public Image[] hearts;
    // �� �� ��Ʈ
    public Sprite fullHeart;
    // �� ��Ʈ
    public Sprite halfHeart;
    // �� ��Ʈ
    public Sprite emptyHeart;
    // =========================================
    // ���� ���� UI �� ���̵� ����
    // =========================================
    [Header("Game Over / Fade UI")]
    public CanvasGroup fadeCanvasGroup; // ȭ���� ������ ���� �г�(���İ� ����) 
    public GameObject gameOverUI;       // ���ӿ��� UI �г�(��ư ����)
    public float fadeDuration = 1.5f;   // ȭ���� ������ ��ο����� �� �ɸ��� �ð�

    // �̹� ���ӿ��� �������� ���� ������ üũ�� ����
    private bool isDead = false;


    public void TakeDamage(float damage)
    {
        // �̹� ��� ���� ���¶�� ������ ����� �����ϰų�,
        // Ȥ�� �߰��� ����� �������� ���� ���� ����
        if (isDead) return;

        health -= damage;
        if (health < 0)
        {
            health = 0;
        }
        // ü���� 0�� �Ǹ� ���� ���� ó��
        if (health <= 0 && !isDead)
        {
            isDead = true; // ��� ó�� �÷��� ����
            StartCoroutine(GameOverSequence());
        }
        UpdateHealthUI();
    }

    private void Update()
    {
        // �ִ� ü���� ���� �ʵ��� ����
        if (health > numOfHearts * 2) // numOfHearts�� ü�� �� ĭ�� ǥ���ϹǷ� �ִ�� 2��
        {
            health = numOfHearts * 2;  // �ִ� ü���� ���� �ʵ���
        }
        UpdateHealthUI();
    }

    // ü�� UI ������Ʈ �Լ�
    public void UpdateHealthUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            // health�� 0.5 ������ �پ�� ������ �� ĭ�� ǥ���� �� �ֵ��� ó��
            if (i < Mathf.FloorToInt(health)) // �� �� ��Ʈ
            {
                hearts[i].sprite = fullHeart;
            }
            else if (i == Mathf.FloorToInt(health) && health % 1 != 0) // �� ��Ʈ (0.5 ������ �� ĭ�� ǥ��)
            {
                hearts[i].sprite = halfHeart;
            }
            else // �� ��Ʈ
            {
                hearts[i].sprite = emptyHeart;
            }

            // ��Ʈ�� Ȱ��ȭ ���� ����
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
    // ���� ���� ������(�ڷ�ƾ)
    // =========================================
    IEnumerator GameOverSequence()
    {
        // 1) ȭ���� ������ ��Ӱ� �����
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
            fadeCanvasGroup.alpha = 1f; // ������ ��ο���
        }

        // 2) ��ο��� ��, ���� ���� UI ǥ��
        if (gameOverUI)
        {
            gameOverUI.SetActive(true);
            
        }
        // 3) �ð� ����
        Time.timeScale = 0;
        // ���Ĵ� ��ư Ŭ�� �� ����(GameManager ���� ���̺�����Ʈ �ε� or ���θ޴�)���� ó��
    }
    //ü��ȸ�� �ڵ�
    //public void ResetHealth()
    //{
    //    // ex) �ִ� ü��(5��Ʈ��� 5)�� �ƴ϶� 5��Ʈ * 2 = 10���� �� ���� ����
    //    health = numOfHearts;
    //    // �Ǵ� health = numOfHearts * 2;
    //    UpdateHealthUI();
    //    // ���̵� �г� �� UI ������ �ʿ��ϴٸ� ���⼭�� ó�� ����
    //}
}
