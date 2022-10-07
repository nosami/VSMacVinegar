using Microsoft.VisualStudio.Text.Editor;
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
        void IVimBufferCreationListener.VimBufferCreated(IVimBuffer vimBuffer)
        {
            bool isVinegarView = vimBuffer.Name.EndsWith("vinegar");
            if (isVinegarView)
            {
                vimBuffer.LocalSettings.Number = false;
                vimBuffer.LocalSettings.RelativeNumber = false;
                var path = new FilePath(vimBuffer.Name).ParentDirectory;
                VinegarKeyProcessor.SetBufferText(path, vimBuffer, vimBuffer.TextView);
            }
        }
    }

    class VinegarKeyProcessor : KeyProcessor 
    {
        private readonly IVimBuffer _vimBuffer;

        public override bool IsInterestedInHandledEvents => true;
        public const string VinegarBufferName = "/vinegar";

        public VinegarKeyProcessor(IVimBuffer vimBuffer)
        {
            _vimBuffer = vimBuffer;
        }

        public override void KeyDown(KeyEventArgs e)
        {
            NSKey key = (NSKey)e.Event.KeyCode;
            if (_vimBuffer.Name.EndsWith("vinegar") && _vimBuffer.Mode.ModeKind == Vim.ModeKind.Normal)
            {
                e.Handled = true;
                if (e.Characters == "-")
                {
                    HyphenPress();
                }
                    else if (key == NSKey.Return)
                    {
                    OpenFileOrFolder();
                }
            }
            else if (_vimBuffer.Mode.ModeKind == Vim.ModeKind.Normal)
            {
                if (e.Characters == "-")
                {
                    e.Handled = true;
                    HyphenPress();
                }
            }
            else
            {
                e.Handled = false;
            }
        }

        public void OpenFileOrFolder()
        {
            var doc = IdeServices.DocumentManager.ActiveDocument;
            var textView = doc.GetContent<ITextView>();
            var buffer = textView.Properties[typeof(VinegarBuffer)] as VinegarBuffer;
            var line = textView.Caret.ContainingTextViewLine.Start.GetContainingLineNumber();
            VinegarOutput? obj = buffer?.Lines.Values[line - 1];
            if (obj is FileLocation)
            {
                IdeServices.DocumentManager.OpenDocument(new FileOpenInformation(obj.Location));
            }
            else if (obj is DirectoryLocation)
            {
                ShowPath(obj.Location, false);
            }
        }
        
        void ShowPath(FilePath path, bool newView)
        {
            if (newView)
            {
                using var stream = new MemoryStream();
                var output = string.Empty;
                stream.Write(System.Text.Encoding.UTF8.GetBytes(output));
                stream.Position = 0;
                FilePath filePath = path.Combine("vinegar");
                // Create the file descriptor to be loaded in the editor
                var descriptor = new FileDescriptor(filePath, "text/vinegar", stream, null);

                var doc = IdeServices.DocumentManager.OpenDocument(descriptor);
                // Buffer text is set when the ITextView materializes
            }
            else
            {
                var doc = IdeServices.DocumentManager.ActiveDocument;
                var textView = doc.GetContent<ITextView>();
                SetBufferText(path, _vimBuffer, textView);
            }
        }



        public static void SetBufferText(FilePath path, IVimBuffer vimBuffer, ITextView textView)
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
            ShowPath(path, !isVinegarView);
        }
    }
}
