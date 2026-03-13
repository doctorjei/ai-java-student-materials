// Added: 2025 Jeremiah Blanchard (University of Florida)

namespace Uwu.GamePlaying;
using CompilerServices = System.Runtime.CompilerServices;
using MemoryMarshal = System.Runtime.InteropServices.MemoryMarshal;

public interface Indexable<T, I> { T this[I index] { get; set; } }
public interface Indexer { public static abstract int[] Dimensions { get; } }

// Index magic follows below. :)
public struct IndexKey<T> : Indexable<int, int> where T : Indexer
{
    // Key provider class; keys are linked to specific GameState variant. (T is linked type.)
    public class KeyProvider() { public IndexKey<T> this[params int[] idx] { get => new(idx); } }

    public static int[] Dimensions { get; } = T.Dimensions;
    readonly int[] indices;
    int hash;

    public override readonly bool Equals(object? obj) =>
        obj is null && (object) this is null || (obj != null 
            && obj.GetType() == typeof(IndexKey<T>) && obj.GetHashCode() == GetHashCode());

    public override readonly int GetHashCode() => hash;

    [CompilerServices.MethodImpl(CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode(int[] indices)
    {
        int[] hashData = new int[Dimensions.Length + indices.Length];
        Dimensions.CopyTo(hashData, 0);
        indices.CopyTo(hashData, Dimensions.Length);

        return (int)(long)System.IO.Hashing.XxHash3.HashToUInt64(
            MemoryMarshal.Cast<int, byte>(new System.ReadOnlySpan<int>(hashData)), 0);
    }

    public IndexKey(int[] _indices)
    {
        indices = new int[Dimensions.Length];

        if (_indices.Length != indices.Length)
            throw new System.IndexOutOfRangeException(string.Format(
                ErrorMessages.IdxCount, indices.Length, _indices.Length));

        for (int dim = 0; dim < indices.Length; dim++)
        {
            if (_indices[dim] >= Dimensions[dim])
                throw new System.IndexOutOfRangeException(string.Format(
                    ErrorMessages.DimIndex, dim, Dimensions[dim], _indices[dim]));

            if (_indices[dim] < 0)
                indices[dim] = _indices[dim] + Dimensions[dim];
            else
                indices[dim] = _indices[dim];
        }

        indices = _indices;
        hash = GetHashCode(indices);
    }

    // Limited dimension setter to simplify indexing (above); inline if possible.
    public int this[int idx]
    {
        readonly get { return indices[idx]; }
        set { indices[idx] = value; hash = GetHashCode(indices); }
    }

    public readonly struct ErrorMessages
    {
        public static readonly string IdxCount = "Number of indices should be {0} but is {1}!",
            DimIndex = "Index for dimension #{0} must be in range [0,{1}] (got {2}).";
    }

    public static bool operator ==(IndexKey<T> lhs, IndexKey<T> rhs) => lhs.Equals(rhs);
    public static bool operator !=(IndexKey<T> lhs, IndexKey<T> rhs) => !(lhs == rhs);
}