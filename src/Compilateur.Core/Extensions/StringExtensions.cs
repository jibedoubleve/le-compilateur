using System.Globalization;
using Compilateur.Core.Runtime;

namespace Compilateur.Core.Extensions;

internal static class StringExtensions
{
    public static string Stringify(this object? obj) =>
        obj switch
        {
            LoxInstance li => $"{li.Class.Declaration.Token.Lexeme} instance",
            LoxClass cd    => cd.Declaration.Token.Lexeme,
            bool b         => b.ToString().ToLower(),
            null           => "nil",
            double d       => d.ToString(CultureInfo.InvariantCulture),
            _              => obj.ToString() ?? string.Empty
        };
}