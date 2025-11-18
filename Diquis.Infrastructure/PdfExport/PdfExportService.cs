using Diquis.Application.Common.PdfExport;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Diquis.Infrastructure.PdfExport
{
    /// <summary>
    /// Provides PDF export functionality using QuestPDF templates.
    /// </summary>
    public class PdfExportService : IPdfExportService
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes static members of the <see cref="PdfExportService"/> class.
        /// Sets the QuestPDF license type.
        /// </summary>
        static PdfExportService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfExportService"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider for resolving PDF templates.</param>
        public PdfExportService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Exports the specified data to a PDF document using a registered template.
        /// </summary>
        /// <typeparam name="T">The type of the data to export.</typeparam>
        /// <param name="data">The data to be exported to PDF.</param>
        /// <returns>A byte array containing the generated PDF document.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if no PDF template is registered for the specified data type.
        /// </exception>
        public async Task<byte[]> Export<T>(T data)
        {
            IQuestPdfTemplate<T> template = _serviceProvider.GetService<IQuestPdfTemplate<T>>();
            if (template is null)
            {
                throw new InvalidOperationException($"No PDF template registered for type '{typeof(T).Name}'");
            }

            IDocument document = await template.BuildAsync(data);
            using MemoryStream stream = new();
            document.GeneratePdf(stream);
            return stream.ToArray();
        }
    }
}
