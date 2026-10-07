namespace AndorCv.Content.Tests;

/// <summary>The repository's files, found from the test's output folder.</summary>
internal static class Repository
{
    public static string Root { get; } = FindRoot();

    public static string Content => Path.Combine(Root, "content");

    /// <summary>Every file the repository holds, without git's, the build outputs and the generated site.</summary>
    public static IEnumerable<string> Files()
    {
        string[] skipped = [".git", "bin", "obj", "build", "site", "node_modules"];
        var pending = new Stack<string>([Root]);
        while (pending.Count > 0)
        {
            var folder = pending.Pop();
            foreach (var file in Directory.EnumerateFiles(folder))
            {
                yield return file;
            }

            foreach (var child in Directory.EnumerateDirectories(folder).Where(child => !skipped.Contains(Path.GetFileName(child))))
            {
                pending.Push(child);
            }
        }
    }

    private static string FindRoot()
    {
        for (var folder = new DirectoryInfo(AppContext.BaseDirectory); folder is not null; folder = folder.Parent)
        {
            if (File.Exists(Path.Combine(folder.FullName, "AndorCv.slnx")))
            {
                return folder.FullName;
            }
        }

        throw new DirectoryNotFoundException("The repository root (AndorCv.slnx) was not found above the test output.");
    }
}
