using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using Vim;
using ExportAttribute = System.ComponentModel.Composition.ExportAttribute;

namespace Vinegar
{
    [Export(typeof(IKeyProcessorProvider))]
    [ContentType(VimConstants.ContentType)]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    [Name("VinegarKeyProcessor")]
    [Order(Before = "VimKeyProcessor")]
    internal sealed class DefaultKeyProcessorProvider : IKeyProcessorProvider
    {
        private readonly IVim _vim;

        [ImportingConstructor]
        internal DefaultKeyProcessorProvider(IVim vim)
        {
            _vim = vim;
        }

        public KeyProcessor? GetAssociatedProcessor(ICocoaTextView cocoaTextView)
        {
            if (!_vim.VimHost.ShouldCreateVimBuffer(cocoaTextView))
                return null;

            var vimBuffer = _vim.GetOrCreateVimBuffer(cocoaTextView);
            return new VinegarKeyProcessor(vimBuffer);
        }
    }
}
