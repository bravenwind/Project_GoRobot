using UnityEngine;

public enum Direction
{
    North,
    East,
    South,
    West
}

public static class DirectionExtensions
{
    public static Quaternion DirectionToRotation(Direction direction)
    {
        switch (direction)
        {
            case Direction.North:
                return Quaternion.LookRotation(Vector3.forward);
            case Direction.East:
                return Quaternion.LookRotation(Vector3.right);
            case Direction.South:
                return Quaternion.LookRotation(Vector3.back);
            case Direction.West:
                return Quaternion.LookRotation(Vector3.left);
            default:
                return Quaternion.identity;
        }
    }

    static readonly Vector2Int[] moveDeltas =
    {
        new Vector2Int(0, 1),   // North
        new Vector2Int(1, 0),   // East
        new Vector2Int(0, -1),  // South
        new Vector2Int(-1, 0),  // West
    };

    public static Vector2Int DirectionToMoveDelta(Direction d)
    {
        return moveDeltas[(int)d];
    }

    public static Direction RotateClockwise(Direction d)
    {
        return (Direction)(((int)d + 1) % 4);
    }

    public static Direction RotateCounterClockwise(Direction d)
    {
        return (Direction)(((int)d + 3) % 4);
    }
}



