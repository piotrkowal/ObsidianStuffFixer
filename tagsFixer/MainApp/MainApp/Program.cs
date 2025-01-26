// Probably should be moved to config file or inp argument
using System.Globalization;

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
            Console.WriteLine(lineIndex + " " + splittedLine);
            lines[lineIndex] = splittedLine;
            break;
        }
        lineIndex++;
    }
    if (lineIndex != 0) File.WriteAllLines(book, lines);
}


string summaryDate = "2025-01-26";
var toReturn = CalculateExpensesFromVault(notepadPath, summaryDate);
Console.WriteLine($"Do zwrotu: {toReturn}");


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


string FindFileInVault(string vaultPath, string title)
{
    var files = Directory.EnumerateFiles(vaultPath, "*", SearchOption.AllDirectories).ToList();
    var filesMatchingTitle = files.Where(x => x.EndsWith($"{title}.md")).ToList();
    return filesMatchingTitle.SingleOrDefault(defaultValue:string.Empty);
}


double CalculateExpensesFromVault(string vaultPath, string date)
{
    var summaryNotePath = FindFileInVault(notepadPath, summaryDate);
    var noteContent = File.ReadAllLines(summaryNotePath).ToList();
    noteContent.RemoveAll(line => line.Equals(string.Empty));

    List<Expenses> monthlyExpenses = [new Expenses("Całość:", 1), new Expenses("Na pół:", 0.5)];
    monthlyExpenses.ForEach(x =>
    {
        var indexStart = noteContent.FindIndex(line => line.Equals(x.Title));
        var costsRaw = noteContent.GetRange(indexStart + 1, noteContent.Count - (indexStart + 1));
        var indexEnd = costsRaw.FindIndex(line => !char.IsDigit(line[0]));

        List<string> costs = costsRaw.GetRange(0, indexEnd < 0 ? costsRaw.Count : indexEnd);
        x.Cost = costs.ConvertAll(cost => Convert.ToDouble(cost, new CultureInfo("en-us")));
    });


    return monthlyExpenses.Sum(expense => expense.Cost.Sum() * expense.Value);

    
}
class Expenses
{
    public Expenses(string title)
    {
        Title = title;
        StartCount = false;
        Cost = [];
        Value = 1;
    }
    public Expenses(string title, double value)
    {
        Title = title;
        StartCount = false;
        Cost = [];
        Value = value;
    }

    public string Title { get; set; }
    public bool StartCount { get; set; }
    public double Value { get; set; }
    public IEnumerable<double> Cost { get; set; }
}
