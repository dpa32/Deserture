using UnityEngine;

public class SelectorNode : CompositeNode
{
    public override INode.STATE Evaluate()
    {
        if (_children == null || _children.Count == 0)
        {
            return INode.STATE.FAILURE;
        }
        while (_currentChild < _children.Count)
        {

            var state = _children[_currentChild].Evaluate();

            if (state == INode.STATE.RUNNING)
            {
                return INode.STATE.RUNNING;
            }
            if (state == INode.STATE.SUCCESS)
            {
                _currentChild = 0;
                return INode.STATE.SUCCESS;
            }
            _currentChild++; // FAILUREÀÏ °æ¿ì
        }
        _currentChild = 0;
        return INode.STATE.FAILURE;
    }
}
