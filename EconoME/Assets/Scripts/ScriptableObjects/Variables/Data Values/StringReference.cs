using System;

[Serializable]
public class StringReference
{
    public StringVariable Variable;
    public string ConstantValue;
    public bool UseConstant;

    public string Value {get{return UseConstant? ConstantValue: Variable.Value;}}
}
