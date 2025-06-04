using UnityEngine;

public class SequenceNode : CompositeNode
{
    public override INode.STATE Evaluate()
    {
        if (_children == null || _children.Count == 0)
        {
            return INode.STATE.SUCCESS;
        }
        while (_currentChild < _children.Count)
        {
            var state = _children[_currentChild].Evaluate();

            if (state == INode.STATE.RUNNING)
            {
                return INode.STATE.RUNNING;
            }
            if (state == INode.STATE.FAILURE)
            {
                _currentChild = 0;
                Debug.Log($"{_children[_currentChild]} : FAIL");
                return INode.STATE.FAILURE;
            }
            Debug.Log($"{_children[_currentChild]} : SUC");
            _currentChild++; // SUCCESSÀÏ °æ¿ì
        }
        _currentChild = 0;
        return INode.STATE.SUCCESS;
    }
}
