using System.Collections.Generic;
using System.Linq;
using Automation.TestFramework.SourceGenerators.ObjectModel;
using Microsoft.CodeAnalysis;

namespace Automation.TestFramework.SourceGenerators;

internal class StepDiscoverer
{
    public IEnumerable<Step> DiscoverSteps(INamedTypeSymbol type)
    {
        foreach (var method in type.GetMembers().OfType<IMethodSymbol>())
        {
            var attribute = method.GetAttributes()
                .FirstOrDefault(attribute => attribute.IsStepAttribute());
    
            if (attribute is null) continue;

            var methodName = method.Name;
    
            var stepAttribute = attribute.ToStepAttribute();
            var description = stepAttribute.Description ?? methodName; // todo humanize?
            yield return new Step(stepAttribute.Type, stepAttribute.Order, description, methodName, method.IsAsync);
        }
    }
}