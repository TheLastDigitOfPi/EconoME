using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class InteractionSO : ScriptableObject
{
    public Guid ID = Guid.NewGuid();
    [SerializeField] string _id;
    public abstract Interaction GetInteraction();
    public bool IsEqualTo(InteractionSO other)
    {
        return ID.Equals(other.ID);
    }

    public bool IsEqualTo(Interaction other)
    {
        return ID.Equals(other.ID);
    }

    public InteractionSO()
    {
        _id = ID.ToString();
    }

    public void resetGUID()
    {
        ID = Guid.NewGuid();
        _id = ID.ToString();
    }
}

public abstract class Interaction
{
    public abstract void Activate(InteractionHandler handler);
    public Guid ID = Guid.NewGuid();

    public abstract event Action OnInteractionEnd;
    [SerializeField] string _id;
    public bool IsEqualTo(InteractionSO other)
    {
        return ID.Equals(other.ID);
    }

    public Interaction(Guid interactionId)
    {
        ID = interactionId;
        _id = interactionId.ToString();
    }
}