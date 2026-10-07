using System.Text;
using Sipos.Resume.Core.Validation;

namespace AndorCv.Content.Tests;

/// <summary>
/// No file of the repository holds an e-mail address: the CV shows one only inside the PDFs, as an image, from the
/// CV_EMAIL secret. Crawlers read public repositories as well as public sites.
/// </summary>
public class PrivacyShould
{
    // Decision: every file is scanned, whatever its extension, unless its first bytes show it is binary (a NUL byte).
    // Why: an allow-list of text extensions misses the file type nobody thought of; a source file, a script or an SVG
    // can carry an address as well as a JSON file.
    private static List<string> TextFiles() =>
        [.. Repository.Files().Where(file => !IsBinary(file))];

    [Fact]
    public void KeepEveryAddressOutOfRepository()
    {
        var files = TextFiles();

        files.ShouldContain(file => file.EndsWith("resume.en.json", StringComparison.Ordinal));
        files.Where(file => EmailGuard.ContainsAddress(File.ReadAllText(file, Encoding.UTF8))).Select(file => Path.GetRelativePath(Repository.Root, file)).ShouldBeEmpty();
    }

    // The address of the old CV must not come back with a copied sentence. The domain is assembled, so this file
    // does not name it either.
    [Fact]
    public void LeaveOldDomainOut()
    {
        var domain = string.Join('.', "sipos", "ws");

        TextFiles().Where(file => File.ReadAllText(file).Contains(domain, StringComparison.OrdinalIgnoreCase)).ShouldBeEmpty();
    }

    [Fact]
    public void NameNoEmailInBasics()
    {
        foreach (var file in Directory.GetFiles(Repository.Content, "resume.*.json"))
        {
            File.ReadAllText(file).ShouldNotContain("\"email\"", Case.Insensitive, Path.GetFileName(file));
        }
    }

    private static bool IsBinary(string file)
    {
        Span<byte> head = stackalloc byte[8000];
        using var stream = File.OpenRead(file);
        var read = stream.Read(head);
        return head[..read].Contains((byte)0);
    }
}
