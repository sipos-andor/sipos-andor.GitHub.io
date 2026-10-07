using Sipos.Resume.Core.Content;

namespace AndorCv.Content.Tests;

/// <summary>The CV's content loads in every language exactly as the build will load it.</summary>
public class ContentShould
{
    [Fact]
    public void LoadWithoutIssuesInFourLanguages()
    {
        var files = Directory.GetFiles(Repository.Content, "*.json").Select(path => new ContentFile(Path.GetFileName(path), File.ReadAllBytes(path))).ToList();

        var result = ContentLoader.Load(files, analyticsToken: null);

        result.Issues.Select(issue => issue.ToString()).ShouldBeEmpty();
        result.Set!.Languages.Select(language => (language.Tag, language.HomePath, language.Endonym))
            .ShouldBe([("en", "/", "English"), ("hu", "/hu/", "Magyar"), ("hr", "/hr/", "Hrvatski"), ("sr-Latn", "/sr/", "Srpski")]);
        result.Set.Settings.Origin.ShouldBe(new Uri("https://andor.sipos.io"));
    }
}
