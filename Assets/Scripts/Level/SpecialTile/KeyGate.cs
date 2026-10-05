using UnityEngine;

public class KeyGate : MonoBehaviour
{
    [SerializeField]
    private GameObject blockObject;

    public void DisableGate()
    {
        blockObject.SetActive(false);
    }
}
