using UnityEngine;
using TMPro;

public class PathfinderCanvas : MonoBehaviour
{
    [SerializeField] private PlayerController playerController; // Reference to the PlayerController
    [SerializeField] private TextMeshProUGUI modeText;

    private bool isBig;

    private void Start()
    {
        // Initialize the text with the current pathfinding mode
        UpdateModeText();
    }

    // Called when the button is clicked
    public void OnSwitchModeButtonClicked()
    {
        // Toggle between A* and D*
        if (playerController.PathfindingMode == PathfindingMode.AStar)
        {
            playerController.SetPathfindingMode(PathfindingMode.DStar);
        }
        else
        {
            playerController.SetPathfindingMode(PathfindingMode.AStar);
        }

        // Update the UI text
        UpdateModeText();
    }

    // Update the UI text to reflect the current pathfinding mode
    private void UpdateModeText()
    {
        if (playerController.PathfindingMode == PathfindingMode.AStar)
        {
            modeText.text = "A*";
        }
        else
        {
            modeText.text = "D*";
        }
    }

    public void ChangeToBigCamera()
    {
        isBig = !isBig;

        if (isBig)
        {
            Camera.main.transform.position = new Vector3(4.5f, 4.5f, -10.0f);
            Camera.main.orthographicSize = 10.0f;
        }
        else 
        {
            Camera.main.transform.position = new Vector3(0.0f, 0.0f, -10.0f);
            Camera.main.orthographicSize = 5.0f;
        }

    }
}
