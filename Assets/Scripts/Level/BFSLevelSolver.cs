using System.Collections.Generic;
using UnityEngine;

public class BFSLevelSolver
{
    public BFSLevelSolver(GridMap grid, int period)
    {
        gridMap = grid;
        gatePeriod = Mathf.Max(1, period);
    }

    private readonly struct SearchState : System.IEquatable<SearchState>
    {
        public readonly Vector2Int pos;
        public readonly Direction facing;
        public readonly bool hasKey;
        public readonly bool switchOn;
        public readonly int tick;

        public SearchState(Vector2Int pos, Direction facing, bool hasKey, bool switchOn, int tick)
        {
            this.pos = pos;
            this.facing = facing;
            this.hasKey = hasKey;
            this.switchOn = switchOn;
            this.tick = tick;
        }

        public bool Equals(SearchState other)
        {
            return pos == other.pos
                && facing == other.facing
                && hasKey == other.hasKey
                && switchOn == other.switchOn
                && tick == other.tick;
        }

        public override int GetHashCode()
        {
            return System.HashCode.Combine(pos, facing, hasKey, switchOn, tick);
        }

        public override bool Equals(object obj)
        {
            return obj is SearchState s && Equals(s);
        }
    }

    private GridMap gridMap;
    private int gatePeriod;

    public int Solve(Vector3Int startCell, Direction startFacing)
    {
        RunBFS(startCell, startFacing, out int goalDistance);
        return goalDistance;
    }

    public Dictionary<Vector2Int, int> DebugGetAllMinDistances(Vector3Int startCell, Direction startFacing)
    {
        var dist = RunBFS(startCell, startFacing, out _);

        var cellDist = new Dictionary<Vector2Int, int>();

        foreach (var pair in dist)
        {
            Vector2Int cell = pair.Key.pos;
            if (!cellDist.ContainsKey(cell) || pair.Value < cellDist[cell])
                cellDist[cell] = pair.Value;
        }
        return cellDist;
    }

    private Dictionary<SearchState, int> RunBFS(Vector3Int startCell, Direction startFacing, out int goalDist)
    {
        Vector2Int startPos = new Vector2Int(startCell.x, startCell.y);
        SearchState startState = new SearchState(startPos, startFacing, false, false, 0);

        Queue<SearchState> frontier = new();
        Dictionary<SearchState, int> dist = new();
        goalDist = -1;

        frontier.Enqueue(startState);
        dist[startState] = 0;

        while (frontier.Count > 0)
        {
            SearchState currentState = frontier.Dequeue();
            int currentDist = dist[currentState];

            if (IsTypeTile(currentState.pos, TileType.Goal))
            {
                if (goalDist == -1)
                {
                    goalDist = currentDist;
                }
            }

            foreach (SearchState nextState in GetNextStates(currentState))
            {
                if (!dist.ContainsKey(nextState))
                {
                    dist[nextState] = currentDist + 1;
                    frontier.Enqueue(nextState);
                }
            }
        }

        return dist;
    }

    private IEnumerable<SearchState> GetNextStates(SearchState s)
    {
        int nextTick = (s.tick + 1) % gatePeriod;

        Vector2Int proceedPos = s.pos + DirectionExtensions.DirectionToMoveDelta(s.facing);
        Vector3Int proceedCell = new Vector3Int(proceedPos.x, proceedPos.y, 0);

        if (gridMap.TryGetTileType(proceedCell, out TileType proceedTile))
        {
            if (TileRules.CanEnter(proceedTile, s.hasKey, s.switchOn, nextTick, gatePeriod))
            {
                if (proceedTile == TileType.Ice)
                {
                    if (TryIceSlide(proceedPos, s.facing, out Vector2Int landPos, out int slideCount))
                    {
                        int slideTick = (nextTick + slideCount) % gatePeriod;
                        yield return new SearchState(landPos, s.facing, s.hasKey, s.switchOn, slideTick);
                    }
                }
                else
                {
                    bool nextSwitch = (proceedTile == TileType.Switch) ? !s.switchOn : s.switchOn;

                    yield return new SearchState(proceedPos, s.facing, s.hasKey, nextSwitch, nextTick);
                }
            }
        }

        yield return new SearchState(s.pos, DirectionExtensions.RotateClockwise(s.facing), s.hasKey, s.switchOn, nextTick);

        yield return new SearchState(s.pos, DirectionExtensions.RotateCounterClockwise(s.facing), s.hasKey, s.switchOn, nextTick);

        if (IsTypeTile(s.pos, TileType.Key) && !s.hasKey)
        {
            yield return new SearchState(s.pos, s.facing, true, s.switchOn, nextTick);
        }

        yield return new SearchState(s.pos, s.facing, s.hasKey, s.switchOn, nextTick);

        yield break;
    }

    private bool TryIceSlide(Vector2Int from, Direction facing, out Vector2Int result, out int slideCount)
    {
        Vector2Int current = from;
        slideCount = 0;

        while (IsTypeTile(current, TileType.Ice))
        {
            Vector2Int next = current + DirectionExtensions.DirectionToMoveDelta(facing);
            if (!TileExists(next) || IsTypeTile(next, TileType.NonWalkable))
            {
                result = current;
                return false;
            }
            current = next;
            slideCount++;
        }
        result = current;
        return true;
    }

    private bool IsTypeTile(Vector2Int tilePos, TileType tileType)
    {
        Vector3Int cell = new Vector3Int(tilePos.x, tilePos.y, 0);
        if (gridMap.TryGetTileType(cell, out TileType type))
            return type == tileType;
        return false;
    }

    private bool TileExists(Vector2Int tilePos)
    {
        Vector3Int cell = new Vector3Int(tilePos.x, tilePos.y, 0);
        return gridMap.TryGetTileType(cell, out TileType type);
    }
}