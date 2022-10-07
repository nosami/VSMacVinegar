using System.Text;
using MonoDevelop.Core;

namespace SamplesExtension.DocumentsandViews.UrlDocumentView
{
    // The ExportDocumentControllerFactory attribute is used here to set register this class as a document controller factory.
    // In this case the file scope properties like FileExtension or MimeType are not set since this document controller
    // is not going to handle a file.

    //[ExportDocumentControllerFactory]
    //class VinegarDocumentViewFactory : DocumentControllerFactory
    //{
    //    protected override IEnumerable<DocumentControllerDescription> GetSupportedControllers(ModelDescriptor modelDescriptor)
    //    {
    //        // This factory knows how to create document controllers for UrlDescriptor.
    //        // Return a DocumentControllerDescription that describes what the document is going to contain.

    //        if (modelDescriptor is VinegarDescriptor)
    //            yield return new DocumentControllerDescription("Url");
    //    }

    //    public override Task<DocumentController> CreateController(ModelDescriptor modelDescriptor, DocumentControllerDescription controllerDescription)
    //    {
    //        // This method is called when GetSupportedControllers returned a DocumentControllerDescription, which is provided as argument
    //        return Task.FromResult<DocumentController>(new CustomEditorDocumentController((VinegarDescriptor)modelDescriptor));
    //    }
    //}

    ///// <summary>
    ///// Our sample model descriptor.
    ///// </summary>
    //public class VinegarDescriptor : ModelDescriptor
    //{
    //    public string? Url { get; set; }
    //}

    /// <summary>
    /// The sample view
    /// </summary>
    //public class UrlDocumentController : DocumentController
    //{
    //    VinegarDescriptor descriptor;
    //    string? text;

    //    public UrlDocumentController(VinegarDescriptor descriptor)
    //    {
    //        this.descriptor = descriptor;
    //        DocumentTitle = "Url";
    //    }

    //    protected override bool OnTryReuseDocument(ModelDescriptor modelDescriptor)
    //    {
    //        // This method is overriden to avoid opening a new document for a url if one is already open.
    //        // It returns True if the descriptor provided matches the one that this controller is using.
    //        return modelDescriptor is VinegarDescriptor urlDescriptor && urlDescriptor.Url == descriptor.Url;
    //    }

    //    protected override async Task OnLoad(bool reloading)
    //    {
    //        // Get the content from the site
    //        HttpClient client = new HttpClient();
    //        text = await client.GetStringAsync(descriptor.Url);
    //    }

    //    protected override Control OnGetViewControl(DocumentViewContent view)
    //    {
    //        var label = new AppKit.NSTextView();
    //        label.Value = text ?? "";
    //        return label;
    //    }
    //}

    // Implement the test command that will open the new view.
    // The command needs to be declared in an add-in manifest.
    // Check the file "ExtensionModel.addin.xml" that is a child of this file
    // to see how the command is declared.

    //public enum Commands
    //{
    //    OpenUrlTest,
    //    OpenCustomEditor
    //}

    //class OpenFileOrFolderCommandHandler : VinegarBaseHandler
    //{
    //    //protected override void Run()
    //    //{
    //    //    IdeServices.DocumentManager.OpenDocument(new UrlDescriptor { Url = "http://microsoft.com" });
    //    //static CustomEditorDocumentController _controller;
    //    protected override async void Run()
    //    {
    //        var doc = IdeServices.DocumentManager.ActiveDocument;
    //        var textView = doc.GetContent<ITextView>();
    //        if (!textView.Properties.ContainsProperty(typeof(BufferFromDocument)))
    //        {
    //            IVimBuffer?.Process(KeyInputUtil.EnterKey);
    //            return;
    //        }


    //        var buffer = textView.Properties[typeof(BufferFromDocument)] as BufferFromDocument;
    //        var line = textView.Caret.ContainingTextViewLine.Start.GetContainingLineNumber();
    //        VinegarOutput? obj = buffer?.Lines.Values[line-1];
    //        if(obj is FileLocation)
    //        {
    //            await IdeServices.DocumentManager.OpenDocument(new FileOpenInformation(obj.Location));
    //        }
    //        else if(obj is DirectoryLocation)
    //        {
    //            await ShowPath(obj.Location, true);
    //        }
    //    }
    //}

