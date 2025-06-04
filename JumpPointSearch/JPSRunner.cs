using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class JPSRunner
{
    private readonly Rigidbody _rb;
    private readonly Grid3D _grid;
    private readonly PathFinder _pathFinder;

    private List<Node> _path;
    private int _index;

    private Transform _target;
    private Vector3 _lastTargetPosition;
    private float _repathDistance = 5.0f; // 재계산 기준

    public JPSRunner(Rigidbody rb, Grid3D grid, PathFinder finder)
    {
        _rb = rb;
        _grid = grid;
        _pathFinder = finder;
    }

    public void SetTarget(Transform target)
    {
        _target = target;
        _lastTargetPosition = target.position;
        Reset();
    }

    public INode.STATE Update(float speed)
    {
        if (_target == null)
        {
            return INode.STATE.FAILURE;
        }

        // 대상 위치가 변했는지 확인
        if (Vector3.Distance(_lastTargetPosition, _target.position) > _repathDistance)
        {
            Reset();
            _lastTargetPosition = _target.position;
        }

        if (_path == null || _path.Count == 0)
        {
            Node startNode = _grid.GetNode(_rb.position);
            Node endNode = _grid.GetNode(_target.position);

            if (startNode == null || endNode == null || !endNode.IsWalkable)
            {
                return INode.STATE.FAILURE;
            }

            _path = _pathFinder.FindPath(_grid, startNode.WorldPos, endNode.WorldPos);
            _index = 0;
            if (_path == null || _path.Count == 0)
            {
                return INode.STATE.FAILURE;
            }
            _path = SimplifyPath(_path);
        }

        if (_index >= _path.Count)
        {
            Reset();
            return INode.STATE.SUCCESS;
        }

        _grid.UpdateChunks(_rb.position);
        Node targetNode = _path[_index];
        Vector3 targetPos = targetNode.WorldPos;
        targetPos.y = _rb.position.y;

        Vector3 dir = targetPos - _rb.position;
        if (dir.magnitude > 0.5f)
        {
            dir.Normalize();
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            Quaternion newRotation = Quaternion.RotateTowards(_rb.rotation, targetRotation, 3f * Time.deltaTime);
            _rb.MoveRotation(newRotation);
            _rb.MovePosition(_rb.position + dir * speed * Time.fixedDeltaTime);

            return INode.STATE.RUNNING;
        }

        _index++;
        return INode.STATE.RUNNING;
    }

    public void Reset()
    {
        _path = null;
        _index = 0;
    }
    public List<Node> SimplifyPath(List<Node> fullPath)
    {
        if (fullPath == null || fullPath.Count <= 2)
        {
            return fullPath;
        }

        List<Node> simplified = new() { fullPath[0] };
        int index = 0;

        for (int i = 1; i < fullPath.Count; i++)
        {
            if (!HasLineOfSight(fullPath[index], fullPath[i]))
            {
                simplified.Add(fullPath[i - 1]);
                index = i - 1;
            }
        }

        simplified.Add(fullPath[^1]);
        return simplified;
    }

    private bool HasLineOfSight(Node a, Node b)
    {
        return !Physics.Raycast(a.WorldPos, (b.WorldPos - a.WorldPos).normalized,
            Vector3.Distance(a.WorldPos, b.WorldPos), AIUtility.ObstacleMask);
    }

    public Transform Target => _target;
    public List<Node> Path => _path;
}
