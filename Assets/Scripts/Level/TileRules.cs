using UnityEngine;

public static class TileRules
{
    public static bool CanEnter(TileType tile, bool hasKey, bool switchOn, int tick, int gatePeriod)
    {
        switch (tile)
        {
            case TileType.NonWalkable: return false;
            case TileType.Empty: return false;
            case TileType.KeyGate: return hasKey;
            case TileType.SwitchGate_Closed: return switchOn;
            case TileType.SwitchGate_Opened: return !switchOn;
            case TileType.TimingGate: return IsGateOpen(tick, gatePeriod);
            default: return true;
        }
    }

    public static bool IsGateOpen(int tick, int gatePeriod)
    {
        return tick % gatePeriod == 1;
    }


    public static bool IsGameOverTile(TileType tile)
    {
        return tile == TileType.Empty || tile == TileType.NonWalkable;
    }

}
