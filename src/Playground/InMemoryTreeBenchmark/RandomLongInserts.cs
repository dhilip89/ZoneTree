﻿using Tenray.ZoneTree.Collections;
using Tenray.ZoneTree.Collections.BTree;
using Tenray.ZoneTree.Comparers;

namespace Playground.InMemoryTreeBenchmark;

public static class RandomLongInserts
{
    public static long[] GetRandomArray(long count)
    {
        var arr = GetSortedArray(count);
        RandomIntInserts.Shuffle(arr);
        return arr;
    }

    public static long[] GetSortedArray(long count)
    {
        var arr = new long[count];
        for (var i = 0; i < count; ++i)
            arr[i] = i;
        return arr;
    }

    public static void InsertBTree(long[] arr)
    {
        var count = arr.Length;
        var tree = new UnsafeBTree<long, long>(new Int64ComparerAscending());
        for(var i = 0; i < count; ++i)
        {
            var x = arr[i];
            tree.TryInsert(x, x + x);
        }
        for (var i = 0; i < count; ++i)
        {
            var x = arr[i];
            var exists = tree.TryGetValue(x, out var val);
            if (!exists || val != x + x)
                throw new Exception($"exists: {exists} ({x},{val}) != ({x},{x + x})");
        }

        var node = tree.First;
        var off = 0;
        Array.Sort(arr);
        while (node != null)
        {
            var keys = node.Keys;
            for (var i = 0; i < node.Length; ++i)
            {
                var a = arr[off++];
                var k = keys[i];
                if (a != k)
                    throw new Exception($"iteration failed. {a} != {k}");
            }
            node = node.Next;
        }
    }

    public static void InsertAndValidateIteratorBTree(long[] arr)
    {
        var count = arr.Length;
        var orj = arr.ToArray();
        var tree = new UnsafeBTree<long, long>(new Int64ComparerAscending());
        for (var i = 0; i < count; ++i)
        {
            var x = arr[i];
            tree.TryInsert(x, x + x);
        }

        for (var i = 0; i < count; ++i)
        {
            var x = arr[i];
            var exists = tree.TryGetValue(x, out var val);
            if (!exists || val != x + x)
                throw new Exception($"exists: {exists} ({x},{val}) != ({x},{x + x})");
        }

        var node = tree.First;
        var off = 0;
        Array.Sort(arr);
        while (node != null)
        {
            var keys = node.Keys;
            for (var i = 0; i < node.Length; ++i)
            {
                var a = arr[off++];
                var k = keys[i];
                if (a != k)
                    throw new Exception($"iteration failed. {a} != {k}");
            }
            node = node.Next;
        }

        off = 0;
        arr = arr.Reverse().ToArray();
        node = tree.Last;
        while (node != null)
        {
            var keys = node.Keys;
            for (var i = node.Length - 1; i >= 0; --i)
            {
                var a = arr[off++];
                var k = keys[i];
                if (a != k)
                    throw new Exception($"iteration failed. {a} != {k}");
            }
            node = node.Previous;
        }
        Console.WriteLine("success: " + count);
    }

    public static void InsertSkipList(long[] arr)
    {
        var count = arr.Length;
        var tree = new SkipList<long, long>(new Int64ComparerAscending());
        for (var i = 0; i < count; ++i)
        {
            var x = arr[i];
            tree.TryInsert(x, x + x);
        }
        for (var i = 0; i < count; ++i)
        {
            var x = arr[i];
            var exists = tree.TryGetValue(x, out var val);
            if (!exists || val != x + x)
                throw new Exception($"exists: {exists} ({x},{val}) != ({x},{x + x})");
        }
    }

    public static void InsertSortedDictionary(long[] arr)
    {
        var count = arr.Length;
        var tree = new SortedDictionary<long, long>(new LongComparer());
        for (var i = 0; i < count; ++i)
        {
            var x = arr[i];
            tree.Add(x, x + x);
        }
        for (var i = 0; i < count; ++i)
        {
            var x = arr[i];
            var exists = tree.TryGetValue(x, out var val);
            if (!exists || val != x + x)
                throw new Exception($"exists: {exists} ({x},{val}) != ({x},{x + x})");
        }
    }
}

public class LongComparer : IComparer<long>
{
    public int Compare(long x, long y)
    {
        var r = x - y;
        if (r == 0)
            return 0;
        return r < 0 ? -1 : 1;
    }
}
