// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2
{
    using System.Diagnostics;

    /// <summary>
    /// Class ActivityIdLogicalCallContext.
    /// </summary>
    internal static class ActivityIdLogicalCallContext
    {
        /// <summary>
        /// Creates the activity.
        /// Note : You still need to start it.
        /// </summary>
        /// <param name="activityMessage">The activity message.</param>
        /// <returns>Activity.</returns>
        internal static Activity CreateW3CActivity(string activityMessage = "Create new Activity")
        {
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;
            Activity.ForceDefaultIdFormat = true;
            var activity = new Activity(activityMessage);
            return activity;
        }

        internal static void InjectHeaders(IServiceRemotingRequestMessage remotingRequestRequestMessage)
        {
            Activity currentActivity = null;

            if (ActivityIdLogicalCallContext.IsPresent())
            {
                currentActivity = ActivityIdLogicalCallContext.Get();
            }
            else
            {
                // If activity ID is not present in AsyncLocal then there's nothing to inject
                return;
            }

            if (currentActivity.IdFormat == ActivityIdFormat.W3C)
            {
                // If W3C then pass over ID, TraceState and Baggage
                remotingRequestRequestMessage.GetHeader().ActivityIdParent = currentActivity.Id.ToString();
                remotingRequestRequestMessage.GetHeader().ActivityIdTraceStateHeader = currentActivity.TraceStateString;
            }
            else
            {
                remotingRequestRequestMessage.GetHeader().ActivityRequestId = currentActivity.Id.ToString();
            }

            foreach (var item in currentActivity.Baggage)
            {
                remotingRequestRequestMessage.GetHeader().ActivityIdBaggage.Add(item);
            }
        }

        internal static void StartActivity(IServiceRemotingRequestMessage requestMessage, string activityMessage = "Start new Activity")
        {
            string parentId = null;
            if (requestMessage.GetHeader().ActivityIdParent != null)
            {
                Activity.DefaultIdFormat = ActivityIdFormat.W3C;
                Activity.ForceDefaultIdFormat = true;
                parentId = requestMessage.GetHeader().ActivityIdParent;
            }
            else if (requestMessage.GetHeader().ActivityRequestId != null)
            {
                parentId = requestMessage.GetHeader().ActivityRequestId;
            }
            else
            {
                return;
            }

            var activity = ServiceFabricActivitySource.StartActivity("StatefulDatabaseIncomingRemoteCall", ActivityKind.Server, CreateActivityContextFromTraceParent(parentId));

            if (!string.IsNullOrEmpty(parentId))
            {
                // if W3C
                if (requestMessage.GetHeader().ActivityIdTraceStateHeader != null)
                {
                    activity.TraceStateString = requestMessage.GetHeader().ActivityIdTraceStateHeader;
                }
            }

            // We expect baggage to be empty by default
            // Only very advanced users will be using it in near future, we encourage them to keep baggage small (few items)
            if (requestMessage.GetHeader().ActivityIdBaggage != null)
            {
                var baggage = requestMessage.GetHeader().ActivityIdBaggage;
                foreach (var item in baggage)
                {
                    activity.AddBaggage(item.Key, item.Value);
                }
            }
        }

        /// <summary>
        /// Determines whether this instance is present.
        /// </summary>
        /// <returns><c>true</c> if this instance is present; otherwise, <c>false</c>.</returns>
        internal static bool IsPresent()
        {
            return Activity.Current != null;
        }

        /// <summary>
        /// Tries the get.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal static Activity Get()
        {
            return Activity.Current;
        }

        /// <summary>
        /// Sets the specified activity.
        /// </summary>
        /// <param name="activity">The activity.</param>
        internal static void Set(Activity activity)
        {
            Activity.Current = activity;
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        internal static void Clear()
        {
            Activity.Current = null;
        }

        private static ActivityContext CreateActivityContextFromTraceParent(string traceParent)
        {
            if (traceParent != null)
            {
                var splitString = traceParent.Split('-');

                if (splitString.Length >= 3)
                {
                    var traceId = ActivityTraceId.CreateFromString(splitString[1].ToCharArray());
                    var spanId = ActivitySpanId.CreateFromString(splitString[2].ToCharArray());
                    ActivityContext ctx = new ActivityContext(traceId: traceId, spanId: spanId, traceFlags: ActivityTraceFlags.Recorded);
                    return ctx;
                }
            }

            return default;
        }
    }
}
