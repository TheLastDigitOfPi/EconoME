using System;

[Serializable]
public class BoolReference
{
    public BoolVariable Variable;
    public bool ConstantValue;
    public bool UseConstant;

    public bool Value {get{return UseConstant? ConstantValue: Variable.Value;}}
}
