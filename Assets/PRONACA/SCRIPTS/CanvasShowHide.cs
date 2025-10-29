using UnityEngine;
using System.Collections;

public class CanvasShowHide : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private bool disableOnHide = true;

    private Coroutine currentRoutine;

    private void Awake()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Show()
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        gameObject.SetActive(true);
        currentRoutine = StartCoroutine(FadeCanvas(1f, true));
    }

    public void Hide()
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(FadeCanvas(0f, false));
    }

    private IEnumerator FadeCanvas(float targetAlpha, bool makeInteractable)
    {
        float startAlpha = canvasGroup.alpha;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / Mathf.Max(fadeDuration, 0.0001f);
            float a = Mathf.Lerp(startAlpha, targetAlpha, t);
            canvasGroup.alpha = a;
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;

        if (targetAlpha <= 0f)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            if (disableOnHide) gameObject.SetActive(false);
        }
        else
        {
            canvasGroup.interactable = makeInteractable;
            canvasGroup.blocksRaycasts = makeInteractable;
        }

        currentRoutine = null;
    }
}
