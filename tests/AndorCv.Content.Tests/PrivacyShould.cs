using System.Text;
using Sipos.Resume.Core.Validation;

namespace AndorCv.Content.Tests;

/// <summary>
/// No file of the repository holds an e-mail address: the CV shows one only inside the PDFs, as an image, from the
/// CV_EMAIL secret. Crawlers read public repositories as well as public sites.
/// </summary>
public class PrivacyShould
{
    private static readonly string[] TextExtensions = [".json", ".md", ".cs", ".csproj", ".props", ".targets", ".slnx", ".yml", ".yaml", ".config", ".txt", ".editorconfig", ".gitignore", ""];

    [Fact]
    public void KeepEveryAddressOutOfRepository()
    {
        var files = Repository.Files().Where(file => TextExtensions.Contains(Path.GetExtension(file))).ToList();

        files.ShouldNotBeEmpty();
        files.Where(file => EmailGuard.ContainsAddress(File.ReadAllText(file, Encoding.UTF8))).Select(file => Path.GetRelativePath(Repository.Root, file)).ShouldBeEmpty();
    }

    // The address of the old CV must not come back with a copied sentence. The domain is assembled, so this file
    // does not name it either.
    [Fact]
    public void LeaveOldDomainOut()
    {
        var domain = string.Join('.', "sipos", "ws");

        Repository.Files().Where(file => TextExtensions.Contains(Path.GetExtension(file)) && File.ReadAllText(file).Contains(domain, StringComparison.OrdinalIgnoreCase))
            .ShouldBeEmpty();
    }

    [Fact]
    public void NameNoEmailInBasics()
    {
        foreach (var file in Directory.GetFiles(Repository.Content, "resume.*.json"))
        {
            File.ReadAllText(file).ShouldNotContain("\"email\"", Case.Insensitive, Path.GetFileName(file));
        }
    }
}
