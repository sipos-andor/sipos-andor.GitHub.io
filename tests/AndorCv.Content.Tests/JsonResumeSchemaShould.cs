using NJsonSchema;

namespace AndorCv.Content.Tests;

/// <summary>Every language is valid JSON Resume v1.0.0 (the official schema, MIT, from jsonresume/resume-schema).</summary>
public class JsonResumeSchemaShould
{
    private static readonly Lazy<JsonSchema> Schema = new(() =>
        JsonSchema.FromJsonAsync(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "jsonresume-schema.json"))).GetAwaiter().GetResult());

    public static TheoryData<string> Languages => ["en", "hu", "hr", "sr-Latn"];

    [Theory]
    [MemberData(nameof(Languages))]
    public void AcceptLanguage(string language) =>
        Schema.Value.Validate(File.ReadAllText(Path.Combine(Repository.Content, $"resume.{language}.json"))).ShouldBeEmpty();
}
