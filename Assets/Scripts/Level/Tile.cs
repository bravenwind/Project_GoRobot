using UnityEngine;

public enum TileType
{
    Empty,
    Walkable,
    NonWalkable,
    Key,
    KeyGate,
    Ice,
    Switch,
    SwitchGate_Closed,
    SwitchGate_Opened,
    TimingGate,
    Start,
    Goal
}

public class Tile : MonoBehaviour
{
    public TileType type;
}
