﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Engines;
using Tenray.ZoneTree.Collections.BplusTree.Lock;
using Tenray.ZoneTree.Collections.BTree;
using Tenray.ZoneTree.Comparers;

namespace Playground.InMemoryTreeBenchmark;

[HtmlExporter]
[SimpleJob(RunStrategy.ColdStart, targetCount: 1)]
[MinColumn, MaxColumn, MeanColumn, MedianColumn, /*AllStatisticsColumn*/]
[MemoryDiagnoser]
/*[HardwareCounters(
    HardwareCounter.TotalCycles,
    HardwareCounter.TotalIssues,
    HardwareCounter.CacheMisses,
    HardwareCounter.Timer)]*/
public class BTreeLockModeMassiveInsertTest
{
    readonly int Count = 3_000_000;
    readonly bool Shuffled = true;

    [GlobalSetup]
    public void Setup()
    {
        Data = Shuffled ?
            RandomLongInserts.GetRandomArray(Count) :
            RandomLongInserts.GetSortedArray(Count);
    }

    long[] Data = Array.Empty<long>();

    [Benchmark]
    public void TopLevelMonitor() => 
        MassiveInsertsAndReads(Data, BTreeLockMode.TopLevelMonitor);

    [Benchmark]
    public void TopLevelReaderWriter() =>
        MassiveInsertsAndReads(Data, BTreeLockMode.TopLevelReaderWriter);

    [Benchmark]
    public void NodeLevelMonitor() =>
        MassiveInsertsAndReads(Data, BTreeLockMode.NodeLevelMonitor);

    [Benchmark]
    public void NodeLevelReaderWriter() =>
        MassiveInsertsAndReads(Data, BTreeLockMode.NodeLevelReaderWriter);

    public static void MassiveInsertsAndReads(long[] arr, BTreeLockMode lockMode = BTreeLockMode.NodeLevelMonitor)
    {
        var readCount = 0;
        var tree = new BTree<long, long>(new Int64ComparerAscending(),
            lockMode);
        var task1 = Parallel.ForEachAsync(arr, (i, t) =>
        {
            tree.TryInsert(i, i);
            return ValueTask.CompletedTask;
        });
        Thread.Sleep(1);
        var task2 = Parallel.ForEachAsync(arr, (i, t) =>
        {
            if (tree.TryGetValue(i, out var j))
            {
                if (i != j)
                    throw new Exception($"{i} != {j}");
                Interlocked.Increment(ref readCount);
            }
            return ValueTask.CompletedTask;
        });
        task1.Wait();
        task2.Wait();
    }
}