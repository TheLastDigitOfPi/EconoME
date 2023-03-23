using UnityEngine;

/*
 * Extensions allow methods to be added to protected classes so common tasks
 * can be easily replicated such as Converting a Vector3 to a Vector 2
 */
public static class UnsortedExtensions
{
    public static void Toggle(this ref bool flag)
    {
        flag = !flag;
    }

    public static void ToggleActive(this GameObject gameObject)
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public static void KillChildren(this Transform t)
    {
        foreach (Transform child in t)
        {
            Object.Destroy(child.gameObject);
        }
    }

    public static Rect GetWorldRect(this RectTransform rectTransform)
    {
        // This returns the world space positions of the corners in the order
        // [0] bottom left,
        // [1] top left
        // [2] top right
        // [3] bottom right
        var corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        Vector2 min = corners[0];
        Vector2 max = corners[2];
        Vector2 size = max - min;

        return new Rect(min, size);
    }

    public static Direction GridToDirection(this GridNeighborPos pos)
    {
        switch (pos)
        {
            case GridNeighborPos.TopLeft:
                return Direction.Left;
            case GridNeighborPos.Top:
                return Direction.Up;
            case GridNeighborPos.TopRight:
                return Direction.Right;
            case GridNeighborPos.Left:
                return Direction.Left;
            case GridNeighborPos.Right:
                return Direction.Right;
            case GridNeighborPos.BottomLeft:
                return Direction.Left;
            case GridNeighborPos.BottomRight:
                return Direction.Right;
            case GridNeighborPos.Bottom:
                return Direction.Down;
            default:
                return Direction.Right;
        }
    }

    public static Direction Inverse(this Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
                return Direction.Right;
            case Direction.Right:
                return Direction.Left;
            case Direction.Up:
                return Direction.Down;
            case Direction.Down:
                return Direction.Up;
            default:
                return Direction.Right;
        }
    }
}
