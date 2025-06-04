using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EnemyBTRunner : MonoBehaviour 
{
    public Blackboard Blackboard = new();

    [SerializeField]
    private GameObject _target;
    [SerializeField]
    private List<GameObject> _points;

    private SelectorNode _rootNode = new();

    private SequenceNode _attackSequence = new();
    private SequenceNode _chaseSequence = new();
    private SequenceNode _patrolSequence = new();

    private INode.STATE _lastState;

    public void Start()
    {
        InitBB();
        InitBT();
    }

    public void Update()
    {
        var state = _rootNode.Evaluate();

        if (_lastState == INode.STATE.RUNNING && state != INode.STATE.RUNNING)
        {
            _rootNode.Reset(); 
        }

        _lastState = state;
    }

    private void InitBB()
    {
        Blackboard.Set(BBKeys.This, transform);
        Blackboard.Set(BBKeys.Target, _target.transform);

        Blackboard.Set(BBKeys.JPSRunner, 
            new JPSRunner(GetComponent<Rigidbody>(),
                            WorldManager.Instance.Grid,
                            WorldManager.Instance.PathFinder));

        Blackboard.Set(BBKeys.PatrolPoints, new List<Transform>());
        foreach (var point in _points)
        {
            Blackboard.Get(BBKeys.PatrolPoints).Add(point.transform);
        }

        Blackboard.Set(BBKeys.Speed, 3.5f);
        Blackboard.Set(BBKeys.ViewAngle, 120f);

        Blackboard.Set(BBKeys.ChaseRange, 40f);
        Blackboard.Set(BBKeys.AttackRange, 1.5f);
    }

    private void InitBT()
    {
        _attackSequence.Add(new IsInAttackRange(Blackboard));
        _attackSequence.Add(new Attack(Blackboard));
        _attackSequence.Add(new WaitAttack(Blackboard));

        _chaseSequence.Add(new IsInChaseRange(Blackboard));
        _chaseSequence.Add(new Chase(Blackboard));

        _patrolSequence.Add(new IsInPatrolRange(Blackboard));
        _patrolSequence.Add(new Patrol(Blackboard));
        _patrolSequence.Add(new WaitPatrol(Blackboard));


        _rootNode.Add(_attackSequence);
        _rootNode.Add(_chaseSequence);
        _rootNode.Add(_patrolSequence);
        _rootNode.Add(new Return(Blackboard));
    }
}
