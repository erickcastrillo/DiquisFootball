using System.Globalization;
using Diquis.Application.Services.ProductService.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Diquis.Infrastructure.PdfExport.Templates
{
    /// <summary>
    /// Provides a PDF template for rendering product information using QuestPDF.
    /// </summary>
    public class ProductPdfTemplate : IQuestPdfTemplate<ProductDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductPdfTemplate"/> class.
        /// </summary>
        public ProductPdfTemplate()
        {
            // inject repository if you need additional data for template
        }

        /// <summary>
        /// Builds a PDF document asynchronously for the specified <see cref="ProductDTO"/>.
        /// </summary>
        /// <param name="data">The product data to include in the PDF.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the generated <see cref="IDocument"/>.
        /// </returns>
        public async Task<IDocument> BuildAsync(ProductDTO data)
        {
            return Document.Create(container =>
            {
                _ = container.Page(page =>
                {
                    page.Margin(30);
                    page.Content().Column(col =>
                    {
                        _ = col.Item().Text("Product Invoice (Example)").Bold().FontSize(20);
                        _ = col.Item().Text($"Product ID: {data.Id}");
                        _ = col.Item().Text($"Name: {data.Name}");
                        _ = col.Item().Text($"Description: {data.Description}");
                        _ = col.Item().PaddingTop(10);
                        _ = col.Item().Text($"Generated: {DateTime.Now.ToString("MM/dd/yyyy hh:mm tt", CultureInfo.GetCultureInfo("en-US"))}");
                    });
                });
            });
        }
    }
}
