using System.Text.RegularExpressions;
using NLua;

namespace Parser;

public class Selector : Parser {

    public Selector() {
        Script = File.ReadAllText("selector.lua");
    }

    public override ParseResult Parse(string text) {
        var status = ParseResultStatus.ALL_CHILDREN_FAILED;
        var children = new List<ParseResult>();
        var allDidntMatch = true;
        foreach (var child in Children) {
            var childResult = child.Parse(text);
            children.Add(childResult);

            if (childResult.Status == ParseResultStatus.DIDNT_MATCH) continue;
            allDidntMatch = false;

            if (childResult.Status == ParseResultStatus.SUCCESS) {
                status = ParseResultStatus.SUCCESS;
                break;
            }
        }

        if (status == ParseResultStatus.ALL_CHILDREN_FAILED && allDidntMatch) status = ParseResultStatus.DIDNT_MATCH;
        return new(this, status, text, children);
    }
}