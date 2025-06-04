using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionNode : INode
{
    /*
    private Func<INode.STATE> _onUpdate;
    public ActionNode(Func<INode.STATE> onUpdate)
    {
        _onUpdate = onUpdate;
    }
    public INode.STATE Evaluate() => _onUpdate?.Invoke() ?? INode.STATE.FAILURE;*/
    protected Blackboard _blackboard;

    public ActionNode(Blackboard blackboard)
    {
        _blackboard = blackboard;
    }

    public abstract INode.STATE Evaluate();
}