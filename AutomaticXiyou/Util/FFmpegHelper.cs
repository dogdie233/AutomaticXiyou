using NLog;

namespace AutomaticXiyou.Util
{
    public static class FFmpegHelper
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private static readonly string _ffmpegPath;

        static FFmpegHelper()
        {
            var path = Utils.FindExecutable("ffmpeg");
            if (path == null)
            {
                _logger.Fatal("无法找到ffmpeg");
                throw new FileNotFoundException("Couldn't found ffmpeg", "ffmpeg");
            }
            _ffmpegPath = path;
        }

        public static async Task<bool> ConvertAsync(string inPath, string outPath)
        {
            var aruguments = string.Format("-i \"{0}\" -y \"{1}\"", inPath, outPath);
            var flag = await Utils.RunProcess(_ffmpegPath, aruguments, _logger);
            if (!flag && File.Exists(outPath))
                return true;
            return flag;
        }

        public static async Task<bool> ConvertAsync(string inPath, string outPath, uint sampleRate, byte channel)
        {
            var aruguments = string.Format("-i \"{0}\" -ar {1} -ac {2} -y \"{3}\"", inPath, sampleRate, channel, outPath);
            var flag = await Utils.RunProcess(_ffmpegPath, aruguments, _logger);
            if (!flag && File.Exists(outPath))
                return true;
            return flag;
        }
    }
}
