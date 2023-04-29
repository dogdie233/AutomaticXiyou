using AutomaticXiyou.Util;

using NLog;

using System.Diagnostics;

namespace AutomaticXiyou
{
    public interface IAudioConverter
    {
        Task<string?> ConvertAsync(string wavInPath);
    }

    // Use: https://github.com/34j/so-vits-svc-fork
    public class SoVitsSvcAudioConverter : IAudioConverter
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private static readonly string _startArgumentSample = "infer -m \"Tools/SoVitsSvc/venv/Models/G_56800.pth\" -c \"Tools/SoVitsSvc/venv/Models/config.json\" -o \"{0}\" \"{1}\"";
        private static readonly string _inferenceToolPath = "Tools/SoVitsSvc/venv/Scripts/svc.exe";

        public async Task<string?> ConvertAsync(string wavInPath)
        {
            var wavInFileName = Path.GetFileNameWithoutExtension(wavInPath);
            var wavOutPath = Path.Combine(CacheManager.baseDir, $"{wavInFileName}.svc.out.wav");
            if (File.Exists(wavOutPath))
                return wavOutPath;
            var arguments = string.Format(_startArgumentSample, wavOutPath, wavInPath);
            var flag = await Utils.RunProcess(_inferenceToolPath, arguments, _logger);
            if (!flag && !File.Exists(wavOutPath))
                return null;
            return wavOutPath;
        }
    }
}
