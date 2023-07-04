namespace Snowsharp.Client.Models.Commons;

public interface ISnowflakePrincipal
{
    public string GetObjectType();

    public string GetObjectIdentifier();
}