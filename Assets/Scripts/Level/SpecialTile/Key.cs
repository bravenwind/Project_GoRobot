using UnityEngine;

public class Key : MonoBehaviour
{
    public GameObject KeyObject {get; private set;}

    private void Start()
    {
        KeyObject = transform.GetChild(0).gameObject;
    }

    public void EnableKey()
    {
        if (KeyObject != null)
        {
            KeyObject.SetActive(true);
        }
    }
}
