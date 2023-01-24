using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class Interaction : ScriptableObject
{
    public abstract void Activate(InteractionHandler handler);
    public Guid ID = Guid.NewGuid();

    public abstract event Action OnInteractionEnd;
    [SerializeField] string _id;
    public bool IsEqualTo(Interaction other)
    {
        return ID.Equals(other.ID);
    }

    public Interaction()
    {
        _id = ID.ToString();
    }

    public void resetGUID()
    {
        ID = Guid.NewGuid();
        _id = ID.ToString();
    }
}

