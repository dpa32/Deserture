using System;
using System.Collections.Generic;
using UnityEngine;

public class Grid3D
{
    public int ViewRange = 2;
    private Dictionary<Vector3Int, GridChunk> _chunks = new();
    private Dictionary<Vector3Int, GridChunk> _vairableChunks; // 이거 써야되 
    private Vector3Int _chunkSize;

    private Func<Vector3, bool> _isWalkablePredicate;

    public Grid3D(Vector3Int chunkSize, Func<Vector3, bool> isWalkablePredicate = null)
    {
        _chunkSize = chunkSize;
        _isWalkablePredicate = isWalkablePredicate ?? (_ => true);
    }

    public GridChunk GetChunk(Vector3 worldPos) // vector가 속한 chunk 
    {
        Vector3Int chunkPos = GetChunkIndex(worldPos);

        if (!_chunks.TryGetValue(chunkPos, out var chunk)) // chunk가 없다면 생성
        {
            chunk = new GridChunk(chunkPos, _chunkSize);
            chunk.Initialize(_isWalkablePredicate);
            _chunks.Add(chunkPos, chunk);
        }

        return chunk;
    }

    public Node GetNode(Vector3 worldPos)
    {
        var chunk = GetChunk(worldPos);
        return chunk?.GetNode(worldPos);
    }

    private Vector3Int GetChunkIndex(Vector3 worldPos) // worldPos가 속한 청크의 인덱스
    {
        return new Vector3Int(
            Mathf.FloorToInt(worldPos.x / _chunkSize.x),
            Mathf.FloorToInt(worldPos.y / _chunkSize.y),
            Mathf.FloorToInt(worldPos.z / _chunkSize.z)
        );
    }
    public void UpdateChunks(Vector3 centerPos)
    {
        var center = GetChunkIndex(centerPos);
        var newChunks = new Dictionary<Vector3Int, GridChunk>();

        for (int x = -ViewRange; x <= ViewRange; x++) // ViewRange = 2f
        {
            for (int y = -ViewRange; y <= ViewRange; y++)
            {
                for (int z = -ViewRange; z <= ViewRange; z++)
                {
                    Vector3Int pos = center + new Vector3Int(x, y, z);

                    if (!_chunks.TryGetValue(pos, out var chunk))
                    {
                        chunk = new GridChunk(pos, _chunkSize);
                        chunk.Initialize(_isWalkablePredicate);
                    }

                    newChunks[pos] = chunk;
                }
            }
        }
        _vairableChunks = newChunks; 
    }


    public Vector3Int GetLocalPos(Vector3 worldPos) // worldPos->chunkPos
    {
        Vector3Int chunkIndex = GetChunkIndex(worldPos);
        Vector3 worldOrigin = new Vector3(
            chunkIndex.x * _chunkSize.x,
            chunkIndex.y * _chunkSize.y,
            chunkIndex.z * _chunkSize.z
        );

        Vector3 local = worldPos - worldOrigin;
        return new Vector3Int(
            Mathf.FloorToInt(local.x),
            Mathf.FloorToInt(local.y),
            Mathf.FloorToInt(local.z)
        );
    }

    public IEnumerable<GridChunk> GetAllChunks() => _chunks.Values;
}