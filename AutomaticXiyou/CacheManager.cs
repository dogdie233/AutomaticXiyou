using NLog;

namespace AutomaticXiyou
{
    public static class CacheManager
    {
        private static ILogger _logger = LogManager.GetCurrentClassLogger();
        public static readonly string baseDir = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath) ?? Environment.CurrentDirectory, "_Cache");
        private static List<string> _caches = new List<string>();

        public static (string path, FileStream file) Create(string name)
        {
            var path = Path.Combine(baseDir, name);
            Directory.CreateDirectory(baseDir);
            var stream = File.Create(path);
            _caches.Add(path);
            return (path, stream);
        }

        public static void TryDeleteAllCaches()
        {
            for (var i = _caches.Count - 1; i >= 0; i--)
            {
                var cache = _caches[i];
                if (File.Exists(cache))
                {
                    try
                    {
                        File.Delete(cache);
                    }
                    catch (Exception e)
                    {
                        _logger.Warn("无法清除缓存文件{FilePath}", cache);
                        _logger.Warn(e);
                    }
                }
                _caches.RemoveAt(i);
            }
        }
    }
}
