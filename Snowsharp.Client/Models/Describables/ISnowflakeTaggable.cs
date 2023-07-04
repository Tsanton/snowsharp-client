
using Snowsharp.Client.Models.Enums;

namespace Snowsharp.Client.Models.Describables;

public interface ISnowflakeTaggable
{
    public string GetObjectType();

    public string GetObjectIdentifier();

}



