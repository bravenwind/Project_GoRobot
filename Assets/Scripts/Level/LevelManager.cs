using UnityEngine;
using System;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [SerializeField]
    private GridMap gridMap;

    [SerializeField]
    private PlayerMovement playerMovement;

    [SerializeField]
    private PlayerBehaviorPlanner playerBehaviorPlanner;

    [SerializeField]
    private DebugUI debugUI;

    [SerializeField]
    private BFSDebugView bfsDebugView;

    [SerializeField]
    private int gatePeriodTicks = 2;

    [SerializeField]
    private int[] plusBehaviorCountPerLevel = new int[6];

    [SerializeField]
    private CanvasGroup fadeImage;

    [SerializeField]
    private GameObject nextLevelButton;

    [SerializeField]
    private GameObject toEndingButton;

    public int Levels { get; private set; } = 0;

    public int CurrentLevel { get; private set; } = 1;

    public int CurrentLevelOptimalBehaviorCount { get; private set; } = 0;

    public CurrentLevelState LevelState { get; private set; }

    public int BehaviorLimit { get; private set; } = 0;

    public int GatePeriodTicks => gatePeriodTicks;

    public CanvasGroup FadeImage => fadeImage;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        fadeImage.alpha = 1f;
        StartCoroutine(FadeUI.Fade(fadeImage, FadeState.FadeIn));

        Levels = gridMap.LevelCount;
        LoadCurrentLevel();
    }

    public void LoadCurrentLevel()
    {
        if (gridMap == null) return;
     
        gridMap.RegisterLevelTile();
        gridMap.SetLevelCamera();

        BFSLevelSolver solver = new BFSLevelSolver(gridMap, gatePeriodTicks);
        Vector3Int startCell = gridMap.GetStartCell();
        Direction startDirection = gridMap.CurrentStartPoint.startDirection;

        var minDistanceMap = solver.DebugGetAllMinDistances(startCell, startDirection);
        bfsDebugView.Show(minDistanceMap, gridMap);

        CurrentLevelOptimalBehaviorCount = solver.Solve(startCell, startDirection);

        if (CurrentLevelOptimalBehaviorCount < 0)
        {
            Debug.LogWarning($"레벨 {CurrentLevel} 클리어 불가.");
        }
        else
        {
            Debug.Log($"레벨 {CurrentLevel} 최적 행동 개수 -> {CurrentLevelOptimalBehaviorCount}");
        }

        BehaviorLimit = CurrentLevelOptimalBehaviorCount + plusBehaviorCountPerLevel[CurrentLevel - 1];

        LevelState = new CurrentLevelState(gatePeriodTicks);
        LevelState.OnStateChanged += RefreshAll;
        LevelState.OnStateChanged += debugUI.UpdateDebugText;

        ResetCurrentLevel();

        StartCoroutine(FadeUI.Fade(fadeImage, FadeState.FadeIn));
    }

    public void ResetCurrentLevel()
    {
        LevelState.Reset();

        foreach (Key key in gridMap.Keys)
        {
            key.KeyObject.SetActive(true);
        }

        if (playerBehaviorPlanner != null) 
        {
            playerBehaviorPlanner.ResetQueue();
        }

        if (playerMovement != null) 
        {
            playerMovement.ResetAndSpawn(gridMap.CurrentStartPoint);
        }

        if (CurrentLevel == Levels)
        {
            if (nextLevelButton != null)
            {
                nextLevelButton.SetActive(false);
            }

            if (toEndingButton != null)
            {
                toEndingButton.SetActive(true);
            }
        }
    }

    public void ClearLevel(int playerBehaviorCount)
    {
        if (CurrentLevelOptimalBehaviorCount < 0) return;

        GameClearUI gameClearUI = UIStateMachine.Instance.GetComponentInChildren<GameClearUI>(true);
        if (gameClearUI != null)
        {
            gameClearUI.SetClearUI(playerBehaviorCount, CurrentLevelOptimalBehaviorCount);
        }

        UIStateMachine.Instance.ReportResult(true);
    }

    private void RefreshAll()
    {
        CurrentLevelState s = LevelState;
        if (s == null) return;

        if (gridMap.Gates == null) return;

        foreach (Gate gate in gridMap.Gates)
        {
            gate.Refresh(s.hasKey, s.switchOn, s.tick, gatePeriodTicks);
        }
    }

    public void NextLevel()
    {
        if (CurrentLevel < Levels)
        {
            CurrentLevel++;

            StartCoroutine(FadeUI.Fade(fadeImage, FadeState.FadeOut, LoadCurrentLevel));
        }
    }

    public void PreviousLevel()
    {
        if (CurrentLevel > 1)
        {
            CurrentLevel--;

            LoadCurrentLevel();
        }
    }
}
