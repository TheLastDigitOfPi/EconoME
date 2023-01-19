using System;
using UnityEngine;

[Serializable]
public class IntReference
{
    public IntVariable Variable;
    [SerializeField] int ConstantValue;
    public bool UseConstant;

    public int Value
    {
        get { return UseConstant ? ConstantValue : Variable.Value; }
        set
        {
            if(UseConstant){ConstantValue = value; }
            else{Variable.Value = value; }
        }
    }
}
