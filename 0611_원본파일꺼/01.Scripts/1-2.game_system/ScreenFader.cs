using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader instance;

    public Image fadeImage; // 검정 이미지

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 이 오브젝트를 씬이 바뀌어도 파괴되지 않게 함
        }
        else
        {
            Destroy(gameObject); // 중복 방지
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
