using Raspite.Library;
using System.Collections.Generic;
using System.IO;

namespace Raspite.WinUI.Models;

internal sealed class Node
{
    public string Name { get; }

    public IEnumerable<Node>? Children { get; }

    public Node(NbtTag tag, string path)
    {
        Name = string.IsNullOrWhiteSpace(tag.Name) ? Path.GetFileName(path)! : tag.Name;

        var children = new List<Node>();

        if (tag is NbtTag.Compound compound)
        {
            foreach (var child in compound.Children)
            {
                children.Add(new Node(child, path));
            }

        }
        else if (tag is NbtTag.List list)
        {
            foreach (var child in list.Children)
            {
                children.Add(new Node(child, child.GetType().Name));
            }
        }
        else
        {
            children = null;
        }

        Children = children;
    }
}