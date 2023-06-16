namespace Snowsharp.Client.Models.Assets;

public interface ISnowflakeAsset
{
    public string GetCreateStatement();
    public string GetDeleteStatement();
}