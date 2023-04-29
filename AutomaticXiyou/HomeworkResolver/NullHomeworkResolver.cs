using XiyouApi.Model;

namespace AutomaticXiyou.HomeworkResolver
{
    public class NullHomeworkResolver : BaseHomeworkResolver
    {
        public NullHomeworkResolver(BagModel bag, HomeworkModel homework) : base(bag, homework) { }

        public override Task<string?> DoAction()
        {
            return Task.FromResult($"�޷���������Ϊ{homeworkModel.Flag}����ҵ")!;
        }
    }
}