    //[Export(typeof(IVimBufferCreationListener))]
    //class VinegarBaseHandler : CommandHandler, IVimBufferCreationListener
    //{ 
    //    //public static FilePath _path = null;
    //    const string VinegarBufferName = "/vinegar";

    //    public IVimBuffer? IVimBuffer { get; private set; }

    //    protected async Task ShowPath(FilePath path, bool reuseView)
    //    {
    //        if (!reuseView)
    //        {

    //            using var stream = new MemoryStream();
    //            var output = string.Empty;// buffer.Build();
    //            stream.Write(System.Text.Encoding.UTF8.GetBytes(output));
    //            stream.Position = 0;
    //            var fileName = Path.Combine(path, "vinegar");
    //            // Create the file descriptor to be loaded in the editor
    //            var descriptor = new FileDescriptor(fileName, "text/vinegar", stream, null);//, stream, null);
    //                                                                                                 // Show the controller in the shell
    //            var doc = await IdeServices.DocumentManager.OpenDocument(descriptor);
    //            // Buffer text is set when the ITextView materializes
    //        }
    //        else
    //        {
    //            var doc = IdeServices.DocumentManager.ActiveDocument;
    //            var textView = doc.GetContent<ITextView>();
    //            SetBufferText(textView);
    //        }
    //    }


    //    void IVimBufferCreationListener.VimBufferCreated(IVimBuffer vimBuffer)
    //    {
    //        IVimBuffer = vimBuffer;
    //        bool isVinegarView = vimBuffer.Name == VinegarBufferName;
    //        if (isVinegarView)
    //        {
    //            vimBuffer.LocalSettings.Number = false;
    //            vimBuffer.LocalSettings.RelativeNumber = false;
    //            SetBufferText(vimBuffer.TextView);
    //        }
    //    }

    //    void SetBufferText(ITextView textView)
    //    {
    //        var path = new FileInfo(IVimBuffer.Name).DirectoryName;
    //        var buffer = new BufferFromDocument(path);
    //        var textBuffer = textView.TextBuffer;
    //        textView.Properties[typeof(BufferFromDocument)] = buffer;
    //        using var edit = textBuffer.CreateEdit();
    //        edit.Replace(new Microsoft.VisualStudio.Text.Span(0, textBuffer.CurrentSnapshot.Length), buffer.Build());
    //        edit.Apply();
    //        IdeServices.DocumentManager.ActiveDocument.IsDirty = false;
    //    }
    //}

    //class OpenUrlTestCommandHandler : VinegarBaseHandler
    //{
    //    protected override async void Run()
    //    {
    //        var _path = new FilePath(IVimBuffer.Name).ParentDirectory;
    //        var doc = IdeServices.DocumentManager.ActiveDocument;
    //        var textView = doc.GetContent<ITextView>();
    //        bool isVinegarView = textView.Properties.ContainsProperty(typeof(BufferFromDocument));

    //        if (_path == null)
    //        {
    //            _path = IdeApp.Workbench.ActiveDocument.FilePath.ParentDirectory;
    //        }
    //        else if (_path.ParentDirectory.IsNull && isVinegarView) 
    //        {
    //            return;
    //        }
    //        else if (_path.IsDirectory && isVinegarView)
    //        {
    //            _path = _path.ParentDirectory;
    //        }
    //        else
    //        {
    //            _path = IdeApp.Workbench.ActiveDocument.FilePath.ParentDirectory;
    //        }
    //        await ShowPath(_path, isVinegarView);
    //    }
    //    //protected override void Run()
    //    //{
    //    //    IdeServices.DocumentManager.OpenDocument(new UrlDescriptor { Url = "http://microsoft.com" });
    //    //static CustomEditorDocumentController _controller;
    //}

    abstract class VinegarOutput
    {
        public VinegarOutput(FilePath location)
        {
            Location = location;
        }

        public FilePath Location { get; }

        public override string ToString()
        {
            return Location.FileName;
        }
    }

    class OriginalLocation : VinegarOutput
    {
        public OriginalLocation(FilePath location) : base(location)
        {
        }

        public override string ToString()
        {
            return Location.FullPath;
        }
    }

    class FileLocation : VinegarOutput
    {
        public FileLocation(FilePath location) : base(location)
        {
        }
    }

    class DirectoryLocation : VinegarOutput
    {
        public DirectoryLocation(FilePath location) : base(location)
        {
        }
    }

    class BufferFromDocument
    {
        private readonly FilePath _filePath;

