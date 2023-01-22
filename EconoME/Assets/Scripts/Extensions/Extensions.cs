using System.Collections.Generic;
using UnityEngine;

/*
 * Extensions allow methods to be added to protected classes so common tasks
 * can be easily replicated such as Converting a Vector3 to a Vector 2
 */
public static class Extensions
{
    public static void ToggleCanvas(this CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = canvasGroup.alpha == 1 ? 0 : 1;
        canvasGroup.interactable = !canvasGroup.interactable;
        canvasGroup.blocksRaycasts = !canvasGroup.blocksRaycasts;
    }

    public static void ToggleCanvas(this CanvasGroup canvasGroup, bool ForceStatus)
    {
        canvasGroup.alpha = ForceStatus ? 1 : 0;
        canvasGroup.interactable = ForceStatus;
        canvasGroup.blocksRaycasts = ForceStatus;
    }
    public static bool isEmpty(this string daString)
    {
        return (daString == null || daString == "");
    }

    public static bool isBetween(this int num, int min, int max)
    {
        return (num > min && num < max);
    }

    public static bool isBetween(this float num, float min, float max)
    {
        return (num > min && num < max);
    }

    public static bool isBetweenInclusive(this int num, int min, int max)
    {
        return (num >= min && num <= max);
    }

    public static Vector3 ToVector3(this Vector2 vec2)
    {
        return new Vector3(vec2.x, vec2.y, 0);
    }

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

    public static Vector2 GetRandomDir()
    {
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1.0f)).normalized;
    }

    public static T First<T>(this T[] array)
    {
        try
        {
            return array[0];
        }
        catch (System.Exception)
        {
            return default;
        }
    }

    public static T Last<T>(this T[] array)
    {
        try
        {
            return array[array.Length - 1];
        }
        catch (System.Exception)
        {
            return default;
        }
    }

    public static T RandomItem<T>(this T[] array)
    {
        if (array.Length < 1) { return default; }
        System.Random rnd = new System.Random();
        return array[rnd.Next(0, array.Length)];
    }

    public static T RandomListItem<T>(this List<T> list)
    {
        if (list.Count < 1) { return default; }
        System.Random rnd = new System.Random();
        //Rand is not inclusive (no need for count -1)
        return list[rnd.Next(0, list.Count)];
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
}


