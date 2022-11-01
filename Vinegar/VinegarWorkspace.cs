using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using MonoDevelop.Ide.Composition;

namespace Vinegar
{
    public static class ContentTypeNames
    {
        public const string RoslynContentType = "Roslyn Languages";
        public const string VinegarContentType = "text/vinegar";
    }


    public class VinegarWorkspace : Workspace
    {
        public VinegarWorkspace() : base(CompositionManager.Instance.HostServices, WorkspaceKind.Interactive)
        {
        }

        public void CreateDocument(ITextBuffer buffer, string name)
        {
            var projectId = ProjectId.CreateNewId();
            var documentId = DocumentId.CreateNewId(projectId, name);
            var container = buffer.AsTextContainer();

            var projectInfo =
                ProjectInfo.Create(
                    projectId,
                    VersionStamp.Create(),
                    name: name,
                    assemblyName: "vinegar.dll",
                    language: ContentTypeNames.VinegarContentType);

            base.OnProjectAdded(projectInfo);
            var documentInfo =
                DocumentInfo.Create(
                    documentId,
                    name,
                    Array.Empty<string>(),
                    sourceCodeKind: SourceCodeKind.Script,
                    filePath: name,
                    loader: TextLoader.From(buffer.AsTextContainer(), VersionStamp.Create()));

            base.OnDocumentAdded(documentInfo);
            base.OnDocumentOpened(documentId, container);
        }
    }
}

