using System;

[Serializable]
public class Leggings : Armor
{
    public Leggings(LeggingsSriptableObject itemType) : base(itemType)
    {
    }

    public Leggings(Leggings armor) : base(armor)
    {
    }

    public override Item Duplicate()
    {
        return new Leggings(this);
    }
}

