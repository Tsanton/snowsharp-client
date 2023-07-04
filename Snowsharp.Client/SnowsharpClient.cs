using Snowflake.Client;
using Snowsharp.Client.Models.Assets;
using Snowsharp.Client.Models.Describables;
using Snowsharp.Client.Models.Entities;
using Snowsharp.Client.Models.Mergeables;

namespace Snowsharp.Client;
public class SnowsharpClient : ISnowsharpClient
{
    private readonly ISnowflakeClient _cli;

    public SnowsharpClient(ISnowflakeClient cli)
    {
        _cli = cli;
    }

    public async Task<T> ExecuteScalar<T>(string query)
    {
        return await ((SnowflakeClient)_cli).ExecuteScalarAsync<T>(query);
    }

    public async Task CreateAsset(ISnowflakeAsset asset)
    {
        await PrivateCreateAsset(asset);
    }

    public async Task DeleteAsset(ISnowflakeAsset asset)
    {
        await PrivateDeleteAsset(asset);
    }

    public async Task RegisterAsset(ISnowflakeAsset asset, Stack<ISnowflakeAsset> queue)
    {
        await PrivateCreateAsset(asset);
        queue.Push(asset);
    }

    public async Task DeleteAssets(Stack<ISnowflakeAsset> queue)
    {
        while (queue.TryPop(out var asset))
        {
            await PrivateDeleteAsset(asset);
        }
    }

    public async Task<T?> ShowOne<T>(ISnowflakeDescribable describable) where T : ISnowflakeEntity
    {
        try
        {
            var query = describable.GetDescribeStatement();
            if (describable.IsProcedure())
            {
                var res = await ((SnowflakeClient)_cli).ExecuteScalarAsync<T>(query);
                return res != null ? res : default;
            }
            else
            {
                var res = await ((SnowflakeClient)_cli).QueryAsync<T>(query);
                return res.FirstOrDefault();
            }

        }
        catch (Snowflake.Client.Model.SnowflakeException e)
        {
            //If you query for objects that does not exist the client throws a SnowflakeException.
            //For non existing data objects: Object 'XXX' does not exist, or operation cannot be performed.
            //For non existing databases: Database 'XXX' does not exist or not authorized.
            //For non existing role inheritance: Role relationship does not exist or not authorized
            if (e.Message.Contains("does not exist"))
            {
                return default;
            }
            throw;
        }
    }

    public async Task<List<T>> ShowMany<T>(ISnowflakeDescribable describable) where T : ISnowflakeEntity
    {
        try
        {
            var query = describable.GetDescribeStatement();
            if (describable.IsProcedure())
            {
                var res = await ((SnowflakeClient)_cli).ExecuteScalarAsync<List<T>>(query);
                return res ?? new();
            }
            else
            {
                var res = await ((SnowflakeClient)_cli).QueryAsync<T>(query);
                return res.ToList();
            }
        }
        catch (Snowflake.Client.Model.SnowflakeException e)
        {
            //If you query for objects that does not exist the client throws a SnowflakeException.
            //For non existing data objects: Object 'XXX' does not exist, or operation cannot be performed.
            //For non existing databases: Database 'XXX' does not exist or not authorized.
            if (e.Message.Contains("does not exist"))
            {
                return new();
            }
            throw;
        }
    }

    public Task MergeInto(ISnowflakeMergeable mergeable)
    {
        throw new NotImplementedException();
    }

    public Task<T> GetMergeable<T>(T mergeable) where T : ISnowflakeMergeable
    {
        throw new NotImplementedException();
    }

    private async Task PrivateCreateAsset(ISnowflakeAsset asset)
    {
        foreach (var query in asset.GetCreateStatement().Trim().Split(";"))
        {
            //TODO: Fix this ugly /n issue
            if (query.Length > 5) await _cli.ExecuteAsync(query);
        }
    }

    private async Task PrivateDeleteAsset(ISnowflakeAsset asset)
    {
        foreach (var query in asset.GetDeleteStatement().Trim().Split(";"))
        {
            //TODO: Fix this ugly /n issue
            if (query.Length > 5) await _cli.ExecuteAsync(query);
        }
    }
}
