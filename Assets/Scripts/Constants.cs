using UnityEngine;

public static class Constants
{
    public class Block
    {
        public const float BLOCK_SCALE = 1f;
    }

    public class Chunk
    {
        public const int CHUNK_WIDTH = 16;
        public const int CHUNK_HEIGHT = 256;
        public const int CHUNK_WIDTH_SQ = CHUNK_WIDTH * CHUNK_WIDTH;

        public static readonly Vector3[] TopVerticies =
        {
            new(0, 1, 0),
            new(0, 1, 1),
            new(1, 1, 0),
            new(1, 1, 1)
        };

        public static readonly Vector3[] BottomVerticies =
        {
            new(0, 0, 0),
            new(1, 0, 0),
            new(0, 0, 1),
            new(1, 0, 1)
        };

        public static readonly Vector3[] LeftVerticies =
        {
            new(0, 0, 0),
            new(0, 0, 1),
            new(0, 1, 0),
            new(0, 1, 1)
        };

        public static readonly Vector3[] RightVerticies =
        {
            new(1, 0, 0),
            new(1, 1, 0),
            new(1, 0, 1),
            new(1, 1, 1)
        };

        public static readonly Vector3[] FrontVerticies =
        {
            new(0, 0, 1),
            new(1, 0, 1),
            new(0, 1, 1),
            new(1, 1, 1)
        };

        public static readonly Vector3[] BackVerticies =
        {
            new(0, 0, 0),
            new(0, 1, 0),
            new(1, 0, 0),
            new(1, 1, 0)
        };
    }

    public static class MouseLook
    {
        public const float MINIMUM_ROTATION_Y = -90;
        public const float MAXIMUM_ROTATION_Y = 90;
    }
}