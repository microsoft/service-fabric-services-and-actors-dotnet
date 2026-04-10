using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.ServiceFabric.Data.Beta
{
    /// <summary>
    /// (Beta) Not for production use - API is subject to change in the future.
    /// Manages all <see cref="IReliableState"/> for a service replica.
    /// Each replica in a service has its own state manager and thus its own set of <see cref="IReliableState"/>.
    /// </summary>
    public interface IReliableStateManager2 : IReliableStateManagerReplica2
    {
        /// <summary>
        /// Create and start a new transaction that can be used to group operations to be performed atomically with specified single read isolation level.
        /// </summary>
        /// <remarks>
        /// Operations are added to the transaction by passing the <see cref="ITransaction"/> object in to reliable state methods.
        /// This does not apply to reads on secondaries
        /// </remarks>
        /// <param name="singleEntityIsolationLevelForPrimaryReads"> The transaction-wide single item read isolation level </param>
        /// <returns>A new transaction.</returns>
        ITransaction CreateTransaction(IsolationLevel singleEntityIsolationLevelForPrimaryReads);
    }
}
