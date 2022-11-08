using System.ComponentModel.Composition;
using Microsoft.Toolkit.HighPerformance.Buffers;
using Microsoft.VisualStudio.Text;
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

                var contentType = _contentTypeRegistryService.GetContentType(ContentTypeNames.VinegarContentType);
                var textBuffer = vimBuffer.TextView.TextBuffer;
                textBuffer.ChangeContentType(contentType, null);
                _workspace.CreateDocument(textBuffer, vimBuffer.Name);
                VinegarKeyProcessor.SetBufferText(VinegarKeyProcessor.Path, VinegarKeyProcessor.From, vimBuffer.TextView);
            }
        }
    }

    class VinegarKeyProcessor : KeyProcessor 
    {
        private readonly IVimBuffer _vimBuffer;

        public static FilePath From { get; private set; }
        public static FilePath Path { get; private set; }

        public override bool IsInterestedInHandledEvents => true;

        FilePath tempPath = System.IO.Path.GetTempPath();
        public VinegarKeyProcessor(IVimBuffer vimBuffer)
        {
            _vimBuffer = vimBuffer;
        }

        public override void KeyDown(KeyEventArgs e)
        {
            e.Handled = false;
            if (_vimBuffer.Name.EndsWith("vinegar") && _vimBuffer.ModeKind == ModeKind.Normal)
            {
                var search = _vimBuffer.IncrementalSearch;
                if (search.HasActiveSession)
                    return;

                if (e.Characters == "-")
                {
                    e.Handled = true;
                    HyphenPress();
                }
                // commented out because of a bug with installing macos workload
                //else if ((NSKey)e.Event.KeyCode == NSKey.Return)
                else if (e.Event.KeyCode == 36)
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
                if(buffer != null)
                { 
                    ShowPath(obj.Location, buffer.FilePath, false, 0);
                }
            }
        }
        
        void ShowPath(FilePath path, FilePath from, bool newView, int notebookIndex)
        {
            if (newView)
            {
                FilePath filePath = tempPath.Combine(notebookIndex + ".vinegar");
                Path = path;
                From = from;
                Runtime.RunInMainThread(async delegate
                {
                    using (var stream = new MemoryStream())
                    {
                        var descriptor = new FileDescriptor(filePath, ContentTypeNames.VinegarContentType, stream, null);
                        descriptor.OpenAsReadOnly = true;
                        var doc = await IdeServices.DocumentManager.OpenDocument(descriptor);

                        var textView = doc.GetContent<ITextView>();
                        if (textView != null)
                            SetBufferText(path, from, textView);
                        // else Buffer text is set when the ITextView materializes
                    }
                });
            }
            else
            {
                var doc = IdeServices.DocumentManager.ActiveDocument;
                var textView = doc.GetContent<ITextView>();
                SetBufferText(path, from, textView);
            }
        }

        public static void SetBufferText(FilePath path, FilePath from, ITextView textView)
        {
            var vinegarBuffer = new VinegarBuffer(path);
            string output = vinegarBuffer.Build();
            var textBuffer = textView.TextBuffer;
            textView.Properties[typeof(VinegarBuffer)] = vinegarBuffer;
            using (var edit = textBuffer.CreateEdit())
            {
                edit.Replace(new Span(0, textBuffer.CurrentSnapshot.Length), output);
                edit.Apply();
            }

            bool found = false;
            foreach (var line in textView.TextBuffer.CurrentSnapshot.Lines)
            {
                var lineText = line.GetText();
                if (lineText == from.FileName)
                {
                    found = true;
                    Task.Run(async delegate
                    {
                        var point = new SnapshotPoint(line.Snapshot, line.Start.Position);
                        var foundTextViewLine = false;
                        while (!foundTextViewLine)
                        {
                            await Task.Delay(20);
                            await Runtime.RunInMainThread(() => {
                                if (textView.TryGetTextViewLineContainingBufferPosition(point, out var textViewLine))
                                {
                                    textView.Caret.MoveTo(textViewLine);
                                    foundTextViewLine = true;
                                }
                            });
                        }
                    });
                    break;
                }    
            }
            if (!found)
            {
                var point = new SnapshotPoint(textView.TextBuffer.CurrentSnapshot, 0);
                textView.Caret.MoveTo(point);
            }
            IdeServices.DocumentManager.ActiveDocument.IsDirty = false;
        }

        void HyphenPress()
        {
            FilePath path;
            FilePath from;
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
                from = oldBuffer.FilePath;
                if (path.IsNull)
                    return;
            }
            else
            {
                from = IdeApp.Workbench.ActiveDocument.FilePath;
                path = from.ParentDirectory;
                
            }
            ShowPath(path, from, !isVinegarView, notebookIndex);
        }
   }
}
