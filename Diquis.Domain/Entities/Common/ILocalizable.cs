using Diquis.Domain.Enums;

namespace Diquis.Domain.Entities.Common;

public interface ILocalizable
{
    public Locale Locale { get; set; }
}
