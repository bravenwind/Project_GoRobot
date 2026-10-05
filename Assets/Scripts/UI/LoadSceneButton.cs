using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneButton : MonoBehaviour
{
    [SerializeField]
    private string loadSceneName;

    private Coroutine fadeCoroutine;

    private bool isLoading = false;

    public void LoadScene()
    {
        if (isLoading) return;
        StartCoroutine(Load(loadSceneName));
    }

    private IEnumerator Load(string sceneName)
    {
        isLoading = true;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        yield return fadeCoroutine = StartCoroutine(FadeUI.Fade(LevelManager.Instance.FadeImage, FadeState.FadeOut));

        SceneManager.LoadScene(sceneName);
    }
}