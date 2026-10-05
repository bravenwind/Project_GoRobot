using UnityEngine;
using System;

public class CurrentLevelState
{
    public event Action OnStateChanged;

    public bool switchOn = false;
    public bool hasKey = false;

    public int tick = 0;

    private readonly int gatePeriod;

    public bool HasKey
    {
        get
        {
            return hasKey;
        }
        set
        {
            hasKey = value;
            OnStateChanged?.Invoke();
        }
    }

    public CurrentLevelState(int gatePeriod)
    {
        this.gatePeriod = Mathf.Max(1, gatePeriod);
    }

    public bool CanEnter(TileType tile)
    {
        return TileRules.CanEnter(tile, hasKey, switchOn, tick, gatePeriod);
    }

    public void ToggleSwitch()
    {
        switchOn = !switchOn;
        OnStateChanged?.Invoke();
    }

    public void AdvanceTick()
    {
        tick = (tick + 1) % gatePeriod;
        OnStateChanged?.Invoke();
    }

    public void Reset() 
    { 
        switchOn = false; hasKey = false; tick = 0;
        OnStateChanged?.Invoke();
    }
}