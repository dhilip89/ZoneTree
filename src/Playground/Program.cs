﻿using Playground.Benchmark;
using Tenray.ZoneTree.Core;
using Tenray.ZoneTree.WAL;

var custom = false;
if (custom)
{
    TestConfig.EnableIncrementalBackup = true;
    TestConfig.RecreateDatabases = false;
    TestConfig.MutableSegmentMaxItemCount = 100000;
    TestConfig.ThresholdForMergeOperationStart = 300000;
    TestConfig.WALCompressionBlockSize = 1024 * 1024 * 1;
    TestConfig.DiskCompressionBlockSize = 1024 * 1024 * 100;
}
TestConfig.EnableParalelInserts = false;
/*TestConfig.DiskSegmentMaximumCachedBlockCount = 32;
TestConfig.DiskCompressionBlockSize = 1024 * 1024 * 100;
TestConfig.WALCompressionBlockSize = 1024 * 1024;
TestConfig.MinimumSparseArrayLength = 0;*/
TestConfig.DiskSegmentMode = DiskSegmentMode.MultipleDiskSegments;

var testAll = true;

if (testAll)
{
    BenchmarkGroups.InsertIterate1(50_000_000, WriteAheadLogMode.Lazy);
}
else
{
    BenchmarkGroups.Insert1(3_000_000, WriteAheadLogMode.Lazy);
    BenchmarkGroups.Iterate1(3_000_000, WriteAheadLogMode.Lazy);
}
