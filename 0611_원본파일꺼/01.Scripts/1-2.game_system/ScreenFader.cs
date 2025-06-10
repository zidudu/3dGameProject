using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader instance;

    public Image fadeImage; // ���� �̹���

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // �� ������Ʈ�� ���� �ٲ� �ı����� �ʰ� ��
        }
        else
        {
            Destroy(gameObject); // �ߺ� ����
        }
    }


    public IEnumerator FadeOut(float duration)
    {
        fadeImage.gameObject.SetActive(true);
        Color color = fadeImage.color;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            color.a = t / duration;
            fadeImage.color = color;
            yield return null;
        }

        color.a = 1;
        fadeImage.color = color;
    }

    public IEnumerator FadeIn(float duration)
    {
        Color color = fadeImage.color;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            color.a = 1 - (t / duration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 0;
        fadeImage.color = color;
        fadeImage.gameObject.SetActive(false);
    }
}
