using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class Resource : Item
{

    public Resource(ResourceBase itemType) : base(itemType)
    {
    }

    public Resource(ResourceBase data, int stackSize) : base(data, stackSize)
    {

    }

    public Resource(Resource other) : base(other)
    {
        
    }

    public override Item Duplicate()
    {
        return new Resource(this);
    }


}

