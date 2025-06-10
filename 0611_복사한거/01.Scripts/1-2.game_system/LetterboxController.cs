using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LetterboxController : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("상단 검은 테두리")]
    public RectTransform topBar;
    [Tooltip("하단 검은 테두리")]
    public RectTransform bottomBar;

    private Coroutine currentRoutine;

    public void ShowLetterbox(float speed = 0.5f)
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(LerpBars(0f, 100f, speed));
    }

    public void HideLetterbox(float speed = 0.5f)
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(LerpBars(100f, 0f, speed));
    }

    private IEnumerator LerpBars(float from, float to, float duration)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / duration;
            float y = Mathf.Lerp(from, to, t);
            topBar.sizeDelta = new Vector2(0, y);
            bottomBar.sizeDelta = new Vector2(0, y);
            yield return null;
        }
    }
}
