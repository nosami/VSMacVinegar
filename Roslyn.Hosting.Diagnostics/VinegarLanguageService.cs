using System.Composition;
using Microsoft.CodeAnalysis.Editor;

using Microsoft.VisualStudio.Utilities;
using ExportAttribute = System.ComponentModel.Composition.ExportAttribute;
    
namespace Vinegar
{

    public static class VinegarContentTypeDefinitions
    {
        static VinegarContentTypeDefinitions()
        {
            var s = "hello";
            s = "there;";
        }
        [Export]
        [FileExtension(".vinegar")]
        [Name(ContentTypeNames.VinegarContentType)]
        [BaseDefinition(ContentTypeNames.RoslynContentType)]
        [BaseDefinition("text")]
        public static readonly ContentTypeDefinition VinegarContentTypeDefinition;
    }

    public static class ContentTypeNames
    {
        public const string RoslynContentType = "Roslyn Languages";// Microsoft.CodeAnalysis.Editor.ContentTypeNames.RoslynContentType;
        public const string VinegarContentType = "vinegar";
    }

    [ExportContentTypeLanguageService(ContentTypeNames.VinegarContentType, ContentTypeNames.VinegarContentType), Shared]
    class VinegerContentTypeLanguageService : IContentTypeLanguageService
    {
        private readonly IContentTypeRegistryService _contentTypeRegistry;

        [ImportingConstructor]
        //[Obsolete(MefConstruction.ImportingConstructorMessage, error: true)]
        public VinegerContentTypeLanguageService(IContentTypeRegistryService contentTypeRegistry)
        {
            _contentTypeRegistry = contentTypeRegistry;
        }

        public IContentType GetDefaultContentType()
        {
            return _contentTypeRegistry.GetContentType(ContentTypeNames.VinegarContentType);
        }
    }
}
