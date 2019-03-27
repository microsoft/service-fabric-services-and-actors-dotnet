// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors
{
    using System;
    using System.Fabric.Common;
    using System.Globalization;
    using System.Runtime.Serialization;
    using System.Text;
    using Microsoft.ServiceFabric.Services;

    /// <summary>
    /// The ActorId represents the identity of an actor within an actor service. This is used to identify the partition of the actor service inside which the actor will run, see <see cref="GetPartitionKey"/>.
    /// </summary>
    [DataContract(Name = "ActorId")]
    public sealed class ActorId : IEquatable<ActorId>, IComparable<ActorId>
    {
        private static readonly Random Rand = new Random();
        private static readonly object RandLock = new object();

        [DataMember(Name = "Kind", Order = 1, IsRequired = true)]
        private readonly ActorIdKind kind;

        [DataMember(Name = "LongId", Order = 2, IsRequired = false, EmitDefaultValue = false)]
        private readonly long longId;

        [DataMember(Name = "GuidId", Order = 3, IsRequired = false, EmitDefaultValue = false)]
        private readonly Guid guidId;

        [DataMember(Name = "StringId", Order = 4, IsRequired = false, EmitDefaultValue = false)]
        private readonly string stringId;

        private volatile string stringRepresentation;
        private volatile string storageKey;
        private long? partitionKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorId"/> class with Id value of type <see cref="long"/>.
        /// </summary>
        /// <param name="id">Value for actor id.</param>
        public ActorId(long id)
        {
            this.kind = ActorIdKind.Long;
            this.longId = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorId"/> class with Id value of type <see cref="System.Guid"/>.
        /// </summary>
        /// <param name="id">Value for actor id.</param>
        public ActorId(Guid id)
        {
            this.kind = ActorIdKind.Guid;
            this.guidId = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorId"/> class with Id value of type <see cref="string"/>.
        /// </summary>
        /// <param name="id">Value for actor id.</param>
        public ActorId(string id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }

            this.kind = ActorIdKind.String;
            this.stringId = id;
        }

        /// <summary>
        /// Gets the <see cref="ActorIdKind"/> for the ActorId.
        /// </summary>
        /// <value><see cref="ActorIdKind"/> for the ActorId.</value>
        public ActorIdKind Kind
        {
            get { return this.kind; }
        }

        /// <summary>
        /// Determines whether two specified actorIds have the same id and <see cref="ActorIdKind"/>.
        /// </summary>
        /// <param name="x">The first actorId to compare, or null. </param>
        /// <param name="y">The second actorId to compare, or null. </param>
        /// <returns>true if the id and <see cref="ActorIdKind"/> is same for both objects; otherwise, false.</returns>
        public static bool operator ==(ActorId x, ActorId y)
        {
            if (ReferenceEquals(x, null) && ReferenceEquals(y, null))
            {
                return true;
            }
            else if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
            {
                return false;
            }
            else
            {
                return EqualsContents(x, y);
            }
        }

        /// <summary>
        /// Determines whether two specified actorIds have different values for id and <see cref="ActorIdKind"/>.
        /// </summary>
        /// <param name="x">The first actorId to compare, or null. </param>
        /// <param name="y">The second actorId to compare, or null. </param>
        /// <returns>true if the id or <see cref="ActorIdKind"/> is different for both objects; otherwise, true.</returns>
        public static bool operator !=(ActorId x, ActorId y)
        {
            return !(x == y);
        }

        /// <summary>
        /// Create a new instance of the <see cref="ActorId"/> of kind <see cref="ActorIdKind.Long"/>
        /// with a random <see cref="long"/> id value.
        /// </summary>
        /// <returns>A new ActorId object.</returns>
        /// <remarks>This method is thread-safe and generates a new random <see cref="ActorId"/> every time it is called.</remarks>
        public static ActorId CreateRandom()
        {
            var buffer = new byte[8];
            lock (RandLock)
            {
                Rand.NextBytes(buffer);
            }

            return new ActorId(BitConverter.ToInt64(buffer, 0));
        }

        /// <summary>
        /// Gets id for ActorId whose <see cref="ActorIdKind"/> is <see cref="ActorIdKind.Long"/>.
        /// </summary>
        /// <returns><see cref="long"/>The id value for ActorId.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="Kind"/> is not <see cref="ActorIdKind.Long"/>.</exception>
        public long GetLongId()
        {
            if (this.kind == ActorIdKind.Long)
            {
                return this.longId;
            }

            throw new InvalidOperationException(
                string.Format(
                    CultureInfo.CurrentCulture,
                    SR.InvalidActorKind,
                    "GetLongId",
                    this.kind.ToString()));
        }

        /// <summary>
        /// Gets id for ActorId whose <see cref="ActorIdKind"/> is <see cref="ActorIdKind.Guid"/>.
        /// </summary>
        /// <returns><see cref="Guid"/>The id value for ActorId.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="Kind"/> is not <see cref="ActorIdKind.Guid"/>.</exception>
        public Guid GetGuidId()
        {
            if (this.kind == ActorIdKind.Guid)
            {
                return this.guidId;
            }

            throw new InvalidOperationException(
                string.Format(
                    CultureInfo.CurrentCulture,
                    SR.InvalidActorKind,
                    "GetGuidId",
                    this.kind.ToString()));
        }

        /// <summary>
        /// Gets id for ActorId whose <see cref="ActorIdKind"/> is <see cref="ActorIdKind.String"/>.
        /// </summary>
        /// <returns><see cref="string"/>The id value for ActorId.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="Kind"/> is not <see cref="ActorIdKind.Guid"/>.</exception>
        public string GetStringId()
        {
            if (this.kind == ActorIdKind.String)
            {
                return this.stringId;
            }

            throw new InvalidOperationException(
                string.Format(
                    CultureInfo.CurrentCulture,
                    SR.InvalidActorKind,
                    "GetStringId",
                    this.kind.ToString()));
        }

        /// <summary>
        /// Gets the partition key for this ActorId.
        /// </summary>
        /// <returns>The key for locating the partition of the actor service that is responsible for this ActorId.</returns>
        /// <remarks>
        ///     <list type="bullet">
        ///         <item>The actor service is always partitioned using <see cref="System.Fabric.Description.PartitionScheme.UniformInt64Range"/> scheme. Therefore the partition key is of <see cref="long"/> type.</item>
        ///         <item>The partition key is generated based on the <see cref="ActorIdKind"/> and the id value as follows:
        ///             <list type="bullet">
        ///                 <item><see cref="ActorIdKind.String"/>: CRC64 hash of the UTF8 bytes of the string id.</item>
        ///                 <item><see cref="ActorIdKind.Guid"/>: CRC64 hash of the bytes of the guid id.</item>
        ///                 <item><see cref="ActorIdKind.Long"/>: Actual value of the long id.</item>
        ///             </list>
        ///         </item>
        ///     </list>
        /// </remarks>
        public long GetPartitionKey()
        {
            var key = this.partitionKey;
            if (key.HasValue)
            {
                return key.Value;
            }

            long keyValue = 0;
            switch (this.kind)
            {
                case ActorIdKind.Long:
                    keyValue = this.longId;
                    break;

                case ActorIdKind.Guid:
                    keyValue = (long)CRC64.ToCRC64(this.guidId.ToByteArray());
                    break;

                case ActorIdKind.String:
                    keyValue = (long)CRC64.ToCRC64(Encoding.UTF8.GetBytes(this.stringId));
                    break;

                default:
                    ReleaseAssert.Failfast("The ActorIdKind value {0} is invalid", this.kind);
                    break;
            }

            this.partitionKey = keyValue;
            return keyValue;
        }

        /// <summary>
        /// Overrides <see cref="object.ToString"/>.
        /// </summary>
        /// <returns>Returns a string that represents the current object.</returns>
        public override string ToString()
        {
            if (this.stringRepresentation != null)
            {
                return this.stringRepresentation;
            }

            var actorIdAsString = string.Empty;
            switch (this.kind)
            {
                case ActorIdKind.Long:
                    actorIdAsString = this.longId.ToString(CultureInfo.InvariantCulture);
                    break;

                case ActorIdKind.Guid:
                    actorIdAsString = this.guidId.ToString();
                    break;

                case ActorIdKind.String:
                    actorIdAsString = this.stringId;
                    break;

                default:
                    ReleaseAssert.Failfast("The ActorIdKind value {0} is invalid", this.kind);
                    break;
            }

            this.stringRepresentation = actorIdAsString;
            return actorIdAsString;
        }

        /// <summary>
        /// Overrides <see cref="object.GetHashCode"/>.
        /// </summary>
        /// <returns>Hash code for the current object.</returns>
        public override int GetHashCode()
        {
            switch (this.kind)
            {
                case ActorIdKind.Long:
                    return this.longId.GetHashCode();

                case ActorIdKind.Guid:
                    return this.guidId.GetHashCode();

                case ActorIdKind.String:
                    return this.stringId.GetHashCode();

                default:
                    ReleaseAssert.Failfast("The ActorIdKind value {0} is invalid", this.kind);
                    return 0; // this fails the process, so unreachable code
            }
        }

        /// <summary>
        /// Determines whether this instance and a specified object, which must also be a <see cref="ActorId"/> object,
        /// have the same value. Overrides <see cref="object.Equals(object)"/>.
        /// </summary>
        /// <param name="obj">The actorId to compare to this instance. </param>
        /// <returns>true if obj is a <see cref="ActorId"/> and its value is the same as this instance;
        /// otherwise, false. If obj is null, the method returns false.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }
            else if (obj.GetType() != typeof(ActorId))
            {
                return false;
            }
            else
            {
                return EqualsContents(this, (ActorId)obj);
            }
        }

        /// <summary>
        /// Determines whether this instance and another specified <see cref="ActorId"/> object have the same value.
        /// </summary>
        /// <param name="other">The actorId to compare to this instance. </param>
        /// <returns>true if the <see cref="ActorIdKind"/> and id of the other parameter is the same as the
        /// <see cref="ActorIdKind"/> and id of this instance; otherwise, false.
        /// If other is null, the method returns false.</returns>
        public bool Equals(ActorId other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            else
            {
                return EqualsContents(this, other);
            }
        }

        /// <summary>
        /// Compares this instance with a specified <see cref="ActorId"/> object and indicates whether this
        /// instance precedes, follows, or appears in the same position in the sort order as the specified actorId.
        /// </summary>
        /// <param name="other">The actorId to compare with this instance. </param>
        /// <returns>A 32-bit signed integer that indicates whether this instance precedes, follows, or appears
        ///  in the same position in the sort order as the other parameter.</returns>
        /// <remarks>The comparison is done based on the id if both the instances have same <see cref="ActorIdKind"/>.
        /// If <see cref="ActorIdKind"/> is different, then comparison is done based on string representation of the actor id.</remarks>
        public int CompareTo(ActorId other)
        {
            return ReferenceEquals(other, null) ? 1 : CompareContents(this, other);
        }

        internal static ActorId TryGetActorIdFromStorageKey(string storageKey)
        {
            Requires.Argument("storageKey", storageKey).NotNullOrWhiteSpace();

            var idx = storageKey.IndexOf('_');

            var kind = storageKey.Substring(0, idx);
            var id = storageKey.Substring(idx + 1);

            if (kind.Equals(ActorIdKind.Guid.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return new ActorId(Guid.Parse(id));
            }

            if (kind.Equals(ActorIdKind.Long.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return new ActorId(long.Parse(id));
            }

            if (kind.Equals(ActorIdKind.String.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return new ActorId(id);
            }

            return null;
        }

        internal string GetStorageKey()
        {
            if (this.storageKey == null)
            {
                string key;
                switch (this.kind)
                {
                    case ActorIdKind.Long:
                        key = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", this.Kind.ToString(), this.longId);
                        break;

                    case ActorIdKind.Guid:
                        key = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", this.Kind.ToString(), this.guidId);
                        break;

                    case ActorIdKind.String:
                        key = string.Format(
                            CultureInfo.InvariantCulture,
                            "{0}_{1}",
                            this.Kind.ToString(),
                            this.stringId);
                        break;

                    default:
                        ReleaseAssert.Failfast("The ActorIdKind value {0} is invalid", this.kind);
                        key = null; // unreachable
                        break;
                }

                this.storageKey = key;
            }

            return this.storageKey;
        }

        internal long EstimateDataLength()
        {
            long size = sizeof(int) // Kind
                        + sizeof(long) // LongId
                        + 16; // this.guidId.ToByteArray().Length

            if (this.stringId != null)
            {
                size += this.stringId.Length * sizeof(char);
            }

            return size;
        }

        private static bool EqualsContents(ActorId x, ActorId y)
        {
            if (x.kind != y.kind)
            {
                return false;
            }

            switch (x.kind)
            {
                case ActorIdKind.Long:
                    return (x.longId == y.longId);

                case ActorIdKind.Guid:
                    return (x.guidId == y.guidId);

                case ActorIdKind.String:
                    return string.Equals(x.stringId, y.stringId, StringComparison.OrdinalIgnoreCase);

                default:
                    return false;
            }
        }

        private static int CompareContents(ActorId x, ActorId y)
        {
            if (x.kind == y.kind)
            {
                switch (x.kind)
                {
                    case ActorIdKind.Long:
                        return (x.longId.CompareTo(y.longId));

                    case ActorIdKind.Guid:
                        return (x.guidId.CompareTo(y.guidId));

                    case ActorIdKind.String:
                        return string.Compare(x.stringId, y.stringId, StringComparison.OrdinalIgnoreCase);

                    default:
                        ReleaseAssert.Failfast("The ActorIdKind value {0} is invalid", x.kind);
                        return 0; // unreachable code
                }
            }
            else
            {
                return string.Compare(x.GetStorageKey(), y.GetStorageKey(), StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
