using System.Diagnostics;

namespace VOD_Manager;

internal partial class Program {
    private static string MergeVideo(string filePath)
    {
        Console.WriteLine("Merging video...");
        if (File.Exists("/drives/500GSSD/merged.mp4"))
        {
            File.Delete("/drives/500GSSD/merged.mp4");
        }
        string location = Path.GetDirectoryName(filePath);

        var processStartInfo = new ProcessStartInfo
        {
            FileName = "./merge.sh",
            Arguments = "",
            WorkingDirectory = location,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        using var process = Process.Start(processStartInfo);

        process.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Console.WriteLine(e.Data);
            }
        };

        process.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Console.Error.WriteLine(e.Data);
            }
        };

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new Exception($"Merge script failed with exit code {process.ExitCode}");
        }
        return "/drives/500GSSD/merged.mp4";
    }
}