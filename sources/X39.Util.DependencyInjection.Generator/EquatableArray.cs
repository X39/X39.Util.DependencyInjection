using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace X39.Util.DependencyInjection.Generator;

/// <summary>
/// A thin wrapper around <see cref="ImmutableArray{T}"/> that provides structural equality.
/// Needed because <see cref="ImmutableArray{T}"/> uses reference equality by default,
/// which breaks incremental generator caching.
/// </summary>
internal readonly struct EquatableArray<T> : IEquatable<EquatableArray<T>>, IEnumerable<T>
    where T : IEquatable<T>
{
    private readonly ImmutableArray<T> _array;

    public EquatableArray(ImmutableArray<T> array)
    {
        _array = array;
    }

    public EquatableArray(IEnumerable<T> items)
    {
        _array = items.ToImmutableArray();
    }

    public ImmutableArray<T> AsImmutableArray() => _array.IsDefault ? ImmutableArray<T>.Empty : _array;

    public int Length => _array.IsDefault ? 0 : _array.Length;

    public T this[int index] => AsImmutableArray()[index];

    public bool Equals(EquatableArray<T> other)
    {
        var self = AsImmutableArray();
        var otherArray = other.AsImmutableArray();

        if (self.Length != otherArray.Length)
            return false;

        for (var i = 0; i < self.Length; i++)
        {
            if (!self[i].Equals(otherArray[i]))
                return false;
        }

        return true;
    }

    public override bool Equals(object? obj) => obj is EquatableArray<T> other && Equals(other);

    public override int GetHashCode()
    {
        var array = AsImmutableArray();
        var hash = 0;
        foreach (var item in array)
        {
            hash = unchecked(hash * 31 + item.GetHashCode());
        }
        return hash;
    }

    public static bool operator ==(EquatableArray<T> left, EquatableArray<T> right) => left.Equals(right);
    public static bool operator !=(EquatableArray<T> left, EquatableArray<T> right) => !left.Equals(right);

    public ImmutableArray<T>.Enumerator GetEnumerator() => AsImmutableArray().GetEnumerator();
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => ((IEnumerable<T>)AsImmutableArray()).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)AsImmutableArray()).GetEnumerator();
}
