using UnityEngine;

public static class VectorExtensions
{
    public static Vector3 ToVector3(this Vector2 vec2)
    {
        return new Vector3(vec2.x, vec2.y, 0);
    }
    public static Vector2Int ToVector2Int(this Vector3Int vector3)
    {
        return new Vector2Int(vector3.x, vector3.y);
    }

    public static Vector2 ToVector2(this Vector3 vector3)
    {
        return new Vector2(vector3.x, vector3.y);
    }

    public static Vector2 RadianToVector2(this float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    public static Vector2 DegreeToVector2(this float degree)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }

    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;

        Vector2 newVec = new Vector2((cos * tx) - (sin * ty), (sin * tx) + (cos * ty));
        return newVec;
    }
    public static Vector2 GetRandomDir()
    {
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1.0f)).normalized;
    }

    public static Vector2Int GetNeighborPos(this Vector2Int v, GridNeighborPos pos)
    {
        switch (pos)
        {
            case GridNeighborPos.Bottom:
                return v + Vector2Int.down;
            case GridNeighborPos.Top:
                return v + Vector2Int.up;
            case GridNeighborPos.Left:
                return v + Vector2Int.left;
            case GridNeighborPos.Right:
                return v + Vector2Int.right;

            case GridNeighborPos.TopLeft:
                return v + Vector2Int.left + Vector2Int.up;
            case GridNeighborPos.TopRight:
                return v + Vector2Int.right + Vector2Int.up;
            case GridNeighborPos.BottomLeft:
                return v + Vector2Int.left + Vector2Int.down;
            case GridNeighborPos.BottomRight:
                return v + Vector2Int.right + Vector2Int.down;
            default:
                return v;
        }
    }

    public static Vector2Int GetNeighborPos(this Vector2Int v, Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
                return v + Vector2Int.left;
            case Direction.Right:
                return v + Vector2Int.right;
            case Direction.Up:
                return v + Vector2Int.up;
            case Direction.Down:
                return v + Vector2Int.down;
            default:
                return v + Vector2Int.right;
        }
        
    }
}