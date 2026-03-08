using PDFiumCore;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

#pragma warning disable CA1416

namespace ScaniFly.Services;

public class VisionLlmOcrService : IOcrService
{
    public string MethodName => "Vision LLM";

    public Task<OcrResult> ExtractAsync(string pdfPath)
    {
        var result = new OcrResult();

        try
        {
            fpdfview.FPDF_InitLibrary();
            var doc = fpdfview.FPDF_LoadDocument(pdfPath, null);
            if (doc == null)
                return Task.FromResult(result);

            int pageCount = fpdfview.FPDF_GetPageCount(doc);

            // Limit to first 3 pages to avoid memory/token bloat with LLM
            int maxPages = Math.Min(pageCount, 3);

            for (int i = 0; i < maxPages; i++)
            {
                var page = fpdfview.FPDF_LoadPage(doc, i);
                double width = fpdfview.FPDF_GetPageWidth(page);
                double height = fpdfview.FPDF_GetPageHeight(page);

                int renderWidth = (int)(width * 2); // 2x scaling for better quality
                int renderHeight = (int)(height * 2);

                var bitmap = fpdfview.FPDFBitmapCreateEx(renderWidth, renderHeight, (int)FPDFBitmapFormat.BGRA, IntPtr.Zero, 0);
                fpdfview.FPDFBitmapFillRect(bitmap, 0, 0, renderWidth, renderHeight, 0xFFFFFFFF);

                fpdfview.FPDF_RenderPageBitmap(bitmap, page, 0, 0, renderWidth, renderHeight, 0, 0);

                var buffer = fpdfview.FPDFBitmapGetBuffer(bitmap);
                var stride = fpdfview.FPDFBitmapGetStride(bitmap);

                // Convert to base64 jpeg
                using (var bmp = new Bitmap(renderWidth, renderHeight, stride, PixelFormat.Format32bppArgb, buffer))
                using (var ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Jpeg);
                    var base64 = Convert.ToBase64String(ms.ToArray());
                    result.Base64Images.Add(base64);
                }

                fpdfview.FPDFBitmapDestroy(bitmap);
                fpdfview.FPDF_ClosePage(page);
            }
            fpdfview.FPDF_CloseDocument(doc);
            fpdfview.FPDF_DestroyLibrary();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Vision OCR Error: {ex.Message}");
        }

        return Task.FromResult(result);
    }
}
#pragma warning restore CA1416
