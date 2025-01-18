using System.Collections;

// Probably should be moved to config file or inp argument
IEnumerable<string> blackList = [".obsidian", ".trash", "assets"];
string notepadPath = @"C:\Users\piotr\notepad";
string booksLocation = @"Zbiory\książki\ebooki\";

var notepadHierarchy = GetNotepadHierarchy(notepadPath, blackList);
var books = GetBooksFiles(notepadHierarchy);


IDictionary GetNotepadHierarchy(string containerPath, IEnumerable<string> ignoreList)
{
    Dictionary<string, IDictionary> hierarchy = new();
    IEnumerable<String> mainPathDirs = Directory.EnumerateDirectories(containerPath);
    IEnumerable<String> mainPathFiles = Directory.EnumerateFiles(containerPath);
    Console.WriteLine("Checking main dir");


    foreach (var subdir in mainPathDirs)
    {
        if (IsDirBlacklisted(subdir)) continue;
        Console.WriteLine($"Checking {subdir} path...");
        var subdirDirs = GetDirsContent(subdir);
        var subdirFiles = Directory.EnumerateFiles(subdir, "*", SearchOption.AllDirectories);
        hierarchy[subdir] = new Dictionary<string, IEnumerable<string>>();
        hierarchy[subdir].Add("files", subdirFiles);
    }

    hierarchy["."] = new Dictionary<string, IEnumerable<string>>
    {
        { "files", mainPathFiles },
        { "dirs", mainPathDirs }
    };

    return hierarchy;

    IEnumerable<string> GetDirsContent(string name) => Directory.EnumerateDirectories(name);
    bool IsDirBlacklisted(string dirName) => ignoreList.Any(path => dirName.Contains(path));
}

IEnumerable<string> GetBooksFiles(IDictionary dirTree)
{

    return [];
}

