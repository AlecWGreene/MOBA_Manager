using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Tag : IComparable<Tag>
{
	public Tag(string inLabel)
	{
		label = inLabel;
		partition = label.Split('.');

		id = 0;
	}

	public int id;
	public string label;

	public string[] partition;

	public string GetLastLabel()
	{
		return partition[partition.Length - 1];
	}

	public string GetLabel()
	{
		return label.Replace("root.", "");
	}

	public override string ToString()
	{
		return label + "#" + id;
	}

	public bool IsValid()
	{
		return id != 0;
	}

	// Comparison operator
	public int CompareTo(Tag inTag)
	{
		return id.CompareTo(inTag.id);
	}

	// Equality operators
	public override bool Equals(object other)
	{
		return (other.GetType().Equals(typeof(Tag))) && id == ((Tag)other).id;
	}

	public static bool operator ==(Tag A, Tag B)
	{
		return A.id == B.id;
	}

	public static bool operator !=(Tag A, Tag B)
	{
		return A.id != B.id;
	}

	public override int GetHashCode()
	{
		return id;
	}
}

public struct TagContainer
{
	SortedDictionary<Tag, int> memberTags;

	/// <summary>
	/// Adds a tag to the container, 
	/// </summary>
	/// <param name="inTag"></param>
	/// <param name="numberStacks"></param>
	public void AddTag(Tag inTag, int numberStacks = 1)
	{
		// Sanity check on Tag dictionary
		if ( memberTags == null )
		{
			memberTags = new SortedDictionary<Tag, int>();
		}

		// Sanity check on Tag
		if ( !inTag.IsValid() )
		{
			return;
		}

		// Sanity check numberStacks
		if (numberStacks <= 0)
		{
			return;
		}

		// Check for existing tag in set
		if (memberTags.ContainsKey(inTag)) 
		{
			memberTags[inTag] += numberStacks;
		}
		else
		{
			memberTags.Add(inTag, numberStacks);
		}
	}

	/// <summary>
	/// Retrieves the number of Tag stacks in the container
	/// </summary>
	/// <param name="inTag">Tag to check the container for</param>
	/// <returns>Number of stacks</returns>
	public int GetTagCount(Tag inTag)
	{
		if (memberTags != null && memberTags.ContainsKey(inTag))
		{
			return memberTags[inTag];
		}

		return 0;
	}

	/// <summary>
	/// Checks if a tag exists in the container
	/// </summary>
	/// <param name="inTag">Tag to check for</param>
	/// <returns>True if the Tag has 1 or more stacks</returns>
	public bool HasTag(Tag inTag)
	{
		if (memberTags == null)
		{
			return false;
		}

		return memberTags.ContainsKey(inTag);
	}

	/// <summary>
	/// Removes stacks of a Tag from the container
	/// </summary>
	/// <param name="inTag">Tag to remove</param>
	/// <param name="numberStacks">Number of stacks to remove</param>
	public void RemoveTag(Tag inTag, int numberStacks = 1)
	{
		if (memberTags.ContainsKey(inTag))
		{
			memberTags[inTag] -= numberStacks;

			if (memberTags[inTag] <= 0)
			{
				memberTags.Remove(inTag);
			}
		}
	}

	/// <summary>
	/// Clears all stacks of a Tag
	/// </summary>
	/// <param name="inTag">Tag to clear</param>
	public void ClearTag(Tag inTag)
	{
		if (memberTags.ContainsKey(inTag))
		{
			memberTags.Remove(inTag);
		}
	}

	/// <summary>
	/// Returns a string describing the container's contents
	/// </summary>
	/// <returns>JSON style representation of the container</returns>
	public override string ToString()
	{
		string returnString = "TagSet{\n";
		if ( memberTags != null && memberTags.Count > 0)
		{
			foreach ( KeyValuePair<Tag, int> entry in memberTags )
			{
				returnString += $"\t{entry.Key}: {entry.Value},\n";
			}
		}

		return returnString.TrimEnd(',', '\n') + "\n}";
	}
}

public static class TagManager
{
	static readonly bool willDebugPrint = true;
	static Tree<Tag> tagTree;

