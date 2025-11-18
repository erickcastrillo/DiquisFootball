using Diquis.Application.Common.Marker;
using QuestPDF.Infrastructure;

namespace Diquis.Infrastructure.PdfExport
{
    public interface IQuestPdfTemplate<T> : IScopedService
    {
        Task<IDocument> BuildAsync(T data);
    }
}
