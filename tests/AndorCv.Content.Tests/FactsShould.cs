using Sipos.Resume.Core.Content;

namespace AndorCv.Content.Tests;

/// <summary>The facts Andor decided, in every language, so a translation or an edit cannot change them unnoticed.</summary>
public class FactsShould
{
    // Each language: its name order, its page, operandor's contact page and senior capacity page in that language.
    public static TheoryData<string, string, string, string, string> Languages => new()
    {
        { "en", "Andor Sípos", "https://andor.sipos.io/", "https://operandor.io/contact", "https://operandor.io/senior-capacity" },
        { "hu", "Sípos Andor", "https://andor.sipos.io/hu/", "https://operandor.io/hu/kapcsolat", "https://operandor.io/hu/senior-kapacitas" },
        { "hr", "Andor Sípos", "https://andor.sipos.io/hr/", "https://operandor.io/hr/kontakt", "https://operandor.io/hr/senior-kapacitet" },
        { "sr-Latn", "Andor Sípos", "https://andor.sipos.io/sr/", "https://operandor.io/sr/kontakt", "https://operandor.io/sr/senior-kapacitet" },
    };

    public static TheoryData<string> Tags => ["en", "hu", "hr", "sr-Latn"];

    [Theory]
    [MemberData(nameof(Languages))]
    public void NamePersonAndLinksInLanguage(string language, string name, string page, string contact, string capacity)
    {
        var basics = Read(language).Basics!;

        basics.Name.ShouldBe(name);
        basics.GivenName.ShouldBe("Andor");
        basics.FamilyName.ShouldBe("Sípos");
        basics.Url.ShouldBe(page);
        basics.Phone.ShouldBe("+36 30 903 6622");
        basics.Contact!.Url.ShouldBe(contact + "?intent=general&service=senior-capacity&source=andor-cv");
        basics.Availability!.Status.ShouldBe("available");
        basics.Availability.Url.ShouldBe(capacity);
        basics.Profiles.Select(profile => profile.Url).ShouldBe(["https://github.com/sipos-andor", "https://www.linkedin.com/in/sipos-andor"]);
    }

    // operandor.io gives the years as the constant 13; the CV must not compute a different number.
    [Theory]
    [MemberData(nameof(Tags))]
    public void StateThirteenYearsInSummary(string language) => Read(language).Basics!.Summary!.ShouldContain("13+");

    // Since AGCO the work went through Andor's own business, renamed operandor: one position, the clients as projects.
    [Fact]
    public void ShowOperandorAsCurrentPositionWithClientProjects()
    {
        var resume = Read("en");
        var operandor = resume.Work[0];

        (operandor.Id, operandor.Name, operandor.StartDate, operandor.EndDate).ShouldBe(("operandor", "operandor", "2022-11", null));
        resume.Projects.Where(project => project.Work == "operandor").Select(project => (project.Entity, project.StartDate, project.EndDate)).ShouldBe(
        [
            ("Lightbloom", "2025-08", "2026-05"),
            ("Supercharge", "2024-10", "2025-05"),
            ("Regatta Project Kft.", "2023-03", "2023-05"),
            ("AGCO Corporation", "2022-11", "2024-09"),
        ]);
    }

    [Fact]
    public void OfferFourPositionProfiles() =>
        Read("en").FocusProfiles.Select(profile => profile.Id).ShouldBe(["architect", "tech-lead", "ai-assisted", "senior-dotnet"]);

    // The company and its product are written in lowercase everywhere; the withdrawn product is never named.
    [Theory]
    [MemberData(nameof(Tags))]
    public void WriteBrandsAsOperandorWritesThem(string language)
    {
        var text = File.ReadAllText(Path.Combine(Repository.Content, $"resume.{language}.json"));

        text.ShouldNotContain("Operandor", Case.Sensitive);
        text.ShouldNotContain("Rovatix", Case.Sensitive);
        text.ShouldNotContain("apistem", Case.Insensitive);
    }

    private static JsonResume Read(string language)
    {
        var file = $"resume.{language}.json";
        return ResumeReader.Read(file, File.ReadAllBytes(Path.Combine(Repository.Content, file))).Resume!;
    }
}
