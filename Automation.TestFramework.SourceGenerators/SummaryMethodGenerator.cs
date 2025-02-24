using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Automation.TestFramework.SourceGenerators.ObjectModel;
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

    public string GenerateCode()
    {
        var steps = ExtractStepsFromClassMethods().ToList();
        // todo order steps

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

    private IEnumerable<Step> ExtractStepsFromClassMethods()
    {
        foreach (var methodDeclaration in _classDeclaration.Members.OfType<MethodDeclarationSyntax>())
        {
            if (methodDeclaration == summaryMethodDeclaration) continue;

            var attribute = methodDeclaration.AttributeLists
                .SelectMany(list => list.Attributes)
                .FirstOrDefault(attribute => attribute.IsStepAttribute());

            if (attribute is null) continue;

            var methodName = methodDeclaration.Identifier.Text;

            var stepAttribute = attribute.ToStepAttribute()!;
            var description = stepAttribute.Description ?? methodName; // todo humanize?
            yield return new Step(stepAttribute.Type, stepAttribute.Order, description, methodName, IsAsyncMethod());
            continue;

            bool IsAsyncMethod()
            {
                var returnType = methodDeclaration.ReturnType is GenericNameSyntax genericType
                    ? genericType.Identifier.Text
                    : methodDeclaration.ReturnType.ToString();
                return returnType is "Task" or "ValueTask";
            }
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