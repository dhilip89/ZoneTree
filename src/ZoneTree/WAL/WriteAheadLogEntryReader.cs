﻿using Tenray.ZoneTree.Collections;
using Tenray.ZoneTree.Collections.TimSort;
using Tenray.ZoneTree.Exceptions.WAL;

namespace Tenray.ZoneTree.WAL;

public static class WriteAheadLogEntryReader
{
    public delegate void LogEntryReaderDelegate<TLogEntry>(BinaryReader reader, ref TLogEntry logEntry);

    public delegate (bool isValid, TKey key, TValue value, long opIndex) LogEntryDeserializerDelegate<TKey, TValue, TLogEntry>(in TLogEntry logEntry);

    public static WriteAheadLogReadLogEntriesResult<TKey, TValue> ReadLogEntries<TKey, TValue, TLogEntry>(
        Stream stream,
        bool stopReadOnException,
        bool stopReadOnChecksumFailure,
        LogEntryReaderDelegate<TLogEntry> logEntryReader,
        LogEntryDeserializerDelegate<TKey, TValue, TLogEntry> logEntryDeserializer,
        bool sortByOpIndexes
        )
    {
        var result = new WriteAheadLogReadLogEntriesResult<TKey, TValue>
        {
            Success = true
        };
        stream.Seek(0, SeekOrigin.Begin);
        var binaryReader = new BinaryReader(stream);
        TLogEntry entry = default;

        var keyList = new List<TKey>();
        var valuesList = new List<TValue>();
        var opIndexes = new List<long>();
        var i = 0;
        var length = stream.Length;
        long maxOpIndex = 0;
        while (true)
        {
            var logEntryPosition = stream.Position;
            try
            {
                if (stream.Position == length)
                    break;
                logEntryReader(binaryReader, ref entry);
            }
            catch (EndOfStreamException e)
            {
                var ex = new IncompleteTailRecordFoundException(e)
                {
                    FileLength = length,
                    RecordPosition = logEntryPosition,
                    RecordIndex = i
                };
                result.Exceptions.Add(i, ex);
                result.Success = false;
                break;
            }
            catch (ObjectDisposedException e)
            {
                result.Exceptions.Add(i, new ObjectDisposedException($"ReadLogEntry failed. Index={i}", e));
                result.Success = false;
                break;
            }
            catch (IOException e)
            {
                result.Exceptions.Add(i, new IOException($"ReadLogEntry failed. Index={i}", e));
                result.Success = false;
                if (stopReadOnException) break;
            }
            catch (Exception e)
            {
                result.Exceptions.Add(i, new InvalidOperationException($"ReadLogEntry failed. Index={i}", e));
                result.Success = false;
                if (stopReadOnException) break;
            }
            TKey key = default;
            TValue value = default;
            long opIndex = default;
            try
            {
                (var isValid, key, value, opIndex) = logEntryDeserializer(in entry);                
                if (!isValid)
                {
                    if (!result.Exceptions.ContainsKey(i))
                        result.Exceptions.Add(i, new InvalidDataException($"Checksum failed. Index={i}"));
                    result.Success = false;
                    if (stopReadOnChecksumFailure) break;
                }
                else
                {
                    maxOpIndex = Math.Max(opIndex, maxOpIndex);
                }
            }
            catch (Exception e)
            {
                if (!result.Exceptions.ContainsKey(i))
                    result.Exceptions.Add(i, new InvalidDataException($"Deserilization of log entry failed. Index={i}", e));
                result.Success = false;
                if (stopReadOnChecksumFailure) break;
            }

            keyList.Add(key);
            valuesList.Add(value);
            opIndexes.Add(opIndex);
            ++i;
        }
        stream.Seek(0, SeekOrigin.End);
        if (sortByOpIndexes)
        {
            var len = opIndexes.Count;
            var arr = Enumerable.Range(0, len).ToArray();
            TimSort<int>.Sort(arr, new LogEntryReaderComparer(opIndexes));
            var keys = new TKey[len];
            var values = new TValue[len];
            for (var k = 0; k < len; k++)
            {
                var m = arr[k];
                keys[k] = keyList[m];
                values[k] = valuesList[m];
            }
            result.Keys = keys;
            result.Values = values;
            result.MaximumOpIndex = maxOpIndex;
            return result;
        }
        result.Keys = keyList;
        result.Values = valuesList;        
        return result;
    }

    public class LogEntryReaderComparer : IRefComparer<int>
    {
        private List<long> OpIndexes;

        public LogEntryReaderComparer(List<long> opIndexes)
        {
            OpIndexes = opIndexes;
        }

        public int Compare(in int x, in int y)
        {
            var r = OpIndexes[x] - OpIndexes[y];
            if (r == 0)
                return 0;
            return r < 0 ? -1 : 1;
        }
    }
}
