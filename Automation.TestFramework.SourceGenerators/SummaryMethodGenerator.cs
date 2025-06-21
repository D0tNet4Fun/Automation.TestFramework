using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Automation.TestFramework.SourceGenerators.ObjectModel;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Automation.TestFramework.SourceGenerators;

internal class SummaryMethodGenerator(MethodDeclarationSyntax summaryMethodDeclaration)
{
    private readonly ClassDeclarationSyntax _classDeclaration = (ClassDeclarationSyntax)summaryMethodDeclaration.Parent!;

    public string GetFileName(string projectDirectory)
    {
        var filePath = summaryMethodDeclaration.SyntaxTree.FilePath;

        string relativeFilePath;
        if (filePath.StartsWith(projectDirectory))
        {
            relativeFilePath = filePath.Substring(projectDirectory.Length).Replace(@"\", "/");
        }
        else
        {
            relativeFilePath = Path.GetFileName(filePath);
        }

        return Path.ChangeExtension(relativeFilePath, ".g.cs");
    }

    public string GenerateCode(Compilation compilation)
    {
        var semanticModel = compilation.GetSemanticModel(summaryMethodDeclaration.SyntaxTree);
        if (semanticModel.GetDeclaredSymbol(summaryMethodDeclaration) is not IMethodSymbol methodSymbol)
        {
            throw new InvalidOperationException("Summary is not a method");
        }

        var steps = DiscoverSteps(methodSymbol);
        var orderedSteps = OrderSteps(steps);

        var methodBody = GenerateMethodBody(orderedSteps);

        var namespaceDeclaration = _classDeclaration.Parent switch
        {
            NamespaceDeclarationSyntax classicNamespaceDeclaration => classicNamespaceDeclaration.Name.ToString(),
            FileScopedNamespaceDeclarationSyntax fileScopedNamespaceDeclaration => fileScopedNamespaceDeclaration.Name.ToString(),
            _ => throw new InvalidOperationException("Namespace declaration is missing.")
        };

        var methodModifiers = string.Join(" ", summaryMethodDeclaration.Modifiers.Select(m => m.Text));
        var methodReturnType = summaryMethodDeclaration.ReturnType.ToString();
        var methodName = summaryMethodDeclaration.Identifier.Text;

        return
            $$"""
              using Automation.TestFramework;

              namespace {{namespaceDeclaration}};

              partial class {{_classDeclaration.Identifier.ToString()}}
              {
                  {{methodModifiers}} {{methodReturnType}} {{methodName}}()
                  {
                      {{methodBody}}
                  }
              }
              """;
    }

    private static SortedList<int, IReadOnlyCollection<Step>> DiscoverSteps(IMethodSymbol methodSymbol)
    {
        StepDiscoverer stepDiscoverer = new();
        SortedList<int, IReadOnlyCollection<Step>> discoveredSteps = [];

        // add the steps from the base types, starting with the last in chain
        var containingType = methodSymbol.ContainingType;
        var baseTypes = containingType.GetBaseTypes();
        var level = 0;
        foreach (var type in baseTypes.Reverse())
        {
            var baseClassSteps = stepDiscoverer.DiscoverSteps(type).ToArray();
            if (baseClassSteps.Length == 0) continue;

            discoveredSteps[level++] = baseClassSteps;
        }

        // add the steps from the current type
        var steps = stepDiscoverer.DiscoverSteps(containingType).ToArray();
        discoveredSteps[level] = steps;

        // return all discovered steps
        return discoveredSteps;
    }

    private IReadOnlyCollection<Step> OrderSteps(SortedList<int, IReadOnlyCollection<Step>> stepsByClassHierarchy)
    {
        var comparer = new StepComparer();
        
        // order the steps for each class in the hierarchy
        var orderedStepsByClassHierarchy = new SortedList<int, IReadOnlyCollection<Step>>();
        foreach (var unorderedSteps in stepsByClassHierarchy)
        {
            orderedStepsByClassHierarchy.Add(unorderedSteps.Key, OrderStepsFromSameClass(unorderedSteps.Value));
        }
        if (orderedStepsByClassHierarchy.Count == 1)
        {
            // there is no class hierarchy
            return orderedStepsByClassHierarchy.First().Value;
        }

        // else we have a class hierarchy such as TestClass : Derived : Base
        // setups: start from Base ... TestClass (same as constructor order)
        // .. rest of the steps
        // cleanups: start from TestClass ... base (same as Dispose order)
        List<Step> orderedSteps = [];
        var depth = orderedStepsByClassHierarchy.Count - 2; // only base classes 
        for (var i = 0; i <= depth; i++)
        {
            var steps = orderedStepsByClassHierarchy[i];
            orderedSteps.AddRange(steps.Where(s => s.Type == StepType.Setup));
        }
        orderedSteps.AddRange(orderedStepsByClassHierarchy.Last().Value);
        for (var i = depth; i >= 0; i--)
        {
            var steps = orderedStepsByClassHierarchy[i];
            orderedSteps.AddRange(steps.Where(s => s.Type == StepType.Cleanup));
        }
        return orderedSteps;

        IReadOnlyCollection<Step> OrderStepsFromSameClass(IReadOnlyCollection<Step> steps)
        {
            return steps.OrderBy(x => x, comparer).ToList();
        }
    }

    private string GenerateMethodBody(IReadOnlyCollection<Step> steps)
    {
        var builder = new StringBuilder()
            .AppendLine($"// generated using {typeof(TestCaseSummaryGenerator).FullName}")
            .AppendLine();

        var indent = new string(' ', 4 * 2);
        if (steps.Count == 0)
        {
            builder.Append($"{indent}// there are no steps defined in this class");
            return builder.ToString();
        }

        // sample:
        // TestCase.Current.Descriptor
        //     .AddStep(StepType.Input, "This is the input", Input)
        //     .AddAsyncStep(StepType.ExpectedResult, "This is the expected result", ExpectedResult);
        builder.AppendLine($"{indent}TestCase.Current.Descriptor");
        indent = new string(' ', 4 * 3);
        foreach (var step in steps)
        {
            var addStepMethodName = step.IsAsync ? "AddAsyncStep" : "AddStep";
            var stepDescription = step.Description.EscapeString();

            string[] addStepMethodArgs =
            [
                $"StepType.{step.Type}",
                stepDescription,
                step.MethodName
            ];
            builder.AppendLine($"{indent}.{addStepMethodName}({string.Join(", ", addStepMethodArgs)})");
        }

        builder.Append($"{indent};");

        return builder.ToString();
    }
}