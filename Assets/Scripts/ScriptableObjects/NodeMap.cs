using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Node
{
    public Vector3 Position;
    public float MaxSpeed;
    public int[] NextNodes;
}
[CreateAssetMenu(fileName = "New Node Map", menuName = "Node Map")]

public class NodeMap : ScriptableObject
{
    public float ReachingRadius;
    public List<Node> Nodes;
}