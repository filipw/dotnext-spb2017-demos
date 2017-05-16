using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;

namespace DiacriticRemover
{

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DiacriticRemoverCodeFixProvider)), Shared]
    public class DiacriticRemoverCodeFixProvider : CodeFixProvider
    {
        private const string title = "Remove diacritics";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiacriticRemoverAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var node = root.FindNode(diagnosticSpan);

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedSolution: c => Rename(context.Document, node, c),
                    equivalenceKey: title),
                diagnostic);
        }

        private async Task<Solution> Rename(Document document, SyntaxNode node, CancellationToken cancellationToken)
        {
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            var symbol = semanticModel.GetDeclaredSymbol(node, cancellationToken);
            var newName = symbol.Name.RemoveGermanDiactrics();

            var newSolution = await Renamer.RenameSymbolAsync(document.Project.Solution, symbol, newName, document.Project.Solution.Workspace.Options, cancellationToken).ConfigureAwait(false);
            return newSolution;
        }


    }
}