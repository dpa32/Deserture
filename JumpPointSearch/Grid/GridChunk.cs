using System;
using System.Collections.Generic;
using UnityEngine;

public class GridChunk
{
    public Vector3Int ChunkPos;
    public Vector3Int ChunkSize;
    public Vector3 WorldOrigin;
    private Node[,,] nodes;

    public GridChunk(Vector3Int chunkPos, Vector3Int chunkSize)
    {
        ChunkPos = chunkPos;
        ChunkSize = chunkSize;
        WorldOrigin = new Vector3(
            chunkPos.x * chunkSize.x,
            chunkPos.y * chunkSize.y,
            chunkPos.z * chunkSize.z
        );

        nodes = new Node[chunkSize.x, chunkSize.y, chunkSize.z];
    }

    public void Initialize(Func<Vector3, bool> isWalkablePredicate = null)
    {
        for (int x = 0; x < ChunkSize.x; x++)
        {
            for (int y = 0; y < ChunkSize.y; y++)
            {
                for (int z = 0; z < ChunkSize.z; z++)
                {
                    Vector3 worldPos = WorldOrigin + new Vector3(x + 0.5f, y + 0.5f, z + 0.5f);
                    bool isWalkable = isWalkablePredicate?.Invoke(worldPos) ?? true;

                    nodes[x, y, z] = new Node(new Vector3Int(x, y, z), worldPos, isWalkable);
                }
            }
        }
    }

    public Node GetNode(Vector3 worldPos)
    {
        Vector3 localPos = worldPos - WorldOrigin;
        int x = Mathf.FloorToInt(localPos.x);
        int y = Mathf.FloorToInt(localPos.y);
        int z = Mathf.FloorToInt(localPos.z);

        if (x >= 0 && y >= 0 && z >= 0 &&
            x < ChunkSize.x && y < ChunkSize.y && z < ChunkSize.z)
        {
            return nodes[x, y, z];
        }

        return null;
    }

   
    public Node GetNodeLocal(Vector3Int localPos)
    {
        if (localPos.x >= 0 && localPos.y >= 0 && localPos.z >= 0 &&
            localPos.x < ChunkSize.x && localPos.y < ChunkSize.y && localPos.z < ChunkSize.z)
        {
            return nodes[localPos.x, localPos.y, localPos.z];
        }

        return null;
    }

    public IEnumerable<Node> GetAllNodes()
    {
        foreach (var node in nodes)
        {
            yield return node;
        }
    }
}
