﻿using Tenray.ZoneTree.Collections;
using Tenray.ZoneTree.Collections.BplusTree.Lock;
using Tenray.ZoneTree.Collections.BTree;
using Tenray.ZoneTree.Comparers;

namespace Playground.InMemoryTreeBenchmark;

public static class RandomIntInserts
{
    static readonly Random Random = new Random(0);
    
    static BTreeLockMode BTreeLockMode = BTreeLockMode.NodeLevelMonitor;

    public static int[] GetRandomArray(int count)
    {
        var arr =
                Enumerable
                .Range(0, count).ToArray();
        Shuffle(arr);
        return arr;
    }

    public static void Shuffle<T>(T[] arr)
    {
        for (int i = arr.Length - 1; i >= 1; i--)
        {
            var j = Random.Next(i + 1);
            (arr[i], arr[j]) = (arr[j], arr[i]);
        }
    }

    public static int[] GetSortedArray(int count)
    {
        var arr = Enumerable.Range(0, count).ToArray();
        return arr;
    }

    public static void InsertBTree(int[] arr)
    {
        var count = arr.Length;
        var tree = new UnsafeBTree<int, int>(new Int32ComparerAscending());
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
    }

    public static void InsertSafeBTree(int[] arr)
    {
        var count = arr.Length;
        var tree = new BTree<int, int>(new Int32ComparerAscending(), BTreeLockMode);
        for (var i = 0; i < count; ++i)
        {
            var x = arr[i];
            tree.TryInsert(x, x + x, out _);
        }
        for (var i = 0; i < count; ++i)
        {
            var x = arr[i];
            var exists = tree.TryGetValue(x, out var val);
            if (!exists || val != x + x)
                throw new Exception($"exists: {exists} ({x},{val}) != ({x},{x + x})");
        }
    }

    public static void InsertSkipList(int[] arr)
    {
        var count = arr.Length;
        var tree = new SkipList<int, int>(new Int32ComparerAscending());
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

    public static void InsertSortedDictionary(int[] arr)
    {
        var count = arr.Length;
        var tree = new SortedDictionary<int, int>(new IntComparer());
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

public class IntComparer : IComparer<int>
{
    public int Compare(int x, int y)
    {
        return x - y;
    }
}
