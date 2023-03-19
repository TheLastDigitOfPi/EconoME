public static class ConditionalExtensions
{
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

    public static bool isBetweenMinInclusive(this float num, float min, float max)
    {
        return (num >= min && num < max);
    }

    public static bool isBetweenInclusive(this float num, float min, float max)
    {
        return (num >= min && num <= max);
    }

    public static bool isBetweenInclusive(this int num, int min, int max)
    {
        return (num >= min && num <= max);
    }


}
