﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenray;
using ZoneTree.Collections;

namespace ZoneTree.UnitTests;

public class SkipListTests
{
    [Test]
    public void SkipListIteration()
    {
        var n = 2000;
        var skipList = new SkipList<int, int>(new IntegerComparerAscending(), (int)Math.Log2(n) + 1);
        for (var i = 0; i < n; ++i)
            skipList.Insert(i, i + i);
        
        var node = skipList.FirstNode;
        for (var i = 0; i < n; ++i)
        {
            Assert.That(node.Key, Is.EqualTo(i));
            Assert.That(node.Value, Is.EqualTo(i + i));
            node = node.NextNode;
        }

        node = skipList.LastNode;
        for (var i = n - 1; i >= 0; --i)
        {
            Assert.That(node.Key, Is.EqualTo(i));
            Assert.That(node.Value, Is.EqualTo(i + i));
            node = node.PreviousNode;
        }
    }

    [Test]
    public void SkipListIterator()
    {
        var n = 3;
        var skipList = new SkipList<int, int>(new IntegerComparerAscending(), (int)Math.Log2(n) + 1);
        for (var i = 0; i < n; ++i)
            skipList.Insert(i, i + i);
        var indexedReader = new SkipListIndexedReader<int, int>(skipList);
        for (var i = 0; i < n; ++i)
        {
            Assert.That(indexedReader.GetKey(i), Is.EqualTo(i));
            Assert.That(indexedReader.GetValue(i), Is.EqualTo(i + i));
        }

        for (var i = n - 1; i >= 0; --i)
        {
            Assert.That(indexedReader.GetKey(i), Is.EqualTo(i));
            Assert.That(indexedReader.GetValue(i), Is.EqualTo(i + i));
        }

        var iterator = new SkipListSeekableIterator<int, int>(skipList);
        iterator.SeekEnd();
        for (var i = n - 1; i >= 0; --i)
        {
            Assert.That(iterator.CurrentKey, Is.EqualTo(i));
            Assert.That(iterator.CurrentValue, Is.EqualTo(i + i));
            iterator.Prev();
        }
        for (var i = 0; i < n; ++i)
        {
            Assert.That(iterator.CurrentKey, Is.EqualTo(i));
            Assert.That(iterator.CurrentValue, Is.EqualTo(i + i));
            iterator.Next();
        }
        Assert.That(iterator.Prev(), Is.True);
        Assert.That(iterator.Next(), Is.True);
        Assert.That(iterator.Next(), Is.False);
    }

    [Test]
    public void SkipListLowerOrEqualBound()
    {
        int n = 10;
        var skipList = new SkipList<int, int>(new IntegerComparerAscending(), (int)Math.Log2(n) + 1);
        for (var i = 1; i < n; i += 2)
            skipList.Insert(i, i);
        // 1 3 5 7 9
        Assert.That(skipList.GetLastNodeSmallerOrEqual(4).Key, Is.EqualTo(3));
        Assert.That(skipList.GetLastNodeSmallerOrEqual(3).Key, Is.EqualTo(3));
        Assert.That(skipList.GetLastNodeSmallerOrEqual(-1), Is.Null);
        Assert.That(skipList.GetLastNodeSmallerOrEqual(10).Key, Is.EqualTo(9));
        Assert.That(skipList.GetLastNodeSmallerOrEqual(9).Key, Is.EqualTo(9));
        Assert.That(skipList.GetLastNodeSmallerOrEqual(1).Key, Is.EqualTo(1));
        Assert.That(skipList.GetLastNodeSmallerOrEqual(5).Key, Is.EqualTo(5));
        Assert.That(skipList.GetLastNodeSmallerOrEqual(7).Key, Is.EqualTo(7));
        Assert.That(skipList.GetLastNodeSmallerOrEqual(8).Key, Is.EqualTo(7));
        Assert.That(skipList.GetLastNodeSmallerOrEqual(0), Is.Null);

        Assert.That(skipList.GetFirstNodeGreaterOrEqual(-1).Key, Is.EqualTo(1));
        Assert.That(skipList.GetFirstNodeGreaterOrEqual(1).Key, Is.EqualTo(1));
        Assert.That(skipList.GetFirstNodeGreaterOrEqual(2).Key, Is.EqualTo(3));
        Assert.That(skipList.GetFirstNodeGreaterOrEqual(3).Key, Is.EqualTo(3));
        Assert.That(skipList.GetFirstNodeGreaterOrEqual(4).Key, Is.EqualTo(5));
        Assert.That(skipList.GetFirstNodeGreaterOrEqual(5).Key, Is.EqualTo(5));
        Assert.That(skipList.GetFirstNodeGreaterOrEqual(6).Key, Is.EqualTo(7));
        Assert.That(skipList.GetFirstNodeGreaterOrEqual(7).Key, Is.EqualTo(7));
        Assert.That(skipList.GetFirstNodeGreaterOrEqual(8).Key, Is.EqualTo(9));
        Assert.That(skipList.GetFirstNodeGreaterOrEqual(9).Key, Is.EqualTo(9));
        Assert.That(skipList.GetFirstNodeGreaterOrEqual(10), Is.Null);
    }
}