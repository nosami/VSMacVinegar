using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editor;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.VisualStudio.Utilities;
using ExportAttribute = System.ComponentModel.Composition.ExportAttribute;
    
namespace Vinegar
{
    public static class VinegarContentTypeDefinitions
    {
        [Export]
        [FileExtension(".vinegar")]
        [Name(ContentTypeNames.VinegarContentType)]
        [BaseDefinition(ContentTypeNames.RoslynContentType)]
        [BaseDefinition("text")]
        public static readonly ContentTypeDefinition VinegarContentTypeDefinition;
    }

    [ExportContentTypeLanguageService(ContentTypeNames.VinegarContentType, ContentTypeNames.VinegarContentType, ServiceLayer.Host), Shared]
    class VinegerContentTypeLanguageService : IContentTypeLanguageService
    {
        private readonly IContentTypeRegistryService _contentTypeRegistry;

        [ImportingConstructor]
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
