using System;

[Serializable]
public class FloatReference
{
    public FloatVariable Variable;
    public float ConstantValue;
    public bool UseConstant;

    public float Value {get{return UseConstant? ConstantValue: Variable.Value;}}
}
