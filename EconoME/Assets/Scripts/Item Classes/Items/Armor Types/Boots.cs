using System;

[Serializable]
public class Boots : Armor
{
    public Boots(BootsSriptableObject itemType) : base(itemType)
    {
    }

    public Boots(Boots armor) : base(armor)
    {
    }
    public override Item Duplicate()
    {
        return new Boots(this);
    }
}

