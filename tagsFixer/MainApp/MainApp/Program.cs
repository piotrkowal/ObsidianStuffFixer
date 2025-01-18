// See https://aka.ms/new-console-template for more information
using System.Collections;
using System.Collections.Generic;

Console.WriteLine("Hello, World!");




string containerPath = @"C:\Users\piotr\notepad";
Dictionary<string, IDictionary> hierarchy = new();
IEnumerable<String> mainPathDirs = Directory.EnumerateDirectories(containerPath);
IEnumerable<String> mainPathFiles = Directory.EnumerateFiles(containerPath);


IEnumerable<string> blackList = [".obsidian", ".trash", "assets"];

foreach (var subdir in mainPathDirs)
{
    if (IsDirBlacklisted(subdir)) continue;
    var subdirDirs = GetDirsContent(subdir);
    var subdirFiles = Directory.EnumerateFiles(subdir, "*", SearchOption.AllDirectories);
    hierarchy[subdir] = new Dictionary<string, IEnumerable<string>>();
    hierarchy[subdir].Add("files", subdirFiles);
}


Console.WriteLine();


IEnumerable<string> GetDirsContent(string name)
{
    return Directory.EnumerateDirectories(name);
}

bool IsDirBlacklisted(string dirName)
{
    return blackList.Any(path => dirName.Contains(path));
}

