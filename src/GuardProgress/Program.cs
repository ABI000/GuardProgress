using System;
using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json;
using System.Threading;

// 从配置文件中读取配置信息
Config config = LoadConfig("config.json");
while (true)
{
    if (!IsProcessRunning(config.TargetProcessName))
    {
        Console.WriteLine($"{config.TargetProcessName} 未运行，启动中...");
        //保存存档并上穿

        UplodeSave(ZipSave(config.SavePaths, config.ExtractPath, config.ZipName));
        StartProcess(config.TargetProcessName);
    }

    // 等待一段时间后重新检查
    Thread.Sleep(config.CheckIntervalMilliseconds);
}

static void UplodeSave(Stream stream)
{

}
static Stream ZipSave(List<string> paths, string extractPath, string zipName)
{
    FileStream zipToOpen = new($"{extractPath}{zipName}", FileMode.Open);
    using ZipArchive archive = new(zipToOpen, ZipArchiveMode.Update);
    foreach (var item in paths)
    {
        ZipArchiveEntry readmeEntry = archive.CreateEntry(item);
    }
    zipToOpen.Seek(0, SeekOrigin.Begin);
    return zipToOpen;
}
static bool IsProcessRunning(string processName)
{
    Process[] processes = Process.GetProcessesByName(processName);
    return processes.Length > 0;
}

static void StartProcess(string processName)
{
    try
    {
        Process.Start(processName);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"启动 {processName} 时发生错误: {ex.Message}");
    }
}

static Config LoadConfig(string configFilePath)
{
    try
    {
        // 读取配置文件内容
        string jsonConfig = File.ReadAllText(configFilePath);

        // 反序列化 JSON 到 Config 对象
        Config config = JsonSerializer.Deserialize<Config>(jsonConfig);

        return config;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"读取配置文件时发生错误: {ex.Message}");
        throw;
    }
}

internal class Config
{
    public required string TargetProcessName { get; set; }
    public int CheckIntervalMilliseconds { get; set; }

    public required List<string> SavePaths { get; set; }
    public required string ExtractPath { get; set; }
    public required string ZipName { get; set; }
}