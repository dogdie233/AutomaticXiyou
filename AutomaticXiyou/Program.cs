using NLog;

using XiyouApi;
using XiyouApi.Model;

var logger = LogManager.GetCurrentClassLogger();
Console.Write("请输入账号：");
var account = Console.ReadLine() ?? "";
Console.Write("请输入密码：");
var password = Console.ReadLine() ?? "";


logger.Info("正在登录账户{Account}...", account);
var userInfo = (await Xiyou.Login(account, password)).Data!.UserInfo;
logger.Info("登录成功，用户名{UserName}({UserID})", userInfo.Name, userInfo.Id.ToString(), userInfo.SchoolName, userInfo.ClassName);


logger.Info("获取作业包中...");
var bags = await Xiyou.FindBagList(1, 10, BagStatus.InProgress);
if (bags.Data == null)
{
    logger.Fatal("获取作业包失败，错误码：{State}，原因：{Message}", bags.State, bags.Note);
    Environment.Exit(0);
}
logger.Info("你有{bagCount}个正在进行中的作业包", bags.Data.Length);


var failedBagList = new List<BagModel>();
for (var bagCount = 0; bagCount < bags.Data.Length; bagCount++)
{
    var bag = bags.Data[bagCount];
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
        if (work.Status == HomeworkStatus.Done)
        {
            logger.Info(" - 作业{WorkName}已经完成，跳过", work.Name);
            continue;
        }
        logger.Info(" - 正在处理{WorkName}", work.Name);
    }
}