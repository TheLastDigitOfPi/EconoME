using System;

[Serializable]
public class Chestplate : Armor
{
    public Chestplate(ChestplateSriptableObject itemBase) : base(itemBase)
    {
    }

    public Chestplate(Chestplate armor) : base(armor)
    {
    }
    public override Item Duplicate()
    {
        return new Chestplate(this);
    }
}

