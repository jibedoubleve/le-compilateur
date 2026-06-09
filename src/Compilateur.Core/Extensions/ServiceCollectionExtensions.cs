using System.Reflection;
using Compilateur.Core.Lexer;
using Compilateur.Core.Lexer.Rules;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Compilateur.Core.Extensions;

public static class ServiceCollectionExtensions
{
    #region Methods

    private static IServiceCollection AddLexerRules(this IServiceCollection serviceCollection)
    {
        var asm = Assembly.GetAssembly(typeof(ITokenRule));
        var types = asm?.GetTypes() ?? [];

        var found = types
                    .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(ITokenRule).IsAssignableFrom(t))
                    .ToList();

        foreach (var type in found)
        {
            serviceCollection.TryAddEnumerable(ServiceDescriptor.Transient(typeof(ITokenRule), type));
        }

        return serviceCollection;
    }

    public static IServiceCollection AddLexer(this IServiceCollection services)
    {
        services.AddLexerRules()
                .AddTransient<Scanner>();
        return services;
    }

    #endregion
}