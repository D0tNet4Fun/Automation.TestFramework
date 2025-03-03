using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Automation.TestFramework.SourceGenerators;

[Generator]
public class TestCaseSummaryGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //System.Diagnostics.Debugger.Launch();

        // https://andrewlock.net/creating-a-source-generator-part-13-providing-and-accessing-msbuild-settings-in-source-generators/
        var projectDirectoryProvider = context.AnalyzerConfigOptionsProvider
            .Select((options, _) =>
            {
                if (!options.GlobalOptions.TryGetValue("build_property.ProjectDir", out var projectDir)) return null;
                return projectDir;
            });

        var methodsWithSummaryAttributes = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "Automation.TestFramework.SummaryAttribute",
                (node, _) => node is MethodDeclarationSyntax method && method.Modifiers.Any(SyntaxKind.PartialKeyword),
                (ctx, _) => (MethodDeclarationSyntax)ctx.TargetNode)
            .Collect();

        var combined = projectDirectoryProvider
            .Combine(context.CompilationProvider)
            .Combine(methodsWithSummaryAttributes);

        context.RegisterSourceOutput(combined, (spc, source) =>
        {
            var (projectDirectory, compilation, summaryMethods) = (source.Left.Left!, source.Left.Right, source.Right);
            Execute(spc, projectDirectory, compilation, summaryMethods);
        });
    }

    private static void Execute(SourceProductionContext sourceProductionContext, 
        string projectDirectory, 
        Compilation compilation, 
        ImmutableArray<MethodDeclarationSyntax> summaryMethods)
    {
        foreach (var method in summaryMethods)
        {
            var generator = new SummaryMethodGenerator(method);
            var fileName = generator.GetFileName(projectDirectory);
            var code = generator.GenerateCode(compilation);
            sourceProductionContext.AddSource(fileName, SourceText.From(code, Encoding.UTF8));
        }
    }
}