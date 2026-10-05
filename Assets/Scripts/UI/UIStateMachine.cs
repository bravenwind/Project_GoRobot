using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;

public class UIStateMachine : MonoBehaviour
{
    public static UIStateMachine Instance;

    // ── 상태 정의 ───────────────────────────────────────────────
    public enum UIState
    {
        Planning,    
        Behaving,  
        Cleared,    
        Failed      
    }

    [SerializeField]
    private GameObject planningPanel;  
    
    [SerializeField]
    private GameObject behavingPanel; 
    
    [SerializeField]
    private GameObject clearedPanel;

    [SerializeField]
    private GameObject failedPanel;

    public UnityEvent OnBehavingStarted;
    public UnityEvent OnRetryRequested;
    public UnityEvent OnNextLevelRequested;

    public UIState CurrentState { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        ChangeState(UIState.Planning);
    }

    public void ChangeState(UIState next)
    {
        CurrentState = next;

        bool planning = next == UIState.Planning;
        bool behaving = next == UIState.Behaving;
        bool cleared = next == UIState.Cleared;
        bool failed = next == UIState.Failed;

        if (planningPanel != null) planningPanel.SetActive(planning);
        if (behavingPanel != null) behavingPanel.SetActive(behaving);
        if (clearedPanel != null) clearedPanel.SetActive(cleared);
        if (failedPanel != null) failedPanel.SetActive(failed);

        if (behaving)
        {
            OnBehavingStarted?.Invoke();
        }
    }

    public void PressGo()
    {
        if (CurrentState != UIState.Planning) return;
        ChangeState(UIState.Behaving);
    }

    public void ReportResult(bool isCleared)
    {
        if (isCleared)
        {
            ChangeState(UIState.Cleared);
            PlaySFX.Instance.PlayLevelClear();
        }
        else
        {
            ChangeState(UIState.Failed);
            PlaySFX.Instance.PlayLevelFail();
        }
    }

    public void PressRetry()
    {
        OnRetryRequested?.Invoke();
        ChangeState(UIState.Planning); 
    }

    public void PressNextLevel()
    {
        OnNextLevelRequested?.Invoke();
        ChangeState(UIState.Planning);
    }
}