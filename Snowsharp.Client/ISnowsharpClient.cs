using Snowsharp.Client.Models.Assets;
using Snowsharp.Client.Models.Describables;
using Snowsharp.Client.Models.Entities;
using Snowsharp.Client.Models.Mergeables;

namespace Snowsharp.Client;

public interface ISnowsharpClient
{
    public Task<T> ExecuteScalar<T>(string query);

    public Task CreateAsset(ISnowflakeAsset asset);
    public Task DeleteAsset(ISnowflakeAsset asset);
    public Task RegisterAsset(ISnowflakeAsset asset, Stack<ISnowflakeAsset> queue);
    public Task DeleteAssets(Stack<ISnowflakeAsset> queue);
    public Task<T?> ShowOne<T>(ISnowflakeDescribable describable) where T: ISnowflakeEntity;
    public Task MergeInto(ISnowflakeMergeable mergeable);
    public Task<T> GetMergeable<T>(T mergeable) where T : ISnowflakeMergeable;

}