using System;
using System.IO;

namespace Jobs.Aggregator.Utils
{
    public interface IXXHash32
    {
        /// <summary>
        /// Gets the <see cref="uint"/> value of the computed hash code.
        /// </summary>
        /// <exception cref="InvalidOperationException">Hash computation has not yet completed.</exception>
        uint HashUInt32 { get; }

        /// <summary>
        /// Gets or sets the value of seed used by xxHash32 algorithm.
        /// </summary>
        /// <exception cref="InvalidOperationException">Hash computation has not yet completed.</exception>
        uint Seed { get; set; }

        bool CanReuseTransform { get; }
        bool CanTransformMultipleBlocks { get; }
        byte[] Hash { get; }
        int HashSize { get; }
        int InputBlockSize { get; }
        int OutputBlockSize { get; }

        /// <summary>
        /// Initializes this instance for new hash computing.
        /// </summary>
        void Initialize();

        void Clear();
        byte[] ComputeHash(byte[] buffer);
        byte[] ComputeHash(byte[] buffer, int offset, int count);
        byte[] ComputeHash(Stream inputStream);
        void Dispose();
        int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset);
        byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount);
        bool TryComputeHash(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesWritten);
    }
}