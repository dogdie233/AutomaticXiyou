using XiyouApi.Model;

namespace AutomaticXiyou.HomeworkResolver
{
    public class NullHomeworkResolver : BaseHomeworkResolver
    {
        public NullHomeworkResolver(BagModel bag, HomeworkModel homework) : base(bag, homework) { }

        public override Task<string?> DoAction()
        {
            return Task.FromResult($"无法处理类型为{homeworkModel.Flag}的作业")!;
        }
    }
}