using XiyouApi.Model;

namespace AutomaticXiyou.HomeworkResolver
{
    public abstract class BaseHomeworkResolver
    {
        protected BagModel bagModel;
        protected HomeworkModel homeworkModel;

        public BaseHomeworkResolver(BagModel bag, HomeworkModel homework)
        {
            bagModel = bag;
            homeworkModel = homework;
        }

        public abstract Task<string?> DoAction();
    }
}