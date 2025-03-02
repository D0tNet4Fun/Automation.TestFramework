using System.Collections.Immutable;
using System.Linq;
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

        // Register additional file provider for project directory
        var additionalFilesProvider = context.AdditionalTextsProvider
            //.Where(file => file.Path.EndsWith(@"\\"))
            .Collect();

        var methodsWithSummaryAttributes = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "Automation.TestFramework.SummaryAttribute",
                (node, _) => node is MethodDeclarationSyntax method && method.Modifiers.Any(SyntaxKind.PartialKeyword),
                (ctx, _) => (MethodDeclarationSyntax)ctx.TargetNode)
            .Collect();

        var combined = additionalFilesProvider
            .Combine(context.CompilationProvider)
            .Combine(methodsWithSummaryAttributes);

        context.RegisterSourceOutput(combined, (spc, source) =>
        {
            var (additionalFiles, compilation, summaryMethods) = (source.Left.Left, source.Left.Right, source.Right);
            Execute(spc, additionalFiles, compilation, summaryMethods);
        });
    }

    private static void Execute(SourceProductionContext sourceProductionContext, 
        ImmutableArray<AdditionalText> additionalFiles, 
        Compilation compilation, 
        ImmutableArray<MethodDeclarationSyntax> summaryMethods)
    {
        var projectDirectory = additionalFiles.FirstOrDefault()?.Path;
        if (projectDirectory == null) return;

        foreach (var method in summaryMethods)
        {
            var generator = new SummaryMethodGenerator(method);
            var fileName = generator.GetFileName(projectDirectory);
            var code = generator.GenerateCode(compilation);
            sourceProductionContext.AddSource(fileName, SourceText.From(code, Encoding.UTF8));
        }
    }
}