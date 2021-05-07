using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class UnitTest_TagBasics
{
    // Raw data for test tags
    Tag tagA, tagB, tagC;

    [SetUp]
    public void PrepareTagManager()
	{
        tagA = new Tag()
        {
            label = "Tag.A",
            id = 1
        };
        tagB = new Tag()
        {
            label = "Tag.B",
            id = 2
        };
        tagC = new Tag()
        {
            label = "Tag.C",
            id = 1
        };
    }

    [TearDown]
    public void ClearTagManager()
	{
        tagA = new Tag();
        tagB = new Tag();
        tagC = new Tag();
	}

    //----- Equality Tests -----//

    [Test]
    public void Tag_WithNonZeroID_IsValid()
    {
        Assert.AreEqual(true, tagA.IsValid());
    }

    [Test]
    public void Tag_WithZeroID_IsNotValid()
    {
        Tag invalidTag = new Tag();
        Assert.AreEqual(false, invalidTag.IsValid());
    }

    [Test]
    public void TwoTags_DifferentIDs_AreNotEqual()
	{
        Assert.AreEqual(false, tagA == tagB);
	}

    [Test]
    public void TwoTags_SameIDs_AreEqual()
	{
        Assert.AreEqual(true, tagA == tagC);
	}
}
