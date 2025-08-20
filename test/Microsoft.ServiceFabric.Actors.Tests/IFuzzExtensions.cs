using System;
using Fuzzy;

namespace Microsoft.ServiceFabric.Actors.Tests
{
    static class IFuzzExtensions
    {
        internal static ActorId ActorId(this IFuzz fuzzy)
        {
            switch(fuzzy.Enum<ActorIdKind>())
            {
                case ActorIdKind.Guid: return new ActorId(Guid.NewGuid());
                case ActorIdKind.Long: return new ActorId(fuzzy.Int64());
                case ActorIdKind.String: return new ActorId(fuzzy.String());
                default: throw new NotSupportedException();
            }
        }
    }
}
