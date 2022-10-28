using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;
//using Microsoft.VisualStudio.Utilities;

namespace Vinegar
{

    [ExportLanguageService(typeof(IClassificationService), "vinegar", layer: ServiceLayer.Host)]
	public class VinegarClassificationService: IClassificationService
	{
        public VinegarClassificationService()
        {
            string s = null;
        }
        private readonly ObjectPool<List<ClassifiedSpan>> s_listPool = new(() => new());

        public ClassifiedSpan AdjustStaleClassification(SourceText text, ClassifiedSpan classifiedSpan)
        {
            throw new NotImplementedException();
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
            
            string s = null;
        }

        Task IClassificationService.AddSyntacticClassificationsAsync(Document document, TextSpan textSpan, ArrayBuilder<ClassifiedSpan> result, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
