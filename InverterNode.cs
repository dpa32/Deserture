public class InverterNode : DecoratorNode
{
    public override INode.STATE Evaluate()
    {
        if (_child == null)
        {
            return INode.STATE.FAILURE;
        }

        var state = _child.Evaluate();
        if (state == INode.STATE.SUCCESS)
        {
            return INode.STATE.FAILURE;
        }
        if (state == INode.STATE.FAILURE)
        {
            return INode.STATE.SUCCESS;
        }
        return INode.STATE.RUNNING;
    }
}