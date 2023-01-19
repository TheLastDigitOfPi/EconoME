using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[Serializable]
public class Tool : Item
{
    [SerializeField] private int _toolTier;
    [SerializeField] private int _damage;
    [SerializeField] TextureGroup[] _toolSwingAnimations = new TextureGroup[4];

    public TextureGroup[] ToolSwingAnimations { get { return _toolSwingAnimations; } }

    public int ToolTier => _toolTier;
    public int Damage => _damage;

    public Tool(ToolSriptableObject itemType) : base(itemType)
    {
        _toolTier = itemType.ToolTier;
        _damage = itemType.Damage;
        Stacksize = 1;
        _toolSwingAnimations = itemType.ToolSwingAnimations;
    }

    public Tool(Tool tool) : base(tool)
    {
        _toolTier = tool.ToolTier;
        _damage = tool.Damage;
        _toolSwingAnimations = tool.ToolSwingAnimations;
    }

    public override Item Duplicate()
    {
        return new Tool(this);
    }

    
}



