using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


public class UnitTest_TagManager
{
	string[] rawTags = { 
		"Weapon.Rifle.M4",
		"Weapon.Rifle.AK47",
		"Weapon.Rifle.SCAR",
		"Weapon.Rifle.Sniper.M80",
		"Weapon.Rifle.Sniper.AWP",
		"Weapon.SMG.Vector",
		"Weapon.SMG.MP5",
		"Team.T",
		"Team.CT"
	};
	
	// Each tag label which is expected to be created
	string[] expectedTags = {
			"Weapon",
			"Weapon.Rifle",
			"Weapon.Rifle.M4",
			"Weapon.Rifle.AK47",
			"Weapon.Rifle.SCAR",
			"Weapon.Rifle.Sniper",
			"Weapon.Rifle.Sniper.M80",
			"Weapon.Rifle.Sniper.AWP",
			"Weapon.SMG",
			"Weapon.SMG.Vector",
			"Weapon.SMG.MP5",
			"Team",
			"Team.T",
			"Team.CT"
		};

	[SetUp]
	public void SetupManager() {
		TagManager.LoadTagList(new string[0], true);
	}

	[TearDown]
	public void TearDownManager() {
	}

	[Test]
	public void TagManager_LoadingTag_CreatesTag()
	{
		string expectedLabel = "Test.Expected";
		TagManager.LoadTag(expectedLabel);

		Tag loadedTag = TagManager.GetTagByLabel(expectedLabel);
		Assert.AreEqual(expectedLabel, loadedTag.GetLabel());
	}

	// Check that each appropriate tag exists
	[Test]
	public void TagManager_ParsingTags_CreatesEachTag()
	{
		TagManager.LoadTagList(rawTags, true);

		foreach (string expectedLabel in expectedTags)
		{
			Assert.AreEqual(true, TagManager.GetTagByLabel(expectedLabel).IsValid());
		}
	}

	[Test]
	public void TagManager_ParsingTags_CreatesNoDuplicates()
	{
		TagManager.LoadTagList(rawTags);

		foreach ( string firstLabel in expectedTags )
		{
			Tag firstTag = TagManager.GetTagByLabel(firstLabel);

			foreach ( string secondLabel in expectedTags )
			{
				if (firstLabel == secondLabel)
				{
					continue;
				}

				Tag secondTag = TagManager.GetTagByLabel(secondLabel);

				if ( !firstTag.IsValid() ) Debug.LogError("Tag not loaded: " + firstTag.ToString() + " label: " + firstLabel);
				if ( !secondTag.IsValid() ) Debug.LogError("Tag not loaded: " + secondTag.ToString() + "label: " + secondLabel);
				if ( firstTag == secondTag )
				{
					Debug.LogWarning("Tags were loaded as equal: " + firstTag.ToString() + "(label: " + firstLabel + ") " + secondTag.ToString() + "(label: " + secondLabel + ")");
				}
				Assert.AreEqual(false, firstTag == secondTag);
			}
		}
	}
}
