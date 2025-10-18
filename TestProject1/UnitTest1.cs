using ConsoleApp2;

namespace TestProject1;

// Minimal test implementation of IAstNode for building trees in tests.
public sealed class TestNode : IAstNode
{
  private readonly List<TestNode> _children = [];

  public TestNode(string text = "", bool isTrivia = false)
  {
    Text = text;
    IsWhitespaceOrComment = isTrivia;
  }

  private string Text { get; }
  public bool IsWhitespaceOrComment { get; }

  public IAstNode Parent { get; private set; }
  public IAstNode FirstChild => _children.Count == 0 ? null : _children[0];
  public IAstNode NextSibling { get; private set; }
  public IAstNode PrevSibling { get; private set; }

  public TestNode Add(params TestNode[] children)
  {
    foreach (var ch in children)
    {
      ch.Parent = this;
      if (_children.Count > 0)
      {
        var prev = _children[^1];
        prev.NextSibling = ch;
        ch.PrevSibling = prev;
      }
      _children.Add(ch);
    }
    return this;
  }

  public string GetText()
  {
    return _children.Count == 0 ? Text : 
      string.Concat(_children.Select(c => c.GetText()));
  }
}

public class AstNodeExtensionsIsEquivalentToTests
{
  [Fact]
  public void Same_Tokens_Same_Order_Ignoring_Whitespace()
  {
    // Tree A: [id][ws][op][ws][num]
    var a = new TestNode()
      .Add(new TestNode("x"),
        new TestNode(" ", isTrivia: true),
        new TestNode("+"),
        new TestNode("\n", isTrivia: true),
        new TestNode("1"));

    // Tree B: [ws][id][ws][op][ws][num][ws]
    var b = new TestNode()
      .Add(new TestNode("\t", isTrivia: true),
        new TestNode("x"),
        new TestNode("  ", isTrivia: true),
        new TestNode("+"),
        new TestNode("  ", isTrivia: true),
        new TestNode("1"),
        new TestNode("//c", isTrivia: true));

    Assert.True(a.IsEquivalentTo(b));
    Assert.True(b.IsEquivalentTo(a));
  }

  [Fact]
  public void Different_Token_Text_Fails()
  {
    var a = new TestNode().Add(new TestNode("x"), new TestNode("+"), new TestNode("1"));
    var b = new TestNode().Add(new TestNode("x"), new TestNode("-"), new TestNode("1"));

    Assert.False(a.IsEquivalentTo(b));
    Assert.False(b.IsEquivalentTo(a));
  }

  [Fact]
  public void Different_Number_Of_NonTrivia_Leaves_Fails()
  {
    var a = new TestNode().Add(new TestNode("x"), new TestNode("+"), new TestNode("1"));
    var b = new TestNode().Add(new TestNode("x"), new TestNode("+"));

    Assert.False(a.IsEquivalentTo(b));
    Assert.False(b.IsEquivalentTo(a));
  }

  [Fact]
  public void Structure_Differs_But_Tokens_Equal_Passes()
  {
    // A: ((x + 1))
    var a = new TestNode()
      .Add(new TestNode()
        .Add(new TestNode("x"),
          new TestNode("+"),
          new TestNode("1")));

    // B: x + 1 with extra trivia nodes and another wrapping node
    var b = new TestNode()
      .Add(new TestNode(" ", isTrivia: true),
        new TestNode().Add(new TestNode("x")),
        new TestNode("+"),
        new TestNode().Add(new TestNode("1")),
        new TestNode("\n", isTrivia: true));

    Assert.True(a.IsEquivalentTo(b));
  }

  [Fact]
  public void Only_Trivia_In_Both_Passes()
  {
    var a = new TestNode().Add(new TestNode(" ", isTrivia: true),
      new TestNode("// comment", isTrivia: true));
    var b = new TestNode().Add(new TestNode("\n", isTrivia: true));

    Assert.True(a.IsEquivalentTo(b));
  }
}