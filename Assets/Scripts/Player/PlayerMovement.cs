using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float startY = 1.0f;

    [SerializeField]
    private float moveDistance = 1.0f;

    [SerializeField]
    private float behaveDurationSeconds = 0.5f;

    [SerializeField]
    private float fallDistance = 1.0f;

    [SerializeField]
    private float fallDuration = 1.0f;

    [SerializeField]
    private float flipDuration = 1.0f;

    [SerializeField]
    private Transform keyPivot;

    [SerializeField]
    private float keyScale;

    [SerializeField]
    private GridMap gridMap;

    private CharacterController characterController;
    private bool isFailed = false;

    private int executedBehaviorCount = 0;

    private Vector3 originalScale = Vector3.zero;

    public UnityEvent<int> onBehaviorChanged;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        originalScale = transform.localScale;
    }

    public void BehaveStart(Queue<PlayerBehavior> playerBehaviors)
    {
        StartCoroutine(COBehaveStart(playerBehaviors));
    }

    private IEnumerator COBehaveStart(Queue<PlayerBehavior> playerBehaviors)
    {
        executedBehaviorCount = 0;

        while (playerBehaviors.Count > 0)
        {
            if (isFailed) yield break;

            PlayerBehavior behavior = playerBehaviors.Dequeue();
            executedBehaviorCount++;

            onBehaviorChanged?.Invoke(executedBehaviorCount - 1);
            LevelManager.Instance.LevelState.AdvanceTick();

            switch (behavior)
            {
                case PlayerBehavior.Proceed:
                    PlaySFX.Instance.PlayMove();
                    yield return StartCoroutine(Proceed());
                    break;
                case PlayerBehavior.RotateClockwise:
                    PlaySFX.Instance.PlayRotate();
                    yield return StartCoroutine(RotateClockwise());
                    break;
                case PlayerBehavior.RotateCounterClockwise:
                    PlaySFX.Instance.PlayRotate();
                    yield return StartCoroutine(RotateCounterClockwise());
                    break;
                case PlayerBehavior.Grab:
                    TryGrabKey();
                    yield return new WaitForSeconds(behaveDurationSeconds);
                    break;
                case PlayerBehavior.Wait:
                    yield return new WaitForSeconds(behaveDurationSeconds);
                    break;
            }
        }

        JudgeFinalPosition();
    }

    private IEnumerator Proceed()
    {
        yield return StartCoroutine(MoveOneTile());

        while (JudgeTileType(transform.position) == TileType.Ice)
        {
            if (isFailed) yield break;

            LevelManager.Instance.LevelState.AdvanceTick();
            yield return StartCoroutine(MoveOneTile());
        }

        TileType arrivedTile = JudgeTileType(transform.position);

        if (arrivedTile == TileType.Switch) 
        {
            LevelManager.Instance.LevelState.ToggleSwitch();
        }

        yield break;
    }

    private IEnumerator MoveOneTile()
    {
        Vector3 originalPos = transform.position;
        Vector3 finalPos = originalPos + transform.forward * moveDistance;

        TileType currentTile = JudgeTileType(originalPos);
        TileType frontTile = JudgeTileType(finalPos);

        var state = LevelManager.Instance.LevelState;
        int gatePeriod = LevelManager.Instance.GatePeriodTicks;

        if (TileRules.IsGameOverTile(frontTile))
        {
            if (currentTile == TileType.Switch)
            {
                StepOffSwitch(originalPos);
            }

            yield return Move(behaveDurationSeconds, transform.forward * (moveDistance / behaveDurationSeconds));
            transform.position = finalPos;

            isFailed = true;
            StartCoroutine(Fall(frontTile));

            UIStateMachine.Instance.ChangeState(UIStateMachine.UIState.Failed);
            yield break;
        }

        bool canEnter = TileRules.CanEnter(frontTile, state.HasKey, state.switchOn, state.tick, gatePeriod);

        float halfSeconds = behaveDurationSeconds * 0.5f;
        float speed = (moveDistance * 0.5f) / halfSeconds;

        yield return Move(halfSeconds, transform.forward * speed);

        if (!canEnter)
        {
            isFailed = true;
            StartCoroutine(GoBackAndFlip(originalPos));

            UIStateMachine.Instance.ChangeState(UIStateMachine.UIState.Failed);
            yield break;
        }
        else
        {
            if (currentTile == TileType.Switch)
            {
                StepOffSwitch(originalPos);
            }

            if (frontTile == TileType.KeyGate)
            {
                UseKey(finalPos);
            }
        }

        if (frontTile == TileType.Goal)
        {
            yield return MoveWithShrink(halfSeconds, transform.forward * speed, transform.localScale, transform.localScale * 0.5f);
        }
        else
        {
            yield return Move(halfSeconds, transform.forward * speed);
        }

        if (canEnter && frontTile == TileType.Switch)
        {
            StepOnSwitch(finalPos);
        }

        characterController.enabled = false;
        transform.position = new Vector3(finalPos.x, startY, finalPos.z);
        characterController.enabled = true;
    }

    private IEnumerator Move(float seconds, Vector3 velocity)
    {
        float t = 0f;
        while (t <= seconds)
        {
            t += Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);

            yield return null;
        }
    }

    private IEnumerator MoveWithShrink(float seconds, Vector3 velocity, Vector3 fromScale, Vector3 toScale)
    {
        float t = 0f;
        while (t <= seconds)
        {
            t += Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);

            transform.localScale = Vector3.Lerp(fromScale, toScale, t / seconds);
            yield return null;
        }
        transform.localScale = toScale;
    }

    private IEnumerator Fall(TileType tile)
    {
        float dist = fallDistance;
        float duration = fallDuration;

        if (tile == TileType.Empty)
        {
            dist = fallDistance * 100f;
            duration = 10f;
        }

        characterController.enabled = false;

        Vector3 start = transform.position;
        Vector3 end = start + Vector3.down * dist;

        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float k = t / duration;

            transform.position = Vector3.Lerp(start, end, k * k);

            yield return null;
        }

        transform.position = end;
    }

    private IEnumerator GoBackAndFlip(Vector3 to)
    {
        float duration = flipDuration;

        characterController.enabled = false;

        Vector3 startPos = transform.position;
        Vector3 endPos = to;

        Quaternion startRot = transform.rotation;
        Quaternion endRot = transform.rotation * Quaternion.Euler(-90, 0, 0);

        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;

            transform.position = Vector3.Lerp(startPos, endPos, t / duration);
            transform.rotation = Quaternion.Slerp(startRot, endRot, t / duration);

            yield return null;
        }

        transform.position = endPos;
        transform.rotation = endRot;
    }

    private IEnumerator RotateClockwise()
    {
        float t = 0f;
        Quaternion startRot = transform.rotation;

        float targetY = Mathf.Round((startRot.eulerAngles.y + 90f) / 90f) * 90f;
        Quaternion finalRot = Quaternion.Euler(0, targetY, 0);

        while (t <= behaveDurationSeconds)
        {
            t += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(startRot, finalRot, t / behaveDurationSeconds);
            yield return null;
        }

        transform.rotation = finalRot;

        yield break;
    }

    private IEnumerator RotateCounterClockwise()
    {
        float t = 0f;
        Quaternion startRot = transform.rotation;

        float targetY = Mathf.Round((startRot.eulerAngles.y - 90f) / 90f) * 90f;
        Quaternion finalRot = Quaternion.Euler(0, targetY, 0);

        while (t <= behaveDurationSeconds)
        {
            t += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(startRot, finalRot, t / behaveDurationSeconds);
            yield return null;
        }

        transform.rotation = finalRot;
        yield break;
    }

    private void TryGrabKey()
    {
        if (LevelManager.Instance.LevelState.HasKey) return;

        if (JudgeTileType(transform.position) == TileType.Key)
        {
            Tile keyTile = gridMap.GetTile(transform.position);
            if (keyTile == null) return;
            
            Key key = keyTile.transform.GetComponent<Key>();
            if (key != null)
            {
                GrabKey(key.KeyObject);
            }
        }
    }

    private void GrabKey(GameObject key)
    {
        PlaySFX.Instance.PlayCollectKey();
        key.SetActive(false);
        keyPivot.gameObject.SetActive(true);
        LevelManager.Instance.LevelState.HasKey = true;
    }

    private void UseKey(Vector3 pos)
    {
        LevelManager.Instance.LevelState.HasKey = false;
        keyPivot.gameObject.SetActive(false);

        Tile gateTile = gridMap.GetTile(pos);
        if (gateTile == null) return;

        KeyGate gate = gateTile.transform.GetComponent<KeyGate>();
        if (gate != null)
        {
            gate.DisableGate();
        }
    }

    private void StepOnSwitch(Vector3 pos)
    {
        Tile switchTile = gridMap.GetTile(pos);
        if (switchTile == null) return;

        Switch _switch = switchTile.transform.GetComponent<Switch>();
        if (_switch != null)
        {
            _switch.StepOnSwitch();
        }
    }

    private void StepOffSwitch(Vector3 pos)
    {
        Tile switchTile = gridMap.GetTile(pos);
        if (switchTile == null) return;

        Switch _switch = switchTile.transform.GetComponent<Switch>();
        if (_switch != null)
        {
            _switch.StepOffSwitch();
        }
    }

    private TileType JudgeTileType(Vector3 pos)
    {
        if (gridMap.TryGetTileType(pos, out TileType type))
        {
            return type;
        }
        else
        {
            return TileType.Empty;
        }
    }

    private void JudgeFinalPosition()
    {
        TileType finalTile = JudgeTileType(transform.position);

        if (finalTile == TileType.Goal)
        {
            LevelManager.Instance.ClearLevel(executedBehaviorCount);
        }
        else
        {
            UIStateMachine.Instance.ChangeState(UIStateMachine.UIState.Failed);
        }
    }

    public void ResetAndSpawn(StartPoint sp)
    {
        if (sp == null) return;
        isFailed = false;
        StopAllCoroutines();

        Vector3 pos = new Vector3(sp.transform.position.x, startY, sp.transform.position.z);
        Quaternion rot = DirectionExtensions.DirectionToRotation(sp.startDirection);
        transform.localScale = originalScale;

        characterController.enabled = false;
        transform.SetPositionAndRotation(pos, rot);
        characterController.enabled = true;

        keyPivot.gameObject.SetActive(false);
    }
}
