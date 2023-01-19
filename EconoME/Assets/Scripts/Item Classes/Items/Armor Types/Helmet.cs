using System;

[Serializable]
public class Helmet : Armor
{
    public int Intelligence;
    public Helmet(HelmetSriptableObject itemType) : base(itemType)
    {
    }

    public Helmet(Helmet helmet) : base(helmet)
    {
        Intelligence = helmet.Intelligence;
    }

    public override Item Duplicate()
    {
        return new Helmet(this);
    }

}

