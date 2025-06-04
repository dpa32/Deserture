
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PathFinder
{
    private Grid3D _grid;
    private Node _startNode;
    private Node _endNode;
    private JumpPointCache _jumpCache;

    //private Dictionary<Node, Vector3Int, Node> ;

    private PriorityQueue<Node> _openList = new();
    private HashSet<Node> _closedList = new();

    private int _maxDepth = 500;
    private int _currentDepth = 0;

    public PriorityQueue<Node> OpenList => _openList;
    public HashSet<Node> ClosedList => _closedList;

    public PathFinder(JumpPointCache jumpCache)
    {
        _jumpCache = jumpCache;
    }

    public List<Node> FindPath(Grid3D grid, Vector3 startWorld, Vector3 endWorld)
    {
        _grid = grid;
        _startNode = _grid.GetNode(startWorld);
        _endNode = _grid.GetNode(endWorld);
        //_startY = _startNode.GridPos.y;

        if (_startNode == null || _endNode == null || !_startNode.IsWalkable || !_endNode.IsWalkable)
        {
            return null;
        }
        /*
        if(_startNode == _endNode)
        {
            return 
        }*/

        _openList = new PriorityQueue<Node>();
        _closedList = new HashSet<Node>();

        _currentDepth = 0;
        Debug.Log($"DEP reset suc {startWorld}  {endWorld}");

        _startNode.GCost = 0;
        _startNode.HCost = Heuristic(_startNode, _endNode);
        _startNode.Parent = null;

        _openList.Enqueue(_startNode);

        while (_openList.Count > 0)
        {
            var current = _openList.Dequeue();

            if (current == _endNode)
            {
                return Reconstruct(current);
            }

            _closedList.Add(current);

            foreach (var dir in GetNextDirs(current))
            {

                Node jumpNode = Jump(current, dir);
                if (jumpNode == null || _closedList.Contains(jumpNode))
                {
                    continue;
                }

                float tentativeG = current.GCost + Heuristic(current, jumpNode);

                if (tentativeG < jumpNode.GCost || !_openList.Contain(jumpNode))
                {
                    jumpNode.GCost = tentativeG;
                    jumpNode.HCost = Heuristic(jumpNode, _endNode);
                    jumpNode.Parent = current;

                    if (!_openList.Contain(jumpNode))
                    {
                        _openList.Enqueue(jumpNode);
                    }
                }
            }
        }

        return null;
    }

    private Node Jump(Node current, Vector3Int dir)
    {
        _currentDepth++;

        if (current == null || !current.IsWalkable || _currentDepth > _maxDepth)
        {
            Debug.Log($"here 1{current == null}   2{!current.IsWalkable}   3{_currentDepth > _maxDepth}");
            return null;
        }

        Vector3 nextWorld = current.WorldPos + new Vector3(dir.x, dir.y, dir.z);

        if (!_grid.GetChunk(nextWorld).GetNode(nextWorld)?.IsWalkable ?? true)
        {
            return null;
        }

        if (_jumpCache.TryGet(current, dir, out Vector3? cached))
        {
            return _grid.GetNode(cached.Value);
        }

        Node nextNode = _grid.GetNode(nextWorld);

        if (nextNode == null || nextNode == current || !nextNode.IsWalkable)
        {
            return null;
        }
        if (nextNode == _endNode )
        {
            return nextNode;
        }

        if (HasForcedNeighbor(nextNode.GridPos, dir))
        {
            _jumpCache.Add(current, dir, nextNode.WorldPos);
            return nextNode;
        }

        if (IsDiagonal(dir))
        {
            foreach (var axis in GetAxes(dir))
            {
                if (Jump(nextNode, axis) != null)
                {
                    //_jumpCache.Add(current, dir, nextNode.WorldPos);
                    return nextNode;
                }
            }
        }
        else
        {
            foreach (var pDir in GetPerpendiculars(dir))
            {
                if (Jump(nextNode, pDir) != null)
                {
                    return nextNode;
                }
            }
        }

        Node result = Jump(nextNode, dir);
        if (result != null)
        {
            _jumpCache.Add(current, dir, nextNode.WorldPos);
        }

        return result;
    }

    private List<Node> Reconstruct(Node end)
    {
        var path = new List<Node>();
        Node cur = end;
        while (cur != null)
        {
            path.Add(cur);
            cur = cur.Parent;
        }
        path.Reverse();
        return path;
    }

    private float Heuristic(Node a, Node b)
    {
        return Vector3.Distance(a.WorldPos, b.WorldPos);
    }

    private List<Vector3Int> GetNextDirs(Node node)
    {
        var dirs = new List<Vector3Int>();
        if (node.Parent == null)
        {
            dirs.AddRange(Directions3D.AllDirections);
            return dirs;
        }

        Vector3Int dir;

        if (_grid.GetChunk(node.WorldPos) == _grid.GetChunk(node.Parent.WorldPos))
        {

            dir = node.GridPos - node.Parent.GridPos;
        }
        else
        {
            dir = Vector3Int.FloorToInt(node.WorldPos - node.Parent.WorldPos);
        }

        dir = new Vector3Int(Mathf.Clamp(dir.x, -1, 1), Mathf.Clamp(dir.y, -1, 1), Mathf.Clamp(dir.z, -1, 1));
        dirs.Add(dir);

        if (IsDiagonal(dir))
        {
            foreach (var axis in GetAxes(dir))
            {
                if ((_grid.GetNode(node.WorldPos + (Vector3)axis)?.IsWalkable ?? false))
                {
                    dirs.Add(axis);
                }
            }
        }
        else
        {
            foreach (var pDir in GetPerpendiculars(dir))
            {
                Node sideNode = _grid.GetNode(node.WorldPos + (Vector3)pDir);
                if (!(sideNode?.IsWalkable ?? false)) 
                {
                    Vector3Int f = dir + pDir;
                    Node forwardNode = _grid.GetNode(node.WorldPos + (Vector3)f);
                    if (forwardNode?.IsWalkable ?? false)
                    {
                        dirs.Add(f);
                    }
                }

            }
        }
        return dirs;
    }

    private bool HasForcedNeighbor(Vector3Int pos, Vector3Int dir)
    {
        foreach (var pDir in GetPerpendiculars(dir))
        {
            var nextPos = pos + pDir; // 수직방향 이동 노드
            var diagonPos = pos + pDir + dir; // 대각선 방향 이동 노드
            if (_grid.GetNode(nextPos)?.IsWalkable ?? false)
            {
                if (!_grid.GetNode(diagonPos)?.IsWalkable ?? true)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsDiagonal(Vector3Int dir) => 
        Mathf.Abs(dir.x) + Mathf.Abs(dir.y) + Mathf.Abs(dir.z) > 1; // 축이 두 개 이상인지 확인

    private List<Vector3Int> GetAxes(Vector3Int dir) => new() {
        new Vector3Int(dir.x, 0, 0), new Vector3Int(0, dir.y, 0), new Vector3Int(0, 0, dir.z)
    };

    private List<Vector3Int> GetPerpendiculars(Vector3Int dir)
    {
        var list = new List<Vector3Int>();
        if (dir.x != 0)
        {
            list.AddRange(new[] { Vector3Int.up, Vector3Int.down, Vector3Int.forward, Vector3Int.back });
        }
        if (dir.y != 0)
        {
            list.AddRange(new[] { Vector3Int.left, Vector3Int.right, Vector3Int.forward, Vector3Int.back });
        }
        if (dir.z != 0)
        {
            list.AddRange(new[] { Vector3Int.left, Vector3Int.right, Vector3Int.up, Vector3Int.down });
        }
        return list;
    }
}
