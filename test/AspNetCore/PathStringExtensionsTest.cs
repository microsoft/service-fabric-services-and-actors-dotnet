// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using Fuzzy;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Communication.AspNetCore;

public abstract class PathStringExtensionsTest
{
    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    public sealed class StartsWithSegments : PathStringExtensionsTest
    {
        // Method parameters
        readonly PathString pathString;
        readonly PathString other;

        readonly string segment = "/" + fuzzy.String().LettersOrDigits();
        readonly string suffix = "/" + fuzzy.String().LettersOrDigits();

        public StartsWithSegments()
        {
            other = new PathString(segment);
            pathString = new PathString(segment + suffix);
        }

        [Fact]
        public void ReturnsTrueAndAssignsMatchedToOtherAndRemainingToSuffixWhenPathStringStartsWithOtherFollowedBySegmentSeparator()
        {
            Assert.True(PathStringExtensions.StartsWithSegments(pathString, other, out PathString matched, out PathString remaining));
            Assert.Equal(other.Value, matched.Value);
            Assert.Equal(suffix, remaining.Value);
        }

        [Fact]
        public void ReturnsTrueAndAssignsMatchedToPathStringAndRemainingToEmptyWhenPathStringEqualsOther()
        {
            Assert.True(PathStringExtensions.StartsWithSegments(other, other, out PathString matched, out PathString remaining));
            Assert.Equal(other.Value, matched.Value);
            Assert.Equal(string.Empty, remaining.Value);
        }

        [Fact]
        public void ReturnsTrueAndAssignsMatchedAndRemainingPreservingCaseOfPathString()
        {
            string casedSegment = segment + fuzzy.Char().Between('a', 'z');
            string upperSegment = casedSegment.ToUpperInvariant();
            var upper = new PathString(upperSegment + suffix);
            var lower = new PathString(casedSegment.ToLowerInvariant());

            Assert.True(PathStringExtensions.StartsWithSegments(upper, lower, out PathString matched, out PathString remaining));
            Assert.Equal(upperSegment, matched.Value);
            Assert.Equal(suffix, remaining.Value);
        }

        [Fact]
        public void ReturnsFalseAndAssignsMatchedAndRemainingToEmptyWhenPathStringExtendsOtherWithoutSegmentSeparator()
        {
            var extended = new PathString(segment + fuzzy.Char().Between('a', 'z') + fuzzy.String().LettersOrDigits());

            Assert.False(PathStringExtensions.StartsWithSegments(extended, other, out PathString matched, out PathString remaining));
            Assert.Equal(string.Empty, matched.Value);
            Assert.Equal(string.Empty, remaining.Value);
        }

        [Fact]
        public void ReturnsFalseAndAssignsMatchedAndRemainingToEmptyWhenPathStringDoesNotStartWithOther()
        {
            var different = new PathString("/_" + segment);

            Assert.False(PathStringExtensions.StartsWithSegments(different, other, out PathString matched, out PathString remaining));
            Assert.Equal(string.Empty, matched.Value);
            Assert.Equal(string.Empty, remaining.Value);
        }

        [Fact]
        public void ReturnsTrueAndAssignsMatchedAndRemainingToEmptyWhenPathStringAndOtherAreBothEmpty()
        {
            Assert.True(PathStringExtensions.StartsWithSegments(default, default, out PathString matched, out PathString remaining));
            Assert.Equal(string.Empty, matched.Value);
            Assert.Equal(string.Empty, remaining.Value);
        }

        [Fact]
        public void ReturnsFalseAndAssignsMatchedAndRemainingToEmptyWhenPathStringIsEmptyAndOtherIsNonEmpty()
        {
            Assert.False(PathStringExtensions.StartsWithSegments(default, other, out PathString matched, out PathString remaining));
            Assert.Equal(string.Empty, matched.Value);
            Assert.Equal(string.Empty, remaining.Value);
        }

        [Fact]
        public void ReturnsTrueAndAssignsMatchedToEmptyAndRemainingToPathStringWhenOtherIsEmptyAndPathStringStartsWithSegmentSeparator()
        {
            Assert.True(PathStringExtensions.StartsWithSegments(pathString, default, out PathString matched, out PathString remaining));
            Assert.Equal(string.Empty, matched.Value);
            Assert.Equal(pathString.Value, remaining.Value);
        }
    }
}
