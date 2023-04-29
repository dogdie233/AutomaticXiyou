using AutomaticXiyou.Singsound;
using AutomaticXiyou.Util;
using NLog;

using XiyouApi;
using XiyouApi.Model;

namespace AutomaticXiyou.HomeworkResolver
{
    public sealed class PaperHomeworkResolver : BaseHomeworkResolver
    {
        private ILogger _logger;

        public PaperHomeworkResolver(BagModel bag, HomeworkModel homework) : base(bag, homework)
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        public override Task<string?> DoAction()
        {
            return Task.FromResult<string?>(null);
        }
    }
}