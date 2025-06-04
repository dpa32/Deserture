using System.Collections.Generic;

public abstract class CompositeNode : INode
{
    protected List<INode> _children = new List<INode>();
    protected int _currentChild = 0;

    public abstract INode.STATE Evaluate();

    public virtual void Reset()
    {
        _currentChild = 0;
        foreach (var child in _children)
        {
            if (child is CompositeNode composite)
            {
                composite.Reset();
            }
        }
    }

    public void Add(INode child)
    {
        _children.Add(child);
    }
}
