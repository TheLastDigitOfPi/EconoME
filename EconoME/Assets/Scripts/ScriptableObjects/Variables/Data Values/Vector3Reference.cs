using System;
using UnityEngine;

[Serializable]
public class Vector3Reference
{
    public Vector3Variable Variable;
    public Vector3 ConstantValue;
    public bool UseConstant;

    public Vector3 Value {get{return UseConstant? ConstantValue: Variable.Value;}}
}