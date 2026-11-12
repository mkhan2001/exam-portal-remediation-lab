using System.Diagnostics;

namespace ExamPortal.Api.Services;

/// <summary>Packages exported registration data into an archive for download.</summary>
public class ArchiveService
{
    public ProcessStartInfo BuildCommand(string archiveName)
    {
        var command = "tar -czf exports/" + archiveName + ".tgz data";

        return OperatingSystem.IsWindows()
            ? new ProcessStartInfo("cmd.exe", "/c " + command)
            : new ProcessStartInfo("/bin/sh", "-c \"" + command + "\"");
    }

    public int Run(string archiveName)
    {
        var startInfo = BuildCommand(archiveName);
        startInfo.RedirectStandardError = true;
        startInfo.UseShellExecute = false;

        using var process = Process.Start(startInfo)!;
        process.WaitForExit();
        return process.ExitCode;
    }
}
