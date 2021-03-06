using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DiacriticRemover
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DiacriticRemoverAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "RS001";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Naming";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.VariableDeclarator, SyntaxKind.Parameter, SyntaxKind.FieldDeclaration, 
                SyntaxKind.EnumDeclaration, SyntaxKind.EnumMemberDeclaration, SyntaxKind.PropertyDeclaration, SyntaxKind.MethodDeclaration, 
                SyntaxKind.ClassDeclaration, SyntaxKind.InterfaceDeclaration);
       }

        private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var token = context.Node.DescendantTokens().FirstOrDefault(x => x.Kind() == SyntaxKind.IdentifierToken);
            if (token == null) return;
            if (!token.Text.ContainsGermanDiactrics()) return;

            var diagnostic = Diagnostic.Create(Rule, token.GetLocation(), token.Text);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
