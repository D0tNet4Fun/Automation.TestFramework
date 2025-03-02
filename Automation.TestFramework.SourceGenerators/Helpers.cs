using System;
using System.Linq;
using Automation.TestFramework.SourceGenerators.ObjectModel;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Automation.TestFramework.SourceGenerators;

public static class Helpers
{
    public static bool IsStepAttribute(this AttributeSyntax attribute)
    {
        var name = attribute.Name.ToString();
        return Enum.GetNames(typeof(StepType)).Any(stepTypeName => stepTypeName == name);
    }

    public static StepAttribute ToStepAttribute(this AttributeSyntax attribute)
    {
        var arguments = attribute.ArgumentList?.Arguments;
        return new StepAttribute(GetType(), GetOrder(), GetDescription());

        StepType GetType()
        {
            var name = attribute.Name.ToString();
            if (Enum.TryParse(name, out StepType type)) return type;

            return StepType.None;
        }

        int GetOrder()
        {
            if (arguments?.Count > 0 &&
                int.TryParse(arguments.Value[0].ToString(), out var order))
            {
                return order;
            }
            
            return StepAttribute.DefaultOrder;
        }

        string? GetDescription()
        {
            return arguments?.Count > 1 ? arguments.Value[1].ToString().Trim('\"') : null;
        }
    }
}