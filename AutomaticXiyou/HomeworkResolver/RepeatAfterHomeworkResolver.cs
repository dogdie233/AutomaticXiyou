using AutomaticXiyou.Singsound;
using AutomaticXiyou.Util;
using NLog;

using System.Text.Json;

using XiyouApi;
using XiyouApi.Model;

namespace AutomaticXiyou.HomeworkResolver
{
    public sealed class RepeatAfterHomeworkResolver : BaseHomeworkResolver
    {
        private ILogger _logger;

        public RepeatAfterHomeworkResolver(BagModel bag, HomeworkModel homework) : base(bag, homework)
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        public override async Task<string?> DoAction()
        {
            var paperGroupId = homeworkModel.PaperGroupId;
            var repeatAfterRes = await Xiyou.GetRepeatAfterById(paperGroupId);
            if (repeatAfterRes.Data == null)
            {
                _logger.Warn("无法获取到作业{WorkName}的题目数据，原因：{Message}，State：{State}", homeworkModel.Name, repeatAfterRes.Note, repeatAfterRes.State);
                return "获取作业题目数据失败";
            }

            var originalAudioMp3Name = Path.GetFileName(repeatAfterRes.Data.AudioUrl);
            (var originalAudioMp3Path, var originalAudioMp3FileStream) = CacheManager.Create(originalAudioMp3Name);
            using (originalAudioMp3FileStream)
            {
                _logger.Debug("Downloading audio {AudioUrl} to {OriginalAudioMp3Path}", repeatAfterRes.Data.AudioUrl, originalAudioMp3Path);
                await Utils.DownloadFileAsync(repeatAfterRes.Data.AudioUrl, originalAudioMp3FileStream);
            }

            // Convert mp3 to wav
            var originalAudioWavPath = originalAudioMp3Path;
            _logger.Debug("Converting audio {OriginalAudioMp3Path} to wav format {OriginalAudioWavPath}", originalAudioMp3Path, originalAudioWavPath);
            if (Path.GetExtension(originalAudioMp3Name) != ".wav")
            {
                originalAudioWavPath = Path.Combine(Path.GetDirectoryName(originalAudioMp3Path) ?? "", Path.GetFileNameWithoutExtension(originalAudioMp3Name) + ".wav");
                if (!await FFmpegHelper.ConvertAsync(originalAudioMp3Path, originalAudioWavPath))
                {
                    return "转换音频到wav失败";
                }
            }

            // Convert reader sound to custom
            var audioConverter = new SoVitsSvcAudioConverter();
            _logger.Debug("Converting reader sound to custom sound using {Converter}", audioConverter.GetType().ToString());
            var doneAudioWavPath = await audioConverter.ConvertAsync(originalAudioWavPath);
            if (doneAudioWavPath == null)
            {
                return "转换音频失败";
            }

            _logger.Debug("Converting custom wav to 16000Hz and mono");
            var doneAndFormatAudioWavPath = Path.Combine(CacheManager.baseDir, Path.GetFileNameWithoutExtension(doneAudioWavPath) + "formated.wav");
            if (!await FFmpegHelper.ConvertAsync(doneAudioWavPath, doneAndFormatAudioWavPath, 16000, 1))
            {
                return "转换音频至上传音频格式失败";
            }

            // Calculate score
            using var doneAndFormatAudioWavStream = File.OpenRead(doneAndFormatAudioWavPath);
            var singsoundResult = await SingsoundHelper.GetSingsoundResult("en.pred.score", Utils.FilterRefText(repeatAfterRes.Data.Content), upload =>
            {
                /*foreach (var stream in OggHelper.ConvertWavToOggAndSplit(doneAndFormatAudioWavStream, 0.5))
                {
                    await upload(stream);
                }*/
                upload(doneAndFormatAudioWavStream);
            });
            var score = JsonDocument.Parse(singsoundResult).RootElement.GetProperty("result").GetProperty("overall").GetDouble();
            _logger.Info("做题完成，得分{Score}", score);
            var saveRes = await Xiyou.SaveRepeatAfterAnswer(paperGroupId, singsoundResult, score, 1, repeatAfterRes.Data.PassageType, homeworkModel.Id);
            if (saveRes.State != 11)
                return saveRes.Note;
            return null;
        }
    }
}