using UnityEngine;
using UnityEngine.UI;

public class BehaviorButtons : MonoBehaviour
{
    [SerializeField]
    private GameObject[] buttonPanels;

    [SerializeField]
    private Button[] enableButtons;

    public void DisableBehaviorButtons()
    {
        foreach (GameObject panel in buttonPanels)
        {
            panel.SetActive(false);
        }

        foreach (Button button in enableButtons)
        {
            button.interactable = false;
        }
    }

    public void EnableBehaviorButtons()
    {
        foreach (Button button in enableButtons)
        {
            button.interactable = true;
        }
    }
}
