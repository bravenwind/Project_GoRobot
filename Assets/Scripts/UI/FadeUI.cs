using System;
using System.Collections;
using UnityEngine;

public enum FadeState
{
    FadeIn, FadeOut
}

public static class FadeUI
{
    public static float fadeDurationSeconds = 1.5f;

    public static IEnumerator Fade(CanvasGroup fadeImage, FadeState fadeState, Action onComplete = null)
    {
        float t = 0f;

        float startAlpha = fadeImage.alpha;
        float endAlpha = (fadeState == FadeState.FadeIn) ? 0f : 1f;

        while (t < fadeDurationSeconds)
        {
            t += Time.deltaTime;

            fadeImage.alpha = Mathf.Lerp(startAlpha, endAlpha, t / fadeDurationSeconds);

            yield return null;
        }

        fadeImage.alpha = endAlpha;

        onComplete?.Invoke();
    }
}
