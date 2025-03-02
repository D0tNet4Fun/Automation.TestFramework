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
            relativeFilePath = filePath.Substring(projectDirectory.Length + 1).Replace(@"\", "/");
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
        var methodSymbol = semanticModel.GetDeclaredSymbol(summaryMethodDeclaration) as IMethodSymbol;
        if (methodSymbol == null)
        {
            // todo
            return string.Empty;
        }

        var steps = DiscoverSteps(methodSymbol);
        OrderSteps(steps);

        var methodBody = GenerateMethodBody(steps);

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

    private static List<Step> DiscoverSteps(IMethodSymbol methodSymbol)
    {
        StepDiscoverer stepDiscoverer = new();
        List<Step> discoveredSteps = [];

        // add the steps from the base types, starting with the last in chain
        var containingType = methodSymbol.ContainingType;
        var baseTypes = containingType.GetBaseTypes();
        foreach (var type in baseTypes.Reverse())
        {
            var baseClassSteps = stepDiscoverer.DiscoverSteps(type);
            discoveredSteps.AddRange(baseClassSteps);
        }

        // add the steps from the current type
        var steps = stepDiscoverer.DiscoverSteps(containingType);
        discoveredSteps.AddRange(steps);

        // return all discovered steps
        return discoveredSteps;
    }

    private void OrderSteps(List<Step> steps)
    {
        steps.Sort((x, y) =>
        {
            // ensure the steps ore ordered first by Type, and then by Order

            if (x.Type != y.Type)
                return x.Type.CompareTo(y.Type);

            return x.Order.CompareTo(y.Order);
        });
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
            string[] addStepMethodArgs =
            [
                $"StepType.{step.Type}",
                $"\"{step.Description}\"",
                step.MethodName
            ];
            builder.AppendLine($"{indent}.{addStepMethodName}({string.Join(", ", addStepMethodArgs)})");
        }

        builder.Append($"{indent};");

        return builder.ToString();
    }
}