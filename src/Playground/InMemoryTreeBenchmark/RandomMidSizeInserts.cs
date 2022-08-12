﻿using Tenray.ZoneTree.Collections;
using Tenray.ZoneTree.Collections.BplusTree;

namespace Playground.InMemoryTreeBenchmark;

public static class RandomMidSizeInserts
{
    public static MidSizeKey[] GetRandomArray(long count)
    {
        var arr = GetSortedArray(count);
        RandomIntInserts.Shuffle(arr);
        return arr;
    }

    public static MidSizeKey[] GetSortedArray(long count)
    {
        var arr = new MidSizeKey[count];
        for (var i = 0; i < count; ++i)
            arr[i] = new MidSizeKey(i);
        return arr;
    }

    public static void InsertBplusTree(MidSizeKey[] arr)
    {
        var count = arr.Length;
        var tree = new BplusTree<MidSizeKey, MidSizeKey>(new MidSizeRefComparer());
        for(var i = 0; i < count; ++i)
        {
            var x = arr[i];
            tree.TryInsert(x, x);
        }
        for (var i = 0; i < count; ++i)
        {
            var x = arr[i];
            var exists = tree.TryGetValue(x, out var val);
            if (!exists || !val.Equals(x))
                throw new Exception($"exists: {exists} ({x},{val}) != ({x},{x})");
        }
    }

    public static void InsertSkipList(MidSizeKey[] arr)
    {
        var count = arr.Length;
        var tree = new SkipList<MidSizeKey, MidSizeKey>(new MidSizeRefComparer());
        for (var i = 0; i < count; ++i)
        {
            var x = arr[i];
            tree.TryInsert(x, x);
        }
        for (var i = 0; i < count; ++i)
        {
            var x = arr[i];
            var exists = tree.TryGetValue(x, out var val);
            if (!exists || !val.Equals(x))
                throw new Exception($"exists: {exists} ({x},{val}) != ({x},{x})");
        }
    }

    public static void InsertSortedDictionary(MidSizeKey[] arr)
    {
        var count = arr.Length;
        var tree = new SortedDictionary<MidSizeKey, MidSizeKey>(new MidSizeComparer());
        for (var i = 0; i < count; ++i)
        {
            var x = arr[i];
            tree.Add(x, x);
        }
        for (var i = 0; i < count; ++i)
        {
            var x = arr[i];
            var exists = tree.TryGetValue(x, out var val);
            if (!exists || !val.Equals(x))
                throw new Exception($"exists: {exists} ({x},{val}) != ({x},{x})");
        }
    }
}

public struct MidSizeKey
{
    public long Key;
    public long A1;
    public long A2;
    public long A3;
    public long A4;

    public MidSizeKey(int key) : this()
    {
        Key = key;
    }
}

public class MidSizeComparer : IComparer<MidSizeKey>
{
    public int Compare(MidSizeKey x, MidSizeKey y)
    {
        var r = x.Key - y.Key;
        if (r == 0)
            return 0;
        return r < 0 ? -1 : 1;
    }
}
public class MidSizeRefComparer : IRefComparer<MidSizeKey>
{
    public int Compare(in MidSizeKey x, in MidSizeKey y)
    {
        var r = x.Key - y.Key;
        if (r == 0)
            return 0;
        return r < 0 ? -1 : 1;
    }
}
