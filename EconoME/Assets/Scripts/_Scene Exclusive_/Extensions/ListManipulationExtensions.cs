using System.Collections.Generic;

public static class ListManipulationExtensions
{
    public static T First<T>(this T[] array)
    {
        return array.Length > 0 ? array[0] : default;
    }

    public static T Last<T>(this T[] array)
    {
        return array.Length > 0 ? array[^1] : default;
    }

    public static T RandomItem<T>(this T[] array, System.Random random = null)
    {
        if (array.Length < 1) { return default; }
        if (random == null)
            random = new System.Random();
        return array[random.Next(0, array.Length)];
    }

    public static T RandomListItem<T>(this List<T> list, System.Random random = null)
    {
        if (list.Count < 1) { return default; }
        if (random == null)
            random = new System.Random();
        //Rand is not inclusive (no need for count -1)
        return list[random.Next(0, list.Count)];
    }

}