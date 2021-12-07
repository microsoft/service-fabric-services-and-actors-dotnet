// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Out of Box support for ActivitySource within SF SDK
    /// </summary>
    internal class ServiceFabricActivitySource
    {
        private readonly ActivitySource serviceFabricActivitySource;
        private readonly string serviceFabricActivitySourceName = "Microsoft.ServiceFabric.ActivitySource";

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceFabricActivitySource"/> class.
        /// </summary>
        internal ServiceFabricActivitySource()
        {
            this.serviceFabricActivitySource = new ActivitySource(this.serviceFabricActivitySourceName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceFabricActivitySource"/> class.
        /// </summary>
        /// <param name="name">Name of Activity</param>
        /// <param name="kind">Kind of Activity</param>
        internal ServiceFabricActivitySource(string name, string kind = "")
        {
            this.serviceFabricActivitySource = new ActivitySource(name, kind);
        }

        // public override bool Equals(object obj)
        // {
        //    return this.serviceFabricActivitySource.Equals(obj);
        // }

        /// <summary>
        /// Adds a listener to the activity starting and stopping events.
        /// </summary>
        /// <param name="listener">listener to be added</param>
        internal static void AddActivityListener(ActivityListener listener)
        {
            ActivitySource.AddActivityListener(listener);
        }

        /// <summary>
        /// Returns the activity source name.
        /// </summary>
        /// <returns>Name of activity source</returns>
        internal string Name()
        {
            return this.serviceFabricActivitySource.Name;
        }

        /// <summary>
        /// Returns the activity source version.
        /// </summary>
        /// <returns>version of activity source</returns>
        internal string Version()
        {
            return this.serviceFabricActivitySource.Version;
        }

        /// <summary>
        /// Disposes the activity source object, removes the current instance from the global list,
        /// and empties the listeners list.
        /// </summary>
        internal void Dispose()
        {
            this.serviceFabricActivitySource.Dispose();
        }

        /// <summary>
        /// Checks if there are any listeners for this activity source.
        /// </summary>
        /// <returns>true if there is a listener registered for this activity source; otherwise, false.</returns>
        internal bool HasListeners()
        {
            return this.serviceFabricActivitySource.HasListeners();
        }

        /// <summary>
        /// Creates a new activity if there are active listeners for it,
        /// using the specified name, activity kind, parent activity context,
        /// tags, optional activity link and optional start time.
        /// </summary>
        /// <param name="name">The operation name of the activity.</param>
        /// <param name="kind">The activity kind.</param>
        /// <returns>The created activity object, if it had active listeners, or null if it has no event listeners.</returns>
        internal Activity StartActivity(string name, ActivityKind kind = ActivityKind.Internal)
        {
            return this.serviceFabricActivitySource.StartActivity(name, kind);
        }

        /// <summary>
        /// Creates a new activity if there are active listeners for it,
        /// using the specified name, activity kind, parent activity context,
        /// tags, optional activity link and optional start time.
        /// </summary>
        /// <param name="name">The operation name of the activity.</param>
        /// <param name="kind">The activity kind.</param>
        /// <param name="parentId">The parent Id to initialize the created activity object with.</param>
        /// <param name="tags">The optional tags list to initialize the created activity object with.</param>
        /// <param name="links">The optional ActivityLink list to initialize the created activity object with.</param>
        /// <param name="startTime">The optional start timestamp to set on the created activity object.</param>
        /// <returns>The created activity object, if it had active listeners, or null if it has no event listeners.</returns>
        internal Activity StartActivity(string name, ActivityKind kind, string parentId, IEnumerable<KeyValuePair<string, object>> tags = null, IEnumerable<ActivityLink> links = null, DateTimeOffset startTime = default)
        {
            return this.serviceFabricActivitySource.StartActivity(name, kind, parentId, tags, links, startTime);
        }

        /// <summary>
        /// Creates a new activity if there are active listeners for it,
        /// using the specified name, activity kind, parent activity context,
        /// tags, optional activity link and optional start time.
        /// </summary>
        /// <param name="name">The operation name of the activity.</param>
        /// <param name="kind">The activity kind.</param>
        /// <param name="parentContext">The parent ActivityContext object to initialize the created activity object with.</param>
        /// <param name="tags">The optional tags list to initialize the created activity object with.</param>
        /// <param name="links">The optional ActivityLink list to initialize the created activity object with.</param>
        /// <param name="startTime">The optional start timestamp to set on the created activity object.</param>
        /// <returns>The created activity object, if it had active listeners, or null if it has no event listeners.</returns>
        internal Activity StartActivity(string name, ActivityKind kind, ActivityContext parentContext, IEnumerable<KeyValuePair<string, object>> tags = null, IEnumerable<ActivityLink> links = null, DateTimeOffset startTime = default)
        {
            return this.serviceFabricActivitySource.StartActivity(name, kind, parentContext, tags, links, startTime);
        }
    }
}
