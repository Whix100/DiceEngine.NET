using System.Text.RegularExpressions;

namespace DiceEngine.Tokenization.Tokens.Defaults;

internal static class TokenizeFunctions
{
    internal static SymbolToken TokenizeSymbol(char match, int index)
        => new SymbolToken(match, index);

    internal static OpenBracketToken TokenizeOpenBracket(char match, int index)
        => new OpenBracketToken(match switch
        {
            '(' => Bracket.Parenthesis,
            '[' => Bracket.Square,
            '{' => Bracket.Curly,
            _ => throw new NotSupportedException()
        }, index);

    internal static CloseBracketToken TokenizeCloseBracket(char match, int index)
        => new CloseBracketToken(match switch
        {
            ')' => Bracket.Parenthesis,
            ']' => Bracket.Square,
            '}' => Bracket.Curly,
            _ => throw new NotSupportedException()
        }, index);

    internal static WordToken TokenizeWord(Match match, int index)
        => new WordToken(match.Value, index);

    internal static NumberToken TokenizeNumber(Match match, int index)
        => new NumberToken(match.Value, index);
}
