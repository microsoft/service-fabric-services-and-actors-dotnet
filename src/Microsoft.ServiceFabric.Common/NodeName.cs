// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a Service Fabric node name.
    /// </summary>
    public class NodeName : IEquatable<NodeName>
    {
        /// <summary>
        /// A read-only instance of the <see cref="NodeName"/> class whose value is an empty string.
        /// </summary>
        /// <value>Instance of <see cref="NodeName"/> class whose value is an empty string.</value>
        public static readonly NodeName Empty = new NodeName(string.Empty);

        private readonly string nodeName;

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeName"/> class by using the value represented by the specified string.
        /// </summary>
        /// <param name="nodeName">A string for the node name.</param>
        public NodeName(string nodeName)
        {
            this.nodeName = nodeName;
        }

        /// <summary>
        /// Gets a value indicating whether this instance of NodeName represents an empty string.
        /// </summary>
        /// <value>true if this instance represents an empty string else false.</value>
        public bool IsEmpty => string.IsNullOrEmpty(this.nodeName);

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeName"/> class by using the value represented by the specified string.
        /// </summary>
        /// <param name="nodeName">A string for the node name.</param>
        /// <value>New instance of the <see cref="NodeName"/> class.</value>
        public static implicit operator NodeName(string nodeName) => new NodeName(nodeName);        

        /// <summary>
        /// Indicates whether the values of two specified <see cref="NodeName"/> objects are equal.
        /// </summary>
        /// <param name="name1">uri1 first object to compare.</param>
        /// <param name="name2">The second object to compare</param>
        /// <returns>true if name1 and name2 are equal; otherwise, false.</returns>
        public static bool operator ==(NodeName name1, NodeName name2)
            => name1.Equals(name2);

        /// <summary>
        /// Indicates whether the values of two specified <see cref="NodeName"/> objects are not equal.
        /// </summary>
        /// <param name="name1">The first object to compare.</param>
        /// <param name="name2">The second object to compare</param>
        /// <returns>true if name1 and name2 are not equal; otherwise, false.</returns>
        public static bool operator !=(NodeName name1, NodeName name2)
            => !name1.Equals(name2);
        
        /// <summary>
        /// Returns a string representation of the value of this instance.
        /// </summary>
        /// <returns>string representation of the value of this instance.</returns>
        public override string ToString() => this.nodeName.ToString();

        /// <summary>
        /// Returns a value indicating whether this instance and a specified <see cref="NodeName"/> object represent the same value.        
        /// </summary>
        /// <param name="other">An object to compare to this instance.</param>
        /// <returns>true if this instance equal to other parameter; otherwise, false.</returns>
        public bool Equals(NodeName other) => this.nodeName.Equals(other.nodeName);

        /// <summary>
        /// Returns a value that indicates whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="other">The object to compare with this instance.</param>
        /// <returns>true if other object is a <see cref="NodeName"/> that has the same value as this instance; otherwise, false.</returns>
        public override bool Equals(object other) => (other is NodeName) && this.nodeName.Equals(((NodeName)other).nodeName);

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>The hash code for this instance.</returns>
        public override int GetHashCode() => this.nodeName.GetHashCode();
    }
}
