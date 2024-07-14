using NLua;

namespace Parser;

public abstract class Parser {
    public required string Name { get; set; }
    public List<Parser> Children { get; set; } = new();
    public string Script { get; set; } = "function _Create() return 'error(\\'script not specified\\')' end";

    abstract public ParseResult Parse(string text);
}