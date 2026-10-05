using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    [SerializeField]
    private Transform switchTransform;

    [SerializeField]
    private float stepOffY = 0.9f;

    [SerializeField]
    private float stepOnY = 0.8f;

    [SerializeField]
    private float moveDuration = 1f;

    private Coroutine moveCoroutine;
    private Queue<Coroutine> moveCoroutineQueue;

    private void Start()
    {
        SetY(stepOffY);
    }

    public void StepOnSwitch()
    {
        StartMove(stepOnY);
    }

    public void StepOffSwitch() 
    {
        StartMove(stepOffY);
    }

    private void StartMove(float targetY)
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        moveCoroutine = StartCoroutine(Move(targetY));
    }

    private IEnumerator Move(float targetY)
    {
        float startY = switchTransform.position.y;

        float distance = Mathf.Abs(targetY - startY);
        float fullDistance = Mathf.Abs(stepOnY - stepOffY);
        float duration = moveDuration * (fullDistance > 0 ? distance / fullDistance : 1f);

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float y = Mathf.Lerp(startY, targetY, t / duration);
            SetY(y);
            yield return null;
        }

        SetY(targetY);
        moveCoroutine = null;
    }

    private void SetY(float y)
    {
        Vector3 p = switchTransform.position;
        switchTransform.position = new Vector3(p.x, y, p.z);
    }
}
