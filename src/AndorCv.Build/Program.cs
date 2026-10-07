// Builds andor.sipos.io from content/: `dotnet publish src/AndorCv.Build -c Release -o build`, then
// `dotnet build/AndorCv.Build.dll --content content --output site --clean`. The e-mail address the PDFs show as an
// image comes from RESUME_CONTACT_EMAIL (the CV_EMAIL secret in CI), never from the repository.
using QuestPDF.Infrastructure;
using Sipos.Resume.Documents.Pdf;
using Sipos.Resume.Generation;
using Sipos.Resume.Theme.Operandor;

// Decision: QuestPDF's Community licence.
// Why: this is Andor's personal CV, not a product of operandor; the Community licence covers individuals.
var pdf = new PdfWriterOptions(LicenseType.Community);
return await ResumeGenerator.Create(args)
    .UseTheme(new OperandorTheme())
    .UseWriter(new PdfDocumentWriter(pdf))
    .UseShareImages(new PdfShareImageWriter(pdf))
    .RunAsync();
