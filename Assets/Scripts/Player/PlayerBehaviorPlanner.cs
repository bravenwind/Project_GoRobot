using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerBehaviorPlanner : MonoBehaviour
{
    [SerializeField]
    private PlayerMovement playerMovement;

    private List<PlayerBehavior> playerBehaviors = new List<PlayerBehavior>();

    public UnityEvent<PlayerBehavior> OnBehaviorEnqueue;
    public UnityEvent OnBehaviorDequeue;
    public UnityEvent<int> OnQueueCountChanged;

    [VisibleEnum(typeof(PlayerBehavior))]
    public void EnqueueBehavior(int behavior)
    {
        if (playerBehaviors == null) return;
        if (playerBehaviors.Count == LevelManager.Instance.BehaviorLimit) return;

        PlayerBehavior playerBehavior = (PlayerBehavior)behavior;
        playerBehaviors.Add(playerBehavior);

        OnBehaviorEnqueue?.Invoke(playerBehavior);
        OnQueueCountChanged?.Invoke(playerBehaviors.Count);
    }

    public void DequeueBehavior()
    {
        if (playerBehaviors == null) return;

        if (playerBehaviors.Count == 0)
        {
            return;
        }

        playerBehaviors.RemoveAt(playerBehaviors.Count - 1);

        OnBehaviorDequeue?.Invoke();
        OnQueueCountChanged?.Invoke(playerBehaviors.Count);
    }

    public void StartBehaving()
    {
        if (playerMovement != null) 
        {
            playerMovement.BehaveStart(new Queue<PlayerBehavior>(playerBehaviors));
        }
    }

    public void ResetQueue()
    {
        if (playerBehaviors != null) 
        {
            playerBehaviors.Clear();
            OnQueueCountChanged?.Invoke(playerBehaviors.Count);
        }
    }
}
