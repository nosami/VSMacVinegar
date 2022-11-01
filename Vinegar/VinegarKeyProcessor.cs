using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Documents;
using Vim;
using ExportAttribute = System.ComponentModel.Composition.ExportAttribute;

namespace Vinegar
{
    [Export(typeof(IVimBufferCreationListener))]
    class VinegarBufferCreationListener : IVimBufferCreationListener
    {
        private readonly IContentTypeRegistryService _contentTypeRegistryService;
        private readonly VinegarWorkspace _workspace;

        [ImportingConstructor]
        public VinegarBufferCreationListener(IContentTypeRegistryService contentTypeRegistryService)
        {
            _contentTypeRegistryService = contentTypeRegistryService;
            _workspace = new VinegarWorkspace();
        }

        void IVimBufferCreationListener.VimBufferCreated(IVimBuffer vimBuffer)
        {
            bool isVinegarView = vimBuffer.Name.EndsWith("vinegar");
            if (isVinegarView)
            {
                vimBuffer.LocalSettings.Number = false;
                vimBuffer.LocalSettings.RelativeNumber = false;
                var path = new FilePath(vimBuffer.Name).ParentDirectory;

                var contentType = _contentTypeRegistryService.GetContentType(ContentTypeNames.VinegarContentType);
                var textBuffer = vimBuffer.TextView.TextBuffer;
                textBuffer.ChangeContentType(contentType, null);
                _workspace.CreateDocument(textBuffer, vimBuffer.Name);
                
                VinegarKeyProcessor.SetBufferText(path, vimBuffer.TextView);
            }
        }
    }

    class VinegarKeyProcessor : KeyProcessor 
    {
        private readonly IVimBuffer _vimBuffer;

        public override bool IsInterestedInHandledEvents => true;

        public VinegarKeyProcessor(IVimBuffer vimBuffer)
        {
            _vimBuffer = vimBuffer;
        }

        public override void KeyDown(KeyEventArgs e)
        {
            e.Handled = false;
            if (_vimBuffer.Name.EndsWith("vinegar") && _vimBuffer.ModeKind == ModeKind.Normal)
            {
                if (e.Characters == "-")
                {
                    e.Handled = true;
                    HyphenPress();
                }
                else if ((NSKey)e.Event.KeyCode == NSKey.Return)
                {
                    e.Handled = true;
                    OpenFileOrFolder();
                }
            }
            else if (_vimBuffer.Mode.ModeKind == ModeKind.Normal)
            {
                if (e.Characters == "-")
                {
                    e.Handled = true;
                    HyphenPress();
                }
            }
        }

        public void OpenFileOrFolder()
        {
            var doc = IdeServices.DocumentManager.ActiveDocument;
            var textView = doc.GetContent<ITextView>();
            var buffer = textView.Properties[typeof(VinegarBuffer)] as VinegarBuffer;
            var line = textView.Caret.ContainingTextViewLine.Start.GetContainingLineNumber();
            VinegarOutput? obj = buffer?.Lines[line];
            if (obj is FileLocation)
            {
                IdeServices.DocumentManager.OpenDocument(new FileOpenInformation(obj.Location));
            }
            else if (obj is DirectoryLocation)
            {
                ShowPath(obj.Location, false, 0);
            }
        }
        
        void ShowPath(FilePath path, bool newView, int notebookIndex)
        {
            if (newView)
            {
                using var stream = new MemoryStream();
                var output = string.Empty;
                stream.Write(System.Text.Encoding.UTF8.GetBytes(output));
                stream.Position = 0;
                FilePath filePath = path.Combine("/", notebookIndex + ".vinegar");
                // Create the file descriptor to be loaded in the editor
                var descriptor = new FileDescriptor(filePath, ContentTypeNames.VinegarContentType, stream, null);
                descriptor.OpenAsReadOnly = true;
                var doc = IdeServices.DocumentManager.OpenDocument(descriptor);
                
                // Buffer text is set when the ITextView materializes
            }
            else
            {
                var doc = IdeServices.DocumentManager.ActiveDocument;
                var textView = doc.GetContent<ITextView>();
                SetBufferText(path, textView);
            }
        }

        public static void SetBufferText(FilePath path, ITextView textView)
        {
            var buffer = new VinegarBuffer(path);
            var textBuffer = textView.TextBuffer;
            textView.Properties[typeof(VinegarBuffer)] = buffer;
            using var edit = textBuffer.CreateEdit();
            edit.Replace(new Microsoft.VisualStudio.Text.Span(0, textBuffer.CurrentSnapshot.Length), buffer.Build());
            edit.Apply();
            IdeServices.DocumentManager.ActiveDocument.IsDirty = false;
        }

        void HyphenPress()
        {
            FilePath path;
            var doc = IdeServices.DocumentManager.ActiveDocument;
            var textView = doc.GetContent<ITextView>();
            bool isVinegarView = textView.Properties.ContainsProperty(typeof(VinegarBuffer));
            var notebookIndex = WindowManagement.GetNotebookIndex(textView);
            // Either in a standard document switching to a vinegar view
            // or in a vinegar view navigating up
            if (isVinegarView)
            {
                var oldBuffer = (VinegarBuffer)textView.Properties[typeof(VinegarBuffer)];
                path = oldBuffer.FilePath.ParentDirectory ;
                if (path.IsNull)
                    return;
            }
            else
            { 
                path = IdeApp.Workbench.ActiveDocument.FilePath.ParentDirectory;
                
            }
            ShowPath(path, !isVinegarView, notebookIndex);
        }
   }
}
