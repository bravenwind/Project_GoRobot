using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BFSDebugView : MonoBehaviour
{
    [SerializeField] 
    private GameObject textPrefab;

    [SerializeField]
    private Transform textParent;

    [SerializeField]
    private Vector3 posOffset;

    private List<GameObject> spawned = new();

    public void Show(Dictionary<Vector2Int, int> distMap, GridMap gridMap)
    {
        Clear();
        foreach (var pair in distMap)
        {
            Vector2Int cell = pair.Key;
            int distance = pair.Value;

            // 셀 좌표 → 월드 위치 (GridMap의 grid로 변환)
            Vector3 worldPos = gridMap.CellToWorld(cell) + posOffset;

            GameObject textObj = Instantiate(textPrefab, worldPos, Quaternion.identity, transform);
            textObj.transform.SetParent(textParent);
            TMP_Text text = textObj.GetComponent<TMP_Text>();
            text.text = distance.ToString();
            text.rectTransform.rotation = Quaternion.Euler(90, 0, 0);
            spawned.Add(textObj);
        }
    }

    public void Clear()
    {
        foreach (var obj in spawned) Destroy(obj);
        spawned.Clear();
    }

    public void ToggleDebugBFS()
    {
        textParent.gameObject.SetActive(!textParent.gameObject.activeSelf);
    }
}