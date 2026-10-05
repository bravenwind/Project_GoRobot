using TMPro;
using UnityEngine;


public class DebugUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text currentStateText;

    [SerializeField]
    private GameObject debugObject;

    public void UpdateDebugText()
    {
        if (currentStateText != null) 
        {
            currentStateText.text = $"Tick : {LevelManager.Instance.LevelState.tick}\n" +
                                    $"HasKey : {LevelManager.Instance.LevelState.HasKey}\n" +
                                    $"SwitchOn : { LevelManager.Instance.LevelState.switchOn}";
        }
    }

    public void DebugNextLevel()
    {
        LevelManager.Instance.NextLevel();
    }

    public void DebugPreviousLevel()
    {
        LevelManager.Instance.PreviousLevel();
    }

    public void ToggleDebugUI()
    {
        debugObject.SetActive(!debugObject.activeSelf);
    }
}

