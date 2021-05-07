using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree<T>
{
	public Tree(T inRootValue)
	{
		root = new TreeNode<T>(inRootValue);
	}

	public TreeNode<T> root;
}

public class TreeNode<T>
{
	public TreeNode(T inData)
	{
		data = inData;
	}

	public T data;

	public TreeNode<T> parent;
	public List<TreeNode<T>> children = new List<TreeNode<T>>();

	public void AddChild(TreeNode<T> inChild)
	{
		children.Add(inChild);
	}

	public void SetParent(TreeNode<T> inNode)
	{
		parent = inNode;
	}
}
