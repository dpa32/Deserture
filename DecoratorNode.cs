using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DecoratorNode : INode
{
    protected INode _child;
    public void SetChild(INode node)
    {
        _child = node;
    }
    public abstract INode.STATE Evaluate();
}
