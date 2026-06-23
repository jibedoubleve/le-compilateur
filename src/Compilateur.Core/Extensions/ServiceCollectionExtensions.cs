using System.Reflection;
using Compilateur.Core.Lexical;
using Compilateur.Core.Lexical.Rules;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Compilateur.Core.Extensions;

public static class ServiceCollectionExtensions
{
    #region Methods

    private static IServiceCollection AddLexicalRules(this IServiceCollection serviceCollection)
    {
        var ruleType = typeof(ITokenRule);
        var asm = Assembly.GetAssembly(ruleType);
        var types = asm?.GetTypes() ?? [];

        var found = types
                    .Where(t => t is { IsClass: true, IsAbstract: false } && ruleType.IsAssignableFrom(t))
                    .ToList();

        foreach (var type in found)
        {
            serviceCollection.TryAddEnumerable(ServiceDescriptor.Transient(ruleType, type));
        }

        return serviceCollection;
    }

    public static IServiceCollection AddLexicalLayer(this IServiceCollection services)
    {
        services.AddLexicalRules()
                .AddTransient<Scanner>();
        return services;
    }

    #endregion
}