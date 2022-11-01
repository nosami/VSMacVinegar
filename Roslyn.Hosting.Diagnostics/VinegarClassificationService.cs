using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text.Editor;
using MonoDevelop.Ide;

namespace Vinegar
{
    [ExportLanguageService(typeof(IClassificationService), "text/vinegar", layer: ServiceLayer.Host)]
	public class VinegarClassificationService: IClassificationService
	{
        private readonly ObjectPool<List<ClassifiedSpan>> s_listPool = new(() => new());

        public ClassifiedSpan AdjustStaleClassification(SourceText text, ClassifiedSpan classifiedSpan)
        {
            return classifiedSpan;
        }

        public TextChangeRange? ComputeSyntacticChangeRange(SolutionServices workspace, SyntaxNode oldRoot, SyntaxNode newRoot, TimeSpan timeout, CancellationToken cancellationToken)
        {
            return null;
        }

        public ValueTask<TextChangeRange?> ComputeSyntacticChangeRangeAsync(Document oldDocument, Document newDocument, TimeSpan timeout, CancellationToken cancellationToken)
        {
            return new();
        }

        Task IClassificationService.AddEmbeddedLanguageClassificationsAsync(Document document, TextSpan textSpan, ClassificationOptions options, ArrayBuilder<ClassifiedSpan> result, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        void IClassificationService.AddLexicalClassifications(SourceText text, TextSpan textSpan, ArrayBuilder<ClassifiedSpan> result, CancellationToken cancellationToken)
        {
            using var _ = s_listPool.GetPooledObject(out var list);
            if (text.Lines.GetLineFromPosition(textSpan.Start).LineNumber % 2 == 0)
            {
                result.Add(new ClassifiedSpan(textSpan, "class"));
            }
            else
            {
                result.Add(new ClassifiedSpan(textSpan, "text"));
            }
        }

        Task IClassificationService.AddSemanticClassificationsAsync(Document document, TextSpan textSpan, ClassificationOptions options, ArrayBuilder<ClassifiedSpan> result, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        void IClassificationService.AddSyntacticClassifications(SolutionServices services, SyntaxNode root, TextSpan textSpan, ArrayBuilder<ClassifiedSpan> result, CancellationToken cancellationToken)
        {
            
        }

        Task IClassificationService.AddSyntacticClassificationsAsync(Document document, TextSpan textSpan, ArrayBuilder<ClassifiedSpan> result, CancellationToken cancellationToken)
        {
            var doc = IdeApp.Workbench.Documents.FirstOrDefault(d => d.FilePath == document.FilePath);
            var textView = doc.GetContent<ITextView>();
            var vinegarBuffer = (VinegarBuffer)textView.Properties[typeof(VinegarBuffer)];

            var span = new Microsoft.VisualStudio.Text.Span(textSpan.Start, textSpan.Length);
            foreach (var line in textView.TextSnapshot.Lines)
            {
                var str = line.GetText();
                if (!line.Extent.IntersectsWith(span))
                    continue;

                var lineNumber = line.Start.GetContainingLineNumber();
                if (lineNumber >= vinegarBuffer.Lines.Count)
                    continue;

                var bufferLine = vinegarBuffer.Lines[lineNumber];

                if (bufferLine is OriginalLocation)
                {
                    result.Add(new ClassifiedSpan("class name", new TextSpan(line.Start, line.Length)));
                }
                else if (bufferLine is DirectoryLocation)
                {
                    result.Add(new ClassifiedSpan("method name", new TextSpan(line.Start, line.Length)));
                }
            }
            return Task.CompletedTask;
        }
    }
}
