using System.Collections.Generic;
using UnityEngine;

public static class BBKeys 
{
    public static readonly BBKey<Transform> This = new("This");
    public static readonly BBKey<Transform> Target = new("Target");

    public static readonly BBKey<JPSRunner> JPSRunner = new("Runner");

    public static readonly BBKey<List<Transform>> PatrolPoints = new("PatrolPoints");

    public static readonly BBKey<int> Health = new("Health");
    public static readonly BBKey<float> Speed = new("Speed");
    public static readonly BBKey<float> ViewAngle = new("ViewAngle");

    public static readonly BBKey<float> ChaseRange = new("ChaseRange");
    public static readonly BBKey<float> AttackRange = new("AttackRange");
}