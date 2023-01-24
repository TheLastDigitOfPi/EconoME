using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Armor : Item
{
    public int defense;

    public Armor(Item other) : base(other)
    {
    }

    public Armor(ArmorSriptableObject itemBase) : base(itemBase)
    {
    }
}

