using UnityEngine;
using TMPro;
public enum CellType
{
    Walkable,
    Wall
}

public class GridCell : MonoBehaviour
{
    public Vector2Int gridPos;
    public CellType cellType;

    private float _gCost;
    private float _hCost;
    private float _fCost;

    // Properties for costs
    public float GCost
    {
        get => _gCost;
        set
        {
            _gCost = value;
            UpdateCostText(gCostText, _gCost);
        }
    }

    public float HCost
    {
        get => _hCost;
        set
        {
            _hCost = value;
            UpdateCostText(hCostText, _hCost);
        }
    }

    public float FCost
    {
        get => _fCost;
        set
        {
            _fCost = value;
            UpdateCostText(fCostText, _fCost);
        }
    }
    // Parent node for path reconstruction
    public GridCell parent;

    [SerializeField]
    private TextMeshProUGUI gCostText;

    [SerializeField]
    private TextMeshProUGUI hCostText;

    [SerializeField]
    private TextMeshProUGUI fCostText;

    public void Initialize(Vector2Int position, CellType type)
    {
        gridPos = position;
        cellType = type;
        ResetPathfindingData();
    }

    public void ResetPathfindingData()
    {
        GCost = float.MaxValue;
        FCost = float.MaxValue;
        parent = null;
    }

    private void UpdateCostText(TextMeshProUGUI textField, float value)
    {
        if (textField != null)
        {
            textField.text = value == float.MaxValue ? "infi" : value.ToString("F1");
        }
    }
}