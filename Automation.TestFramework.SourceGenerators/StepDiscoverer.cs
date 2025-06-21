using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Automation.TestFramework.SourceGenerators.ObjectModel;
using Humanizer;
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
            var description = stepAttribute.Description ?? methodName.Humanize();
            yield return new Step(stepAttribute.Type, stepAttribute.Order, description, methodName, IsAsyncMethod(method));
        }

        static bool IsAsyncMethod(IMethodSymbol method)
        {
            // check if the method has the async modifier
            if (method.IsAsync) return true;
            // check if the method return type is either Task or ValueTask
            if (method.ReturnType.Name is nameof(Task) or nameof(ValueTask)) return true;

            return false;
        }
    }
}