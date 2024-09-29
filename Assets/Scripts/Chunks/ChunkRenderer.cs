using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

namespace Chunks
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class ChunkRenderer : MonoBehaviour
    {
        

        

        [SerializeField] private MeshFilter _meshFilter;

        public int[,,] Blocks = new int[Constants.Chunk.CHUNK_WIDTH, Constants.Chunk.CHUNK_HEIGHT, Constants.Chunk.CHUNK_WIDTH];

        private readonly List<Vector3> verticies = new();
        private readonly List<int> triangles = new();

        private void Start()
        {
            var mesh = new Mesh();

            Blocks = TerrainGenerator.GenerateTerrain();

            for (var y = 0; y < Constants.Chunk.CHUNK_HEIGHT; y++)
            {
                for (var x = 0; x < Constants.Chunk.CHUNK_WIDTH; x++)
                {
                    for (var z = 0; z < Constants.Chunk.CHUNK_WIDTH; z++)
                    {
                        var blockPosition = new Vector3Int(x, y, z);
                        GenerateBlock(blockPosition);
                    }
                }
            }

            mesh.vertices = verticies.ToArray();
            mesh.triangles = triangles.ToArray();

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            _meshFilter.mesh = mesh;
        }

        private void GenerateBlock(Vector3Int blockPosition)
        {
            if (GetBlockAtPosition(blockPosition) == 0) return;

            var up = blockPosition + Vector3Int.up;
            var down = blockPosition + Vector3Int.down;
            var left = blockPosition + Vector3Int.left;
            var right = blockPosition + Vector3Int.right;
            var forward = blockPosition + Vector3Int.forward;
            var back = blockPosition + Vector3Int.back;

            if (GetBlockAtPosition(up) == 0) GenerateTopSide(blockPosition);
            if (GetBlockAtPosition(down) == 0) GenerateBottomSide(blockPosition);
            if (GetBlockAtPosition(left) == 0) GenerateLeftSide(blockPosition);
            if (GetBlockAtPosition(right) == 0) GenerateRightSide(blockPosition);
            if (GetBlockAtPosition(forward) == 0) GenerateFrontSide(blockPosition);
            if (GetBlockAtPosition(back) == 0) GenerateBackSide(blockPosition);
        }

        private int GetBlockAtPosition(Vector3Int blockPosition)
        {
            if (blockPosition.x is >= 0 and < Constants.Chunk.CHUNK_WIDTH &&
                blockPosition.y is >= 0 and < Constants.Chunk.CHUNK_HEIGHT &&
                blockPosition.z is >= 0 and < Constants.Chunk.CHUNK_WIDTH)
            {
                return Blocks[blockPosition.x, blockPosition.y, blockPosition.z];
            }

            return 0;
        }

        private void GenerateTopSide(Vector3Int blockPosition)
        {
            foreach (var topVertex in Constants.Chunk.TopVerticies)
            {
                verticies.Add(topVertex + blockPosition);
            }

            AddLastVerticiesSquare();
        }

        private void GenerateBottomSide(Vector3Int blockPosition)
        {
            foreach (var bottomVertex in Constants.Chunk.BottomVerticies)
            {
                verticies.Add(bottomVertex + blockPosition);
            }

            AddLastVerticiesSquare();
        }

        private void GenerateLeftSide(Vector3Int blockPosition)
        {
            foreach (var leftVertex in Constants.Chunk.LeftVerticies)
            {
                verticies.Add(leftVertex + blockPosition);
            }

            AddLastVerticiesSquare();
        }

        private void GenerateRightSide(Vector3Int blockPosition)
        {
            foreach (var rightVertex in Constants.Chunk.RightVerticies)
            {
                verticies.Add(rightVertex + blockPosition);
            }

            AddLastVerticiesSquare();
        }

        private void GenerateFrontSide(Vector3Int blockPosition)
        {
            foreach (var frontVertex in Constants.Chunk.FrontVerticies)
            {
                verticies.Add(frontVertex + blockPosition);
            }

            AddLastVerticiesSquare();
        }

        private void GenerateBackSide(Vector3Int blockPosition)
        {
            foreach (var backVertex in Constants.Chunk.BackVerticies)
            {
                verticies.Add(backVertex + blockPosition);
            }

            AddLastVerticiesSquare();
        }

        private void AddLastVerticiesSquare()
        {
            triangles.Add(verticies.Count - 4);
            triangles.Add(verticies.Count - 3);
            triangles.Add(verticies.Count - 2);

            triangles.Add(verticies.Count - 3);
            triangles.Add(verticies.Count - 1);
            triangles.Add(verticies.Count - 2);
        }
    }
}