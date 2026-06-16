using System.Reflection;
using Compilateur.Core.Errors;
using Compilateur.Core.Lexical.Rules;
using Compilateur.Core.Syntactic;
using Compilateur.Core.Syntactic.Rules;
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

    private static IServiceCollection AddSyntacticRules(this IServiceCollection serviceCollection)
    {
        var ruleType = typeof(IParser);
        var asm = Assembly.GetAssembly(ruleType);
        var types = asm?.GetTypes() ?? [];

        var found = types
                    .Where(t => t is { IsClass: true, IsAbstract: false } && ruleType.IsAssignableFrom(t))
                    .ToList();

        foreach (var type in found)
        {
            serviceCollection.AddTransient(type);
        }

        return serviceCollection;
    }

    public static IServiceCollection AddLexicalLayer(this IServiceCollection services)
    {
        services.AddLexicalRules()
                .AddTransient<Scanner>();
        return services;
    }

    public static IServiceCollection AddSyntacticLayer(this IServiceCollection services)
    {
        services.AddSyntacticRules()
                .AddTransient<Parser>();
        return services;
    }

    #endregion
}