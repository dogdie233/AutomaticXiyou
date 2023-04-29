using AutomaticXiyou.HomeworkResolver;

using NLog;

using XiyouApi;
using XiyouApi.Model;

namespace AutomaticXiyou
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var logger = LogManager.GetCurrentClassLogger();
            Console.Write("请输入账号：");
            var account = Console.ReadLine() ?? "";
            Console.Write("请输入密码：");
            var password = Console.ReadLine() ?? "";


            logger.Info("正在登录账户{Account}...", account);
            var userInfo = (await Xiyou.Login(account, password)).Data!.UserInfo;
            logger.Info("登录成功，用户名{UserName}({UserID})", userInfo.Name, userInfo.Id.ToString(), userInfo.SchoolName, userInfo.ClassName);


            logger.Info("获取作业包中...");
            var bags = await Xiyou.FindBagList(1, 20, BagStatus.Ended);
            if (bags.Data == null)
            {
                logger.Fatal("获取作业包失败，错误码：{State}，原因：{Message}", bags.State, bags.Note);
                Environment.Exit(0);
            }
            logger.Info("你有{BagCount}个正在进行中的作业包", bags.Data.Length);


            var failedBagList = new List<BagModel>();
            for (var bagCount = 0; bagCount < bags.Data.Length; bagCount++)
            {
                var bag = bags.Data[bagCount];
                if (bag.Id != new XiyouID("63DAA017FCAC4818A999F26712ED5212"))
                    continue;
                logger.Info("正在获取作业包{BagName}({BagId})的作业列表", bag.Name, bag.Id.ToString());
                var worksRep = await Xiyou.FindHomeworkListByBagId(bag.Id);
                if (worksRep.Data == null)
                {
                    logger.Error("获取作业包{BagName}的作业列表失败", bag.Name);
                    failedBagList.Add(bag);
                    continue;
                }
                foreach (var work in worksRep.Data)
                {
                    /*if (work.Status == HomeworkStatus.Done)
                    {
                        logger.Info(" - 作业{WorkName}已经完成，跳过", work.Name);
                        continue;
                    }*/
                    logger.Info(" - 正在处理{WorkName}", work.Name);

                    /*if (work.Id != new XiyouID("5EEB0D75080A4CCD855CC00DC92AD7B6"))
                        continue;*/

                    BaseHomeworkResolver? resolver = work.Flag switch
                    {
                        3 => new RepeatAfterHomeworkResolver(bag, work),
                        _ => new NullHomeworkResolver(bag, work),
                    };

                    try
                    {
                        var failedReason = await resolver.DoAction();
                        if (failedReason != null)
                        {
                            logger.Error(" - 在处理{WorkName}时发生错误，跳过！原因：{FailedReason}", work.Name, failedReason);
                        }
                    }
                    catch (Exception e)
                    {
                        if (resolver is IDisposable d)
                            d.Dispose();
                        logger.Error(" - 在处理{WorkName}时发生异常，跳过", work.Name);
                        logger.Error(e);
                    }
                }
            }
        }
    }
}