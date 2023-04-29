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
                _logger.Warn("�޷���ȡ����ҵ{WorkName}����Ŀ���ݣ�ԭ��{Message}��State��{State}", homeworkModel.Name, repeatAfterRes.Note, repeatAfterRes.State);
                return "��ȡ��ҵ��Ŀ����ʧ��";
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
                    return "ת����Ƶ��wavʧ��";
                }
            }

            // Convert reader sound to custom
            var audioConverter = new SoVitsSvcAudioConverter();
            _logger.Debug("Converting reader sound to custom sound using {Converter}", audioConverter.GetType().ToString());
            var doneAudioWavPath = await audioConverter.ConvertAsync(originalAudioWavPath);
            if (doneAudioWavPath == null)
            {
                return "ת����Ƶʧ��";
            }

            _logger.Debug("Converting custom wav to 16000Hz and mono");
            var doneAndFormatAudioWavPath = Path.Combine(CacheManager.baseDir, Path.GetFileNameWithoutExtension(doneAudioWavPath) + "formated.wav");
            if (!await FFmpegHelper.ConvertAsync(doneAudioWavPath, doneAndFormatAudioWavPath, 16000, 1))
            {
                return "ת����Ƶ���ϴ���Ƶ��ʽʧ��";
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
            _logger.Info("������ɣ��÷�{Score}", score);
            var saveRes = await Xiyou.SaveRepeatAfterAnswer(paperGroupId, singsoundResult, score, 1, repeatAfterRes.Data.PassageType, homeworkModel.Id);
            if (saveRes.State != 11)
                return saveRes.Note;
            return null;
        }
    }
}