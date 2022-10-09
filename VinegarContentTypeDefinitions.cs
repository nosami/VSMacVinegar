using Microsoft.VisualStudio.Utilities;
using ExportAttribute = System.ComponentModel.Composition.ExportAttribute;

namespace Vinegar
{
    static class VinegarContentTypeDefinitions
    {
        [Export]
        [Name(ContentTypeNames.VinegarContentType)]
        [BaseDefinition(ContentTypeNames.RoslynContentType)]
        public static readonly ContentTypeDefinition VinegarContentTypeDefinition;
    }
}
