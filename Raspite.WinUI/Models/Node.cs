using Raspite.Library;
using System;
using System.Collections.Generic;
using System.IO;

namespace Raspite.WinUI.Models;

internal sealed class Node
{
    public string Name { get; }

    public string Type { get; }

    public string Color { get; }

    public IEnumerable<Node>? Children { get; }

    public NbtTag Tag { get; }

    public Node(NbtTag tag, string path)
    {
        Tag = tag;

        Name = string.IsNullOrWhiteSpace(tag.Name) ? Path.GetFileName(path)! : tag.Name;

        var type = tag.GetType().Name;
        Type = type.ToLower();

        Color = type switch
        {
            nameof(NbtTag.Byte) => "#00E085",
            nameof(NbtTag.Short) => "#00E085",
            nameof(NbtTag.Int) => "#00E085",
            nameof(NbtTag.Long) => "#00E085",
            nameof(NbtTag.Float) => "#00C2AE",
            nameof(NbtTag.Double) => "#00C2AE",
            nameof(NbtTag.ByteArray) => "#05ADA0",
            nameof(NbtTag.String) => "#0084FF",
            nameof(NbtTag.List) => "#E600FF",
            nameof(NbtTag.Compound) => "#E600FF",
            nameof(NbtTag.IntArray) => "#05ADA0",
            nameof(NbtTag.LongArray) => "#05ADA0",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown type.")
        };

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
                children.Add(new Node(child, type));
            }
        }
        else
        {
            children = null;
        }

        Children = children;
    }
}