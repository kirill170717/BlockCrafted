using System;
using UnityEngine;
using Blocks.Enums;

public class TerrainGenerator : MonoBehaviour
{
    [Serializable]
    public class NoiseOctaveSettings
    {
        public FastNoiseLite.NoiseType NoiseType;
        public float Frequency = 0.2f;
        public float Amplitude = 1;
    }

    public int Seed;
    public float BaseHeight = 8f;
    public NoiseOctaveSettings[] Octaves;
    public NoiseOctaveSettings DomainWarp;

    private FastNoiseLite[] _octavesNoises;
    private FastNoiseLite _warpNoise;

    private void Awake()
    {
        InitializationNoises();
    }
    
    public void InitializationNoises()
    {
        _octavesNoises = new FastNoiseLite[Octaves.Length];

        for (var i = 0; i < Octaves.Length; i++)
        {
            _octavesNoises[i] = new FastNoiseLite(Seed);
            _octavesNoises[i].SetNoiseType(Octaves[i].NoiseType);
            _octavesNoises[i].SetFrequency(Octaves[i].Frequency);
        }

        _warpNoise = new FastNoiseLite(Seed);
        _warpNoise.SetNoiseType(DomainWarp.NoiseType);
        _warpNoise.SetFrequency(DomainWarp.Frequency);
        _warpNoise.SetDomainWarpAmp(DomainWarp.Amplitude);
    }

    public BlockType[,,] GenerateTerrain(float xOffset, float zOffset)
    {
        var result = new BlockType
        [
            Constants.Chunk.CHUNK_WIDTH,
            Constants.Chunk.CHUNK_HEIGHT,
            Constants.Chunk.CHUNK_WIDTH
        ];

        for (var x = 0; x < Constants.Chunk.CHUNK_WIDTH; x++)
        {
            for (var z = 0; z < Constants.Chunk.CHUNK_WIDTH; z++)
            {
                var height = GetHeight(x * Constants.Block.BLOCK_SCALE + xOffset,
                    z * Constants.Block.BLOCK_SCALE + zOffset);
                var calculatedHeight = height / Constants.Block.BLOCK_SCALE;
                
                for (var y = 0; y < calculatedHeight; y++)
                {
                    result[x, y, z] = BlockType.Grass;
                }
            }
        }

        return result;
    }

    private float GetHeight(float x, float y)
    {
        _warpNoise.DomainWarp(ref x, ref y);
        var result = BaseHeight;

        for (var i = 0; i < Octaves.Length; i++)
        {
            var noise = _octavesNoises[i].GetNoise(x, y);
            result += noise * Octaves[i].Amplitude / 2;
        }

        return result;
    }
}