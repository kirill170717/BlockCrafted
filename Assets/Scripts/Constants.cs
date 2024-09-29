using UnityEngine;

namespace DefaultNamespace
{
    public static class Constants
    {
        public class Chunk
        {
            public const int CHUNK_WIDTH = 16;
            public const int CHUNK_HEIGHT = 256;

            public static readonly Vector3Int[] TopVerticies =
            {
                new(0, 1, 0),
                new(0, 1, 1),
                new(1, 1, 0),
                new(1, 1, 1)
            };

            public static readonly Vector3Int[] BottomVerticies =
            {
                new(0, 0, 0),
                new(1, 0, 0),
                new(0, 0, 1),
                new(1, 0, 1)
            };

            public static readonly Vector3Int[] LeftVerticies =
            {
                new(0, 0, 0),
                new(0, 0, 1),
                new(0, 1, 0),
                new(0, 1, 1)
            };

            public static readonly Vector3Int[] RightVerticies =
            {
                new(1, 0, 0),
                new(1, 1, 0),
                new(1, 0, 1),
                new(1, 1, 1)
            };

            public static readonly Vector3Int[] FrontVerticies =
            {
                new(0, 0, 1),
                new(1, 0, 1),
                new(0, 1, 1),
                new(1, 1, 1)
            };

            public static readonly Vector3Int[] BackVerticies =
            {
                new(0, 0, 0),
                new(0, 1, 0),
                new(1, 0, 0),
                new(1, 1, 0)
            };
        }
    }
}