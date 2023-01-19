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
    public string _id;
    public bool IsEqualTo(object other)
    {
        if (other is not TextPopup) return false;
        TextPopup otherPopup = (TextPopup)other;
        return ID.Equals(otherPopup.ID);
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

