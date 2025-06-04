using System;
using UnityEngine;

public class Node : IComparable<Node>
{
    public Vector3Int GridPos;
    public Vector3 WorldPos;
    public bool IsWalkable;
    public float GCost;
    public float HCost;
    public float FCost => GCost + HCost;
    public Node Parent;

    public Node(Vector3Int gridPos, Vector3 worldPos, bool isWalkable)
    {
        GridPos = gridPos;
        WorldPos = worldPos;
        IsWalkable = isWalkable;
        GCost = float.MaxValue;
        HCost = 0;
    }

    public override bool Equals(object obj)
    {
        return obj is Node node && GridPos.Equals(node.GridPos);
    }

    public override int GetHashCode()
    {
        return GridPos.GetHashCode();
    }

    public int CompareTo(Node other)
    {
        int fCom = FCost.CompareTo(other.FCost);
        return FCost != 0 ? fCom : HCost.CompareTo(other.HCost);
    }
}
