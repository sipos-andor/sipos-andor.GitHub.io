using System.Text.Json;
using NJsonSchema;

namespace AndorCv.Content.Tests;

/// <summary>
/// Every language is valid JSON Resume v1.2.1: the official schema of that release (jsonresume/resume-schema, MIT),
/// copied verbatim but for the example address in the e-mail field's description, which no file here may hold.
/// v1.2.1 leaves the root open, so the engine's top-level x- extensions are valid; partial dates are valid everywhere.
/// </summary>
public class JsonResumeSchemaShould
{
    public const string SchemaUrl = "https://raw.githubusercontent.com/jsonresume/resume-schema/v1.2.1/schema.json";

    private static readonly Lazy<JsonSchema> Schema = new(() =>
        JsonSchema.FromJsonAsync(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "jsonresume-schema-v1.2.1.json"))).GetAwaiter().GetResult());

    public static TheoryData<string> Languages => ["en", "hu", "hr", "sr-Latn"];

    [Theory]
    [MemberData(nameof(Languages))]
    public void AcceptLanguage(string language) =>
        Schema.Value.Validate(File.ReadAllText(Path.Combine(Repository.Content, $"resume.{language}.json"))).ShouldBeEmpty();

    // The file names the schema it is checked against, so an editor validates it the same way.
    [Theory]
    [MemberData(nameof(Languages))]
    public void DeclareSchemaItIsCheckedAgainst(string language)
    {
        using var json = JsonDocument.Parse(File.ReadAllText(Path.Combine(Repository.Content, $"resume.{language}.json")));

        json.RootElement.GetProperty("$schema").GetString().ShouldBe(SchemaUrl);
    }

    // The upstream schema has no closed root since v1.1.0, so the content's top-level keys are checked here: the
    // standard sections, the schema reference and the engine's x- extensions, nothing else.
    [Theory]
    [MemberData(nameof(Languages))]
    public void UseOnlyKnownTopLevelKeys(string language)
    {
        using var json = JsonDocument.Parse(File.ReadAllText(Path.Combine(Repository.Content, $"resume.{language}.json")));

        json.RootElement.EnumerateObject().Select(property => property.Name)
            .Where(name => !name.StartsWith("x-", StringComparison.Ordinal) && name != "$schema" && !KnownSections.Contains(name))
            .ShouldBeEmpty();
    }

    private static readonly string[] KnownSections =
        ["basics", "work", "volunteer", "education", "awards", "certificates", "publications", "skills", "languages", "interests", "references", "projects", "meta"];
}
