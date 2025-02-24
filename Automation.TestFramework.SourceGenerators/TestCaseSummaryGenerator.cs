using System.Diagnostics;
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
        //Debugger.Launch();
        
        // Register additional file provider for project directory
        var additionalFilesProvider = context.AdditionalTextsProvider
            //.Where(file => file.Path.EndsWith(@"\\"))
            .Collect();
        
        var methodsWithSummaryAttributes = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "Automation.TestFramework.SummaryAttribute",
                (node, _) => node is MethodDeclarationSyntax method && method.Modifiers.Any(SyntaxKind.PartialKeyword),
                (ctx, _) => (MethodDeclarationSyntax)ctx.TargetNode);
            
        context.RegisterSourceOutput(methodsWithSummaryAttributes.Combine(additionalFilesProvider), (sourceProductionContext, combined) =>
        {
            var (method, additionalFiles) = combined;
            var projectDirectory = additionalFiles.FirstOrDefault()?.Path;
            if (projectDirectory == null) return;
            
            var generator = new SummaryMethodGenerator(method);
            var fileName = generator.GetFileName(projectDirectory);
            var source = generator.GenerateCode();
            sourceProductionContext.AddSource(fileName, SourceText.From(source, Encoding.UTF8));
        });
    }
}