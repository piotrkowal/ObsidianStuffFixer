// Probably should be moved to config file or inp argument
IEnumerable<string> blackList = [".obsidian", ".trash", "assets"];
string notepadPath = @"C:\Users\piotr\notepad";
string booksLocation = @"Zbiory\książki\ebooki\";

var notepadHierarchy = GetNotepadFilesList(notepadPath, blackList);
var books = GetBooksFiles(notepadHierarchy, booksLocation);

foreach (var book in books)
{
    Console.WriteLine(book);
    var lines = File.ReadAllLines(book).ToList();

    var lineIndex = 0;
    string splittedLine = string.Empty;
    foreach (var line in lines)
    {
        if (line.StartsWith("cover: \"") && !line.EndsWith(".png]]\""))
        {
            splittedLine = line.Split('|').FirstOrDefault() + "]]\"";
            Console.WriteLine(lineIndex+" "+splittedLine);
            lines[lineIndex] = splittedLine;
            break;
        }
        lineIndex++;
    }
    if (lineIndex != 0) File.WriteAllLines(book, lines);
}

IEnumerable<string> GetNotepadFilesList(string containerPath, IEnumerable<string> ignoreList)
{
    List<string> fileList = [];
    IEnumerable<String> mainPathDirs = Directory.EnumerateDirectories(containerPath).ToList();

    foreach (var subdir in mainPathDirs)
    {
        if (IsDirBlacklisted(subdir)) continue;
        Console.WriteLine($"Checking {subdir} path...");
        var subdirFiles = Directory.EnumerateFiles(subdir, "*", SearchOption.AllDirectories).ToList();
        fileList.AddRange(subdirFiles);
    }

    return fileList;

    bool IsDirBlacklisted(string dirName) => ignoreList.Any(path => dirName.Contains(path));
}

IEnumerable<string> GetBooksFiles(IEnumerable<string> dirTree, string booksPath) => dirTree.Where(x => x.Contains(booksPath));