	/// <summary>
	/// Retrieves a Tag from the tree which matches the label
	/// </summary>
	/// <param name="inLabel">Label to search for</param>
	/// <returns>Tag with matching label, or root if none are found</returns>
	public static Tag GetTagByLabel(string inLabel)
	{
		string[] sublabelArray = inLabel.Split('.');
		TreeNode<Tag> currentNode = tagTree.root;
		for ( int depth = 0; depth < sublabelArray.Length; depth++ )
		{
			byte tagDepthIndex = FindLabelInCategory(sublabelArray[depth], currentNode);
			if ( tagDepthIndex == 0 )
			{
				return tagTree.root.data;
			}
			else
			{
				currentNode = currentNode.children[tagDepthIndex - 1];
			}
		}

		return currentNode.data;
	}

	// Load list of names
	public static void LoadTagList(string[] inNames, bool createNewTree = false)
	{
		if (createNewTree || tagTree == null)
		{
			tagTree = new Tree<Tag>(new Tag("root"));
		}

		foreach ( string name in inNames )
		{
			LoadTag(name);
		}
	}

	// Load each subtag
	public static void LoadTag(string inName)
	{
		DebugLog("Loading Tag: " + inName);
		if (tagTree == null)
		{
			tagTree = new Tree<Tag>(new Tag("root"));
		}

		string[] subLabels = inName.Split('.');
		TreeNode<Tag> currentNode = tagTree.root;
		for ( int depth = 0; depth < subLabels.Length; depth++ )
		{
			byte tagDepthIndex = FindLabelInCategory(subLabels[depth], currentNode);
			if ( tagDepthIndex == 0 )
			{
				Tag newTag = new Tag(currentNode.data.label + '.' + subLabels[depth]);
				newTag.id = (currentNode.data.id << 8) + (currentNode.children.Count + 1);
				TreeNode<Tag> newNode = new TreeNode<Tag>(newTag);
				currentNode.AddChild(newNode);
				currentNode = newNode;
				DebugLog($"Tag created {newTag.label}#{newTag.id}");
			}
			else
			{
				currentNode = currentNode.children[tagDepthIndex - 1];
			}
		}

		DebugLog("\n");
	}

	// Looks for a sublabel in the children of a tagTree node
	public static byte FindLabelInCategory(string label, TreeNode<Tag> categoryParent)
	{
		byte matchIndex = (byte)(categoryParent.children.FindIndex(node => node.data.GetLastLabel() == label) + 1);
		if ( matchIndex == 0 )
		{
			DebugLog($"Tag {categoryParent.data.label + '.' + label} was not found.");
		}

		return matchIndex;
	}

	// Convert a tag to an array of bytes for fast comparison
	public static byte[] ConvertTagToFlagArray(Tag inTag)
	{
		int numBytes = 0;
		List<byte> tempList = new List<byte>();
		numBytes = (int)(Mathf.Ceil(Mathf.Log(inTag.id, 16) / 2));

		if ( numBytes > 0 )
		{
			for ( int idx = 0; idx < numBytes; idx++ )
			{
				int shiftSize = 8 * (numBytes - idx - 1);
				int idxFlag = 0xFF << shiftSize;
				byte match = (byte)((inTag.id & idxFlag) >> shiftSize);
				tempList.Insert(0, match);
			}
		}

		return tempList.ToArray();
	}

	// Convert a potential id into the array of bytes that it would exist as
	public static byte[] ConvertTagToFlagArray(int inID)
	{
		int numBytes = 0;
		List<byte> tempList = new List<byte>();
		numBytes = (int)(Mathf.Ceil(Mathf.Log(inID, 16) / 2));

		if ( numBytes > 0 )
		{
			for ( int idx = 0; idx < numBytes; idx++ )
			{
				int shiftSize = 8 * (numBytes - idx - 1);
				int idxFlag = 0xFF << shiftSize;
				byte match = (byte)((inID & idxFlag) >> shiftSize);
				tempList.Add(match);
			}
		}

		return tempList.ToArray();
	}

	// Wrapper method for debugging logging
	static void DebugLog(string inMessage)
	{
		if ( willDebugPrint ) Debug.Log(inMessage);
	}

	public static void PrintTree()
	{
		PrintSubTree(tagTree.root);
	}

	public static void PrintSubTree(TreeNode<Tag> inParent)
	{
		DebugLog(inParent.data.GetLabel());
		foreach ( TreeNode<Tag> child in inParent.children )
		{
			PrintSubTree(child);
		}
	}
}