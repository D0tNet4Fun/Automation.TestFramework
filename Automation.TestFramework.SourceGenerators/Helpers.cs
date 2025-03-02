using System;
using System.Collections.Generic;
using System.Linq;
using Automation.TestFramework.SourceGenerators.ObjectModel;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Automation.TestFramework.SourceGenerators;

internal static class Helpers
{
    public static bool IsStepAttribute(this AttributeSyntax attribute)
    {
        var name = attribute.Name.ToString();
        return Enum.GetNames(typeof(StepType)).Any(stepTypeName => stepTypeName == name);
    }

    public static bool IsStepAttribute(this AttributeData attribute)
    {
        var name = attribute.AttributeClass!.Name;
        return Enum.GetNames(typeof(StepType)).Any(stepTypeName => stepTypeName + "Attribute" == name);
    }

    public static StepAttribute ToStepAttribute(this AttributeData attribute)
    {
        var arguments = attribute.ConstructorArguments;
        return new StepAttribute(GetType(), GetOrder(), GetDescription());

        StepType GetType()
        {
            var name = attribute.AttributeClass!.Name.Replace("Attribute", string.Empty);
            if (Enum.TryParse(name, out StepType type)) return type;

            return StepType.None;
        }

        int GetOrder()
        {
            if (arguments.Length > 0 &&
                int.TryParse(arguments[0].ToString(), out var order))
            {
                return order;
            }
            
            return StepAttribute.DefaultOrder;
        }

        string? GetDescription()
        {
            return arguments.Length > 1 ? arguments[1].Value?.ToString().Trim('\"') : null;
        }
    }

    public static IReadOnlyCollection<INamedTypeSymbol> GetBaseTypes(this INamedTypeSymbol type)
    {
        List<INamedTypeSymbol> baseTypes = [];
        var baseType = type.BaseType;
        while (baseType is not null)
        {
            baseTypes.Add(baseType);
            baseType = baseType.BaseType;
        }

        return baseTypes;
    }
}