        private SortedList<string, VinegarOutput> _lines = new SortedList<string, VinegarOutput>();
        public BufferFromDocument(FilePath filePath)
        {
            _filePath = filePath;
        }

        public FilePath FilePath => _filePath;
        internal SortedList<string, VinegarOutput> Lines { get => _lines; set => _lines = value; }

        public string Build()
        {
            AddDirectory(_filePath);
            var sb = new StringBuilder();
            var location = new OriginalLocation(_filePath);
            sb.AppendLine($"{location}:");
            foreach(var output in _lines)
            {
                sb.AppendLine(output.Key);
            }
            return sb.ToString();
        }

        private void AddDirectory(FilePath filePath)
        {
            foreach (var dir in Directory.EnumerateDirectories(filePath))
            {
                var location = new DirectoryLocation(dir);
                _lines.Add(location.ToString(), location);
            }
            EnumerateFiles(filePath);
        }

        private void EnumerateFiles(string dir)
        {
            foreach (var file in Directory.EnumerateFiles(dir))
            {
                var location = new FileLocation(file);
                _lines.Add(location.ToString(), location);
            }
        }
    }
    //public class CustomEditorDocumentController : FileDocumentController
    //{
    //    // This field keeps the reference to the controller of the embedded editor
    //    FileDocumentController? editorController;
    //    private VinegarDescriptor modelDescriptor;

    //    public CustomEditorDocumentController(VinegarDescriptor modelDescriptor)
    //    {
    //        this.modelDescriptor = modelDescriptor;
    //    }

    //    public FilePath Path { get; set; }

    //    //protected override async Task OnLoad(bool reloading)
    //    //{
    //    //    // Get the content from the site
    //    //    HttpClient client = new HttpClient();
    //    //    var buffer = new BufferFromDocument(modelDescriptor.Url);
    //    //    using var stream = new MemoryStream();
    //    //    stream.Write(System.Text.Encoding.UTF8.GetBytes(buffer.Build()));
    //    //    stream.Position = 0;
    //    //    //text = await client.GetStringAsync(.Url);
    //    //    await Initialize(modelDescriptor);
    //    //}
    //    //// Initializes the controller, using the provided file name, type and content
    //    //public Task InitializeWithContent(string fileName, string fileType, FilePath path)
    //    //{
    //    //    Path = path;
    //    //    // Write the text into a Stream, since that's what FileDescriptor can take as content
    //    //    var buffer = new BufferFromDocument(path);
    //    //    using var stream = new MemoryStream();
    //    //    stream.Write(System.Text.Encoding.UTF8.GetBytes(buffer.Build()));
    //    //    stream.Position = 0;

    //    //    // Create the file descriptor to be loaded in the editor
    //    //    var descriptor = new FileDescriptor(fileName, fileType, stream, null);

    //    //    // Initialize the controller
    //    //    return Initialize(descriptor);
    //    //}

    //    //protected override async Task OnInitialize(ModelDescriptor modelDescriptor, Properties status)
    //    //{
    //    //    await base.OnInitialize(modelDescriptor, status);

    //    //    // Create a text editor controller
    //    //    editorController = await IdeServices.DocumentControllerService.CreateTextEditorController((FileDescriptor)modelDescriptor);

    //    //    // Initialize the text editor
    //    //    await editorController.Initialize(modelDescriptor);

    //    //    // In this example the view is going to show some static text, so we make it read only
    //    //    IsReadOnly = false;

    //    //    // Don't mark the file as "dirty"
    //    //    HasUnsavedChanges = false;
    //    //}

    //    protected override Task<DocumentView> OnInitializeView()
    //    {
    //        // Return the editor view as own view
    //        return editorController!.GetDocumentView();
    //    }

    //    protected override void OnContentShown()
    //    {
    //        base.OnContentShown();

    //        // Get the ITextView of the editor. This has to be done in OnContentShown since
    //        // this is when the editor view is created.

    //        var textView = editorController!.GetContent<ITextView>();

    //        // Make the editor read-only
    //        //textView.Options.SetOptionValue(new ViewProhibitUserInput().Key, true);
    //    }
    //    protected override bool OnTryReuseDocument(ModelDescriptor modelDescriptor)
    //    {
    //        // This method is overriden to avoid opening a new document for a url if one is already open.
    //        // It returns True if the descriptor provided matches the one that this controller is using.
    //        return true;
    //    }
    //}
}