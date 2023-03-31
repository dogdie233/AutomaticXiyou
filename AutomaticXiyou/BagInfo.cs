using XiyouApi;
using XiyouApi.Model;

public class BagInfo
{
    private XiyouID _id;

    public XiyouID ID => _id;

    private BagInfo(XiyouID id)
    {
        _id = id;
    }

    /*public async Task UpdateAsync()
    {
        
    }

    public static async Task<BagInfo> Create(BagModel bag)
    {
        var bag = new BagInfo(id);
        await bag.UpdateAsync();
        return bag;
    }*/
}