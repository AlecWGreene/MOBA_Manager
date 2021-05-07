using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class UnitTest_TagContainers
{
    TagContainer emptyContainer;
    TagContainer dogContainer;
    TagContainer fullContainer;

    Tag dogTag, catTag;

    [SetUp]
    public void PrepareContainers()
	{
        // Init containers
        emptyContainer = new TagContainer();
        dogContainer = new TagContainer();
        fullContainer = new TagContainer();

        // Create temp tags to not interact with the individual tags
        Tag tempCatTag = new Tag()
        {
            label = "Animal.Cat",
            id = 2
        };
        Tag tempDogTag = new Tag()
        {
            label = "Animal.Dog",
            id = 1
        };

        // Add dog container tags
        dogContainer.AddTag(new Tag()
        {
            label = "Animal.Dog",
            id = 1
        }, 1);

        // Add full container tags 
        fullContainer.AddTag(tempDogTag, 2);
        fullContainer.AddTag(tempCatTag, 1);
    }

    [SetUp]
    public void PrepareTags()
    {
        dogTag = new Tag()
        {
            label = "Animal.Dog",
            id = 1
        };
        catTag = new Tag()
        {
            label = "Animal.Cat",
            id = 2
        };
    }

    [TearDown]
    public void ResetTags()
	{
        emptyContainer = new TagContainer();
        dogContainer = new TagContainer();
        fullContainer = new TagContainer();

        dogTag = new Tag();
        catTag = new Tag();
    }

    //----- Adding Tags -----//

    [Test]
    public void EmptyContainer_AddedTag_AppliesOneStack()
	{
        emptyContainer.AddTag(dogTag);

        Assert.AreEqual(true, emptyContainer.HasTag(dogTag));
        Assert.AreEqual(1, emptyContainer.GetTagCount(dogTag));
    }
    
    [Test]
    public void EmptyContainer_AddedTagTwice_AppliesTwoStacks()
    {
        emptyContainer.AddTag(dogTag);
        emptyContainer.AddTag(dogTag);

        Assert.AreEqual(true, emptyContainer.HasTag(dogTag));
        Assert.AreEqual(2, emptyContainer.GetTagCount(dogTag));
    }

    [Test]
    public void EmptyContainer_AddingTwoTags_AppliesOneStackEach()
    {
        emptyContainer.AddTag(dogTag);
        emptyContainer.AddTag(catTag);

        Assert.AreEqual(true, emptyContainer.HasTag(dogTag));
        Assert.AreEqual(true, emptyContainer.HasTag(catTag));
        Assert.AreEqual(1, emptyContainer.GetTagCount(dogTag));
        Assert.AreEqual(1, emptyContainer.GetTagCount(dogTag));
    }

    [Test]
    public void ContainerWithTags_AddMultipleStacks_AppliesCorrectNumber()
    {
        dogContainer.AddTag(dogTag, -1);
        Assert.AreEqual(1, dogContainer.GetTagCount(dogTag));

        fullContainer.AddTag(dogTag, 3);
        Assert.AreEqual(5, fullContainer.GetTagCount(dogTag));
    }

    [Test]
    public void EmptyContainer_AddingInvalidTags_DoesNothing()
	{
        emptyContainer.AddTag(new Tag());

        Assert.AreEqual(false, emptyContainer.HasTag(new Tag()));
	}

    //----- Removing Tags -----//

    [Test]
    public void ContainerWithTags_RemovingTag_RemovesOneStack()
	{
        dogContainer.RemoveTag(dogTag);
        Assert.AreEqual(false, dogContainer.HasTag(dogTag));

        fullContainer.RemoveTag(dogTag);
        Assert.AreEqual(1, fullContainer.GetTagCount(dogTag));
        Assert.AreEqual(true, fullContainer.HasTag(catTag));
    }

    [Test]
    public void ContainerWithTags_ClearingTag_RemovesAllStacks()
    {
        dogContainer.ClearTag(dogTag);
        Assert.AreEqual(false, dogContainer.HasTag(dogTag));

        fullContainer.ClearTag(dogTag);
        Assert.AreEqual(false, fullContainer.HasTag(dogTag));
        Assert.AreEqual(true, fullContainer.HasTag(catTag));
    }

    [Test]
    public void ContainerWithTags_RemovingExcessStacks_ClearsTag()
	{
        dogContainer.RemoveTag(dogTag, 1);
        Assert.AreEqual(false, dogContainer.HasTag(dogTag));

        fullContainer.RemoveTag(dogTag, 3);
        Assert.AreEqual(false, fullContainer.HasTag(dogTag));
        Assert.AreEqual(true, fullContainer.HasTag(catTag));
    }

    //----- Query Methods -----//

    [Test]
    public void EmptyContainer_QueriedForTag_ReturnsZero()
    {
        Assert.AreEqual(false, emptyContainer.HasTag(dogTag));
        Assert.AreEqual(0, emptyContainer.GetTagCount(dogTag));
    }

    [Test]
    public void ContainerWithTags_QueriedForTag_ReturnsCorrectAmount()
	{
        Assert.AreEqual(true, fullContainer.HasTag(dogTag));
        Assert.AreEqual(2, fullContainer.GetTagCount(dogTag));
    }

}
