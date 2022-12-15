using Raspite.Library;
using System;
using System.Collections.Generic;
using System.IO;

namespace Raspite.WinUI.Models;

internal sealed class Node
{
    public string? Name { get; }

    public string? Value { get; }

    public string Type { get; }

    public string? Color { get; }

    public IEnumerable<Node>? Children { get; }

    public NbtTag Tag { get; }

    public Node(NbtTag tag, string? path)
    {
        Tag = tag;

        switch (tag)
        {
            case NbtTag.Byte:
            case NbtTag.Short:
            case NbtTag.Int:
            case NbtTag.Long:
            case NbtTag.Float:
            case NbtTag.Double:
            case NbtTag.String:
                Color = tag is NbtTag.String ? "#0084FF" : "#00E085";
                Value = ((NbtTag.ValueBase) tag).Value.ToString();
                break;

            case NbtTag.IntArray:
            case NbtTag.LongArray:
            case NbtTag.ByteArray:
                Color = "#05ADA0";
                break;

            case NbtTag.Compound:
            case NbtTag.List:
                Color = "#E600FF";
                break;
        }


        Name = string.IsNullOrWhiteSpace(tag.Name) ? Path.GetFileName(path)! : tag.Name;

        var type = tag.GetType().Name;
        Type = type.ToLower();

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
                children.Add(new Node(child, null));
            }
        }
        else
        {
            children = null;
        }

        Children = children;
    }
}