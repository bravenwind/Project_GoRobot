using UnityEngine;

public class Gate : MonoBehaviour
{
    private TileType gateType;

    [SerializeField]
    private GameObject blockObject;

    private void Awake()
    {
        gateType = GetComponent<Tile>().type;
    }

    public void Refresh(bool hasKey, bool switchOn, int tick, int gatePeriod)
    {
        bool open = TileRules.CanEnter(gateType, hasKey, switchOn, tick, gatePeriod);
        if (blockObject != null)
            blockObject.SetActive(!open);
    }
}
