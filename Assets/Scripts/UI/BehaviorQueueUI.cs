using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BehaviorQueueUI : MonoBehaviour
{
    [Serializable]
    public struct PlayerBehaviorUI
    {
        public PlayerBehavior behavior;
        public GameObject uiObject;
    }

    [SerializeField]
    private List<PlayerBehaviorUI> uiList = new List<PlayerBehaviorUI>();

    [SerializeField]
    private ScrollRect queueScrollView;

    [SerializeField]
    private Color highlightColor = Color.yellow;

    [SerializeField]
    private float scrollDuration = 0.2f;

    [SerializeField]
    private TMP_Text behaviorCountText;

    private Dictionary<PlayerBehavior, GameObject> playerBehaviorUIDict = new Dictionary<PlayerBehavior, GameObject>();

    private void Awake()
    {
        if (playerBehaviorUIDict == null) return;

        foreach (PlayerBehaviorUI behaviorUI in uiList)
        {
            if (playerBehaviorUIDict.ContainsKey(behaviorUI.behavior))
            {
                continue;
            }

            playerBehaviorUIDict.Add(behaviorUI.behavior, behaviorUI.uiObject);
        }
    }

    public void EnqueueScroll(PlayerBehavior behavior)
    {
        if (playerBehaviorUIDict == null) return;

        GameObject behaviorUI = playerBehaviorUIDict[behavior];
        if (behaviorUI != null)
        {
            if (queueScrollView != null) 
            {
                Instantiate(behaviorUI, queueScrollView.content);
            }
        }
    }

    public void DequeueScroll()
    {
        if (queueScrollView == null) return;

        int index = queueScrollView.content.childCount - 1;
        Transform behaviorUI = queueScrollView.content.GetChild(index);
        Destroy(behaviorUI.gameObject);
    }

    public void HighlightBehavior(int index)
    {
        if (queueScrollView == null || queueScrollView.content.childCount == 0) return;
        if (index >= queueScrollView.content.childCount) return;

        for (int i = 0; i < queueScrollView.content.childCount; i++)
        {
            Transform child = queueScrollView.content.GetChild(i);
            Image childImage = child.gameObject.GetComponent<Image>();

            if (childImage == null) return;
            
            if (i == index)
            {
                childImage.color = highlightColor;
            }
            else
            {
                childImage.color = Color.white;
            }
        }

        StartCoroutine(ScrollToItem(index));
    }


    private IEnumerator ScrollToItem(int index)
    {
        Canvas.ForceUpdateCanvases();

        int totalItems = queueScrollView.content.childCount;

        if (totalItems <= 1) yield break;

        float targetNormalizedPos = (float)index / (totalItems - 1);
        targetNormalizedPos = Mathf.Clamp01(targetNormalizedPos);

        float currentPos = queueScrollView.horizontalNormalizedPosition;
        float t = 0f;

        while (t < scrollDuration)
        {
            t += Time.deltaTime;
            queueScrollView.horizontalNormalizedPosition = Mathf.Lerp(currentPos, targetNormalizedPos, t / scrollDuration);
            yield return null;
        }

        queueScrollView.horizontalNormalizedPosition = targetNormalizedPos;
    }

    public void UpdateQueueCount(int current)
    {
        if (behaviorCountText != null)
        {
            behaviorCountText.text = $"{current} / {LevelManager.Instance.BehaviorLimit}";
        }
    }

    public void ResetUI()
    {
        if (queueScrollView == null) return;

        for (int i = queueScrollView.content.childCount - 1; i >= 0; i--)
        {
            Destroy(queueScrollView.content.GetChild(i).gameObject);
        }
    }
}
