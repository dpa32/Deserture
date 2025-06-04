using UnityEngine;
using System.Collections.Generic;

public class JPSVisualizer : MonoBehaviour
{
    public float NodeSize = 3f;
    public Color WalkableColor = Color.white;
    public Color UnwalkableColor = Color.red;
    public Color PathColor = Color.green;
    public Color ListColor = Color.blue;

    private List<Node> _pathNodes;
    private IEnumerable<GridChunk> _chunks;
    //public PriorityQueue<Node> _openList;
    //public HashSet<Node> _closedList;

    public void SetChunk(IEnumerable<GridChunk> chunks)
    {
        _chunks = chunks;
    }

    public void SetPath(List<Node> path)
    {
        _pathNodes = path;
    }

    private void OnDrawGizmos()
    {

        if (_pathNodes != null)
        {
            Gizmos.color = PathColor;
            foreach (var node in _pathNodes)
            {
                Gizmos.DrawSphere(node.WorldPos, NodeSize * 1.2f);
            }
        }

        if (_chunks != null)
        {
            foreach (var chunk in _chunks)
            {
                foreach (var node in chunk.GetAllNodes())
                {
                    Gizmos.color = node.IsWalkable ? WalkableColor : UnwalkableColor;
                    Gizmos.DrawWireCube(node.WorldPos, Vector3.one * NodeSize);
                }

            }
        }

    }
}
