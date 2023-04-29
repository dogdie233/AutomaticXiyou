using NLog;

using System.Diagnostics;
using System.Text.RegularExpressions;

namespace AutomaticXiyou.Util
{
    public static partial class Utils
    {
        [GeneratedRegex("<[^>]+?>")] private static partial Regex TagRegex();
        [GeneratedRegex("\\[(.*?)\\]")] private static partial Regex OptionRegex();


        internal static readonly HttpClient httpClient = new HttpClient();

        public static string? FindExecutable(string name)
        {
            var fileExt = OperatingSystem.IsWindows() ? ".exe" : "";
            var searchPath = new[] { Environment.CurrentDirectory, Path.Combine(Path.GetDirectoryName(Environment.ProcessPath) ?? Environment.CurrentDirectory, "Tools") };
            var envPath = Environment.GetEnvironmentVariable("PATH")?.Split(Path.PathSeparator) ?? Array.Empty<string>();
            return searchPath.Concat(envPath).Select(p => Path.Combine(p, name + fileExt)).FirstOrDefault(File.Exists);
        }

        public static async Task DownloadFileAsync(string url, Stream outStream)
        {
            var res = await httpClient.GetAsync(url);
            res.EnsureSuccessStatusCode();
            await res.Content.CopyToAsync(outStream);
        }

        public static string FilterRefText(string text) => OptionRegex().Replace(TagRegex().Replace(text, ""), "$1").Replace("#", "");

        public static async Task<bool> RunProcess(string processName, string arguments, ILogger logger)
        {
            var psi = new ProcessStartInfo(processName, arguments)
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };
            logger.Debug("Launch {ProcessPath} with argument {Argument}", psi.FileName, psi.Arguments);
            using (var process = Process.Start(psi))
            {
                if (process == null)
                    return false;
                await Task.Run(() =>
                {
                    while (true)
                    {
                        var output = process.StandardOutput.ReadLine();  // Why they output to StandardError?
                        logger.Debug(output);
                        if (process.HasExited)
                            break;
                    }
                });
                await Task.Run(() =>
                {
                    while (true)
                    {
                        var output = process.StandardError.ReadLine();  // Why they output to StandardError?
                        logger.Debug(output);
                        if (process.HasExited)
                            break;
                    }
                });
                // TODO: Redirect output stream and error stream into LogStream
                logger.Debug(process.StandardOutput.ReadToEnd());
                logger.Warn(process.StandardError.ReadToEnd());
                if (process.ExitCode != 0)
                {
                    logger.Debug("Exit code is expect 0 but {ExitCode}", process.ExitCode);
                    return false;
                }
                return true;
            }
        }
    }
}
