using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Automation.TestFramework.SourceGenerators;

/// <summary>
/// Diagnostic suppressor to ensure test methods marked as steps are not considered unused.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class UnusedTestMethodDiagnosticSuppressor : DiagnosticSuppressor
{
    private static readonly SuppressionDescriptor SuppressionDescriptor = new(
        id: "UnusedTestMethodDiagnosticSuppressor",
        suppressedDiagnosticId: "IDE0051",
        justification: "Methods marked as test case steps are not unused, they are invoked from generated code.");

    public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions => [SuppressionDescriptor];

    public override void ReportSuppressions(SuppressionAnalysisContext context)
    {
        // Debugger.Launch();
        
        foreach (var diagnostic in context.ReportedDiagnostics)
        {
            if (ShouldSuppress(diagnostic))
            {
                context.ReportSuppression(Suppression.Create(SuppressionDescriptor, diagnostic));
            }
        }
    }

    private bool ShouldSuppress(Diagnostic diagnostic)
    {
        // suppress the diagnostic if it was raised for a method that is marked as a step
        
        var location = diagnostic.Location;
        var tree = location.SourceTree;
        if (tree is not null)
        {
            var root = tree.GetRoot();
            var methodDeclaration = root.FindNode(location.SourceSpan).FirstAncestorOrSelf<MethodDeclarationSyntax>();
            if (methodDeclaration is not null)
            {
                var hasStepAttribute = methodDeclaration.AttributeLists
                    .SelectMany(attrList => attrList.Attributes)
                    .Any(attribute => attribute.IsStepAttribute());

                return hasStepAttribute;
            }
        }

        return false;
    }
}