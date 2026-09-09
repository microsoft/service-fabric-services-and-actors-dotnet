// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Fuzzy;
using Xunit;
using static Microsoft.ServiceFabric.FabricTransport.NativeFabricTransport;

namespace Microsoft.ServiceFabric.FabricTransport;

public abstract partial class NativeFabricTransportMessageDisposerTest
{
    readonly IFabricTransportMessageDisposer sut = new NativeFabricTransportMessageDisposer();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    public sealed partial class Dispose : NativeFabricTransportMessageDisposerTest, IDisposable
    {
        // Method parameters
        uint count;
        IntPtr messages;

        NativeMessageArray nativeMessages;

        void IDisposable.Dispose() => ((IDisposable)nativeMessages)?.Dispose();

        [Fact]
        public void InvokesDisposeOnMessagesInPointerArrayOrder()
        {
            List<int> order = [];
            int id = fuzzy.Int32();
            FakeMessage[] fakes = fuzzy.Array(() => new FakeMessage { Id = id++, DisposeOrder = order }, Length.Min(2));
            nativeMessages = new NativeMessageArray(fakes);
            count = nativeMessages.Count;
            messages = nativeMessages.Ptr;

            sut.Dispose(count, messages);

            Assert.Equal(fakes.Select(static f => f.Id), order);
        }

        [Fact]
        public void DoesNotInvokeDisposeOnMessagesBeyondCount()
        {
            FakeMessage[] fakes = fuzzy.Array(static () => new FakeMessage(), Length.Min(2));
            nativeMessages = new NativeMessageArray(fakes);
            count = nativeMessages.Count - 1;
            messages = nativeMessages.Ptr;

            sut.Dispose(count, messages);

            Assert.All(fakes.Take((int)count), static f => Assert.Equal(1, f.DisposeCallCount));
            Assert.All(fakes.Skip((int)count), static f => Assert.Equal(0, f.DisposeCallCount));
        }

        [Fact]
        public void DoesNotDereferenceMessagesWhenCountIsZero() =>
            sut.Dispose(count, messages);

        [Fact(Explicit = true)] // TODO: SUT bug. Dispose crashes with AccessViolationException instead of throwing ArgumentNullException.
        public void ThrowsArgumentNullExceptionWhenMessagesIsZeroAndCountIsGreaterThanZero()
        {
            count = fuzzy.UInt32().Minimum(1u);

            // messages is left at its default IntPtr.Zero; the SUT's loop reads through that null pointer with
            // Marshal.ReadIntPtr, crashing with AccessViolationException before validating the argument.
            var actual = Assert.Throws<ArgumentNullException>(() => sut.Dispose(count, messages));
            Assert.Equal(nameof(messages), actual.ParamName);
        }

        [GeneratedComClass]
        sealed partial class FakeMessage : IFabricTransportMessage
        {
            internal int Id;
            internal int DisposeCallCount;
            internal List<int> DisposeOrder;

            void IFabricTransportMessage.GetHeaderAndBodyBuffer(out IntPtr header, out uint length, out IntPtr buffer) =>
                throw new NotImplementedException();

            void IFabricTransportMessage.Dispose()
            {
                DisposeCallCount++;
                DisposeOrder?.Add(Id);
            }
        }

        sealed class NativeMessageArray : IDisposable
        {
#if NET
            static readonly StrategyBasedComWrappers wrappers = new();
#endif
            readonly IntPtr[] iunknowns;
            internal readonly IntPtr Ptr;
            internal readonly uint Count;

            internal NativeMessageArray(FakeMessage[] messages)
            {
                iunknowns = new IntPtr[messages.Length];
                Count = (uint)messages.Length;
                Ptr = Marshal.AllocHGlobal(IntPtr.Size * messages.Length);
                for (int i = 0; i < messages.Length; i++)
                {
                    iunknowns[i] = GetIUnknownForObject(messages[i]);
                    Marshal.WriteIntPtr(Ptr, i * IntPtr.Size, iunknowns[i]);
                }
            }

            void IDisposable.Dispose()
            {
                foreach (IntPtr ptr in iunknowns)
                    if (ptr != IntPtr.Zero) _ = Marshal.Release(ptr);
                Marshal.FreeHGlobal(Ptr);
            }

            static IntPtr GetIUnknownForObject(object managed) =>
#if NET
                wrappers.GetOrCreateComInterfaceForObject(managed, CreateComInterfaceFlags.None);
#else
                Marshal.GetIUnknownForObject(managed);
#endif
        }
    }
}
