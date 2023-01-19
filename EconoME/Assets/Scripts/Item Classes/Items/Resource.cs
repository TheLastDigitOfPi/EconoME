using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class Resource:Item
{
    public int Tier;

    public Resource(ResourceSriptableObject itemType) : base(itemType)
    {
        this.Tier = itemType.Tier;
    }

    public Resource(Resource other) : base(other)
    {
        Tier = other.Tier;
    }

    public override Item Duplicate()
    {
        return new Resource(this);
    }


}

