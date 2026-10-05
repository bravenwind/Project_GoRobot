using TMPro;
using UnityEngine;

public class GameClearUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text optimalBehaviorCountText;

    [SerializeField]
    private TMP_Text myBehaviorCountText;

    public void SetClearUI(int myCount, int optimalCount)
    {
        if (optimalBehaviorCountText != null) 
        {
            optimalBehaviorCountText.text = $"Optimal Behavior Count : {optimalCount}";
        }

        if (myBehaviorCountText != null)
        {
            myBehaviorCountText.text = $"My Behavior Count : {myCount}";
        }
    }
}
