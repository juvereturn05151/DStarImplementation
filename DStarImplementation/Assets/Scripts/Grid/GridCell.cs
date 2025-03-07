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

    public void SetCosts(float gCost, float hCost, float fCost)
    {
        GCost = gCost;
        HCost = hCost;
        FCost = fCost;
        UpdateCostText();
    }

    private void UpdateCostText()
    {
        if (gCostText != null)
        {
            gCostText.text = GCost == float.MaxValue ? "infi" : GCost.ToString("F1");
        }
        if (hCostText != null)
        {
            hCostText.text = HCost == float.MaxValue ? "infi" : HCost.ToString("F1");
        }
        if (fCostText != null)
        {
            fCostText.text = FCost == float.MaxValue ? "infi" : FCost.ToString("F1");
        }
    }
}