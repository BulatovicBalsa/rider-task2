namespace ConsoleApp2;

using System;
using System.Collections.Generic;

public interface IAstNode
{
    IAstNode Parent { get; }
    IAstNode FirstChild { get; }
    IAstNode NextSibling { get; }
    IAstNode PrevSibling { get; }

    bool IsWhitespaceOrComment { get; }

    // returns this node’s text (for leaves)
    // or concatenated text of the subtree
    string GetText();
}

public static class AstNodeExtensions
{
    /// <summary>
    /// Checks if two AST nodes are equivalent
    /// ignoring whitespace/comment tokens.
    /// </summary>
    public static bool IsEquivalentTo(this IAstNode node, IAstNode other)
    {
        if (ReferenceEquals(node, other)) return true;

        using var e1 = EnumerateNonTriviaLeaves(node).GetEnumerator();
        using var e2 = EnumerateNonTriviaLeaves(other).GetEnumerator();

        while (true)
        {
            var m1 = e1.MoveNext();
            var m2 = e2.MoveNext();
            if (m1 != m2) return false; // different number of non-trivia leaves
            if (!m1) return true;       // both finished → equivalent
            if (!string.Equals(e1.Current.GetText(), e2.Current.GetText(), StringComparison.Ordinal))
                return false;
        }
    }

    private static IEnumerable<IAstNode> EnumerateNonTriviaLeaves(IAstNode root)
    {
        // Non-recursive DFS yielding only non-whitespace/comment leaves in document order.
        var stack = new Stack<IAstNode>();
        stack.Push(root);
        while (stack.Count > 0)
        {
            var n = stack.Pop();
            if (n.FirstChild is null)
            {
                if (!n.IsWhitespaceOrComment)
                    yield return n;
            }
            else
            {
                // Push children in reverse to visit FirstChild first.
                var children = new List<IAstNode>();
                var c = n.FirstChild;
                while (c != null)
                {
                    children.Add(c); 
                    c = c.NextSibling;
                }
                for (var i = children.Count - 1; i >= 0; --i)
                    stack.Push(children[i]);
            }
        }
    }
}
