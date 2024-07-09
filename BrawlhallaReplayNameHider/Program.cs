using System;
using System.IO;
using System.Text;
using BrawlhallaReplayLibrary;

const int NAME_LENGTH = 16;
const string CHOOSE_FROM = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

Random random = new();
string GetRandomName()
{
    StringBuilder sb = new(NAME_LENGTH);
    for (int i = 0; i < NAME_LENGTH; ++i)
        sb.Append(CHOOSE_FROM[random.Next(0, CHOOSE_FROM.Length)]);
    return sb.ToString();
}

void TryCatch(Action a)
{
    try { a(); }
    catch (Exception e)
    {
        Console.WriteLine($"[ERROR]: {e.Message}");
#if DEBUG
        Console.WriteLine($"[TRACE]: {e.StackTrace}");
#endif

        Console.WriteLine("Press enter to exit.");
        Console.ReadLine();
        Environment.Exit(1);
    }
}

Console.WriteLine("Welcome to the brawlhalla replay name hider.");
Console.WriteLine("Please insert the path to the replay:");
string? inPath = null;
while (inPath is null)
{
    Console.Write("> ");
    inPath = Console.ReadLine();
    if (!File.Exists(inPath))
    {
        inPath = null;
        Console.WriteLine("Invalid path. Please try again:");
    }
}

Console.WriteLine("Parsing replay file...");
Replay replay = null!;
TryCatch(() =>
{
    using FileStream file = new(inPath, FileMode.Open, FileAccess.Read);
    replay = Replay.Load(file);
});

Console.WriteLine("Randomizing names...");
foreach (ReplayEntityData entity in replay.GameData.Entities)
    entity.Name = GetRandomName();

Console.WriteLine("Please insert the path for the new replay file:");
string? outPath = null;
while (outPath is null)
{
    Console.Write("> ");
    outPath = Console.ReadLine();
    // no simple way to do path validation, so rely on exception when saving
    if (outPath is null)
    {
        outPath = null;
        Console.WriteLine("Invalid path. Please try again:");
    }
}

Console.WriteLine("Saving...");
TryCatch(() =>
{
    using FileStream file = new(outPath, FileMode.Create, FileAccess.Write);
    replay.Save(file);
});

Console.WriteLine("Done!");

Console.WriteLine("Press enter to exit.");
Console.ReadLine();