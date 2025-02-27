using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerController : MonoBehaviour
{
    private Vector2Int playerPosition;
    private float spacing;
    private int height;

    public void SetGridParameters(float gridSpacing, int gridHeight)
    {
        spacing = gridSpacing;
        height = gridHeight;
        playerPosition = new Vector2Int(1, 1);
        UpdatePlayerPosition();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit)
            {
                if (hit.collider.CompareTag("Walkable"))
                {
                    string[] nameParts = hit.collider.gameObject.name.Split('_');
                    if (nameParts.Length == 3 && nameParts[0] == "Cell")
                    {
                        int x = int.Parse(nameParts[1]);
                        int y = int.Parse(nameParts[2]);
                        MovePlayer(x, y);
                    }
                }
            }
        }
    }

    void MovePlayer(int x, int y)
    {
        playerPosition = new Vector2Int(x, y);
        UpdatePlayerPosition();
    }

    void UpdatePlayerPosition()
    {
        transform.position = new Vector3(playerPosition.x * spacing, playerPosition.y * spacing, 0.0f);
    }
}
