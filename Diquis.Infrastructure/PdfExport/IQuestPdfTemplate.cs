using Diquis.Application.Common.Marker;
using QuestPDF.Infrastructure;

namespace Diquis.Infrastructure.PdfExport
{
    /// <summary>
    /// Defines a contract for creating a QuestPDF document from a data model.
    /// </summary>
    /// <typeparam name="T">The type of the data model used to build the document.</typeparam>
    public interface IQuestPdfTemplate<T> : IScopedService
    {
        /// <summary>
        /// Asynchronously builds a QuestPDF document based on the provided data.
        /// </summary>
        /// <param name="data">The data model to use for building the document.</param>
        /// <returns>A task that represents the asynchronous build operation. The task result contains the generated <see cref="IDocument"/>.</returns>
        Task<IDocument> BuildAsync(T data);
    }
}
