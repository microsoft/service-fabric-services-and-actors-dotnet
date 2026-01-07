// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Globalization;
using System.Text;
using Fuzzy;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Diagnostics.Tracing
{
    public class ITextEventSourceExtensionsTest
    {
        readonly ITextEventSource eventSource = Mock.Of<ITextEventSource>();

        // Method parameters
        readonly string type = "Type" + fuzzy.String();
        readonly string id = "Id" + fuzzy.String();
        readonly string message = "Message" + fuzzy.String();
        readonly string format;
        readonly object[] args = fuzzy.Array(() => (object)fuzzy.DateTime()); // Culture-sensitive

        public ITextEventSourceExtensionsTest()
        {
            var format = new StringBuilder("Format" + fuzzy.String().LettersOrDigits());
            for (int i = 0; i < args.Length; i++)
                format.Append("Arg" + i + "={" + i + "}");
            this.format = format.ToString();
        }

        [Fact]
        public void WriteErrorWritesMessageToEventSource()
        {
            eventSource.WriteError(type, message);
            Mock.Get(eventSource).Verify(_ => _.ErrorText(string.Empty, type, message));
        }

        [Fact]
        public void WriteErrorWritesFormattedMessageToEventSource()
        {
            eventSource.WriteError(type, format, args);
            Mock.Get(eventSource).Verify(_ => _.ErrorText(string.Empty, type, FormattedMessage()));
        }

        [Fact]
        public void WriteErrorWithIdWritesMessageToEventSource()
        {
            eventSource.WriteErrorWithId(type, id, message);
            Mock.Get(eventSource).Verify(_ => _.ErrorText(id, type, message));
        }

        [Fact]
        public void WriteErrorWithIdWritesFormattedMessageToEventSource()
        {
            eventSource.WriteErrorWithId(type, id, format, args);
            Mock.Get(eventSource).Verify(_ => _.ErrorText(id, type, FormattedMessage()));
        }

        [Fact]
        public void WriteInfoWritesMessageToEventSource()
        {
            eventSource.WriteInfo(type, message);
            Mock.Get(eventSource).Verify(_ => _.InfoText(string.Empty, type, message));
        }

        [Fact]
        public void WriteInfoWritesFormattedMessageToEventSource()
        {
            eventSource.WriteInfo(type, format, args);
            Mock.Get(eventSource).Verify(_ => _.InfoText(string.Empty, type, FormattedMessage()));
        }

        [Fact]
        public void WriteInfoWithIdWritesMessageToEventSource()
        {
            eventSource.WriteInfoWithId(type, id, message);
            Mock.Get(eventSource).Verify(_ => _.InfoText(id, type, message));
        }

        [Fact]
        public void WriteInfoWithIdWritesFormattedMessageToEventSource()
        {
            eventSource.WriteInfoWithId(type, id, format, args);
            Mock.Get(eventSource).Verify(_ => _.InfoText(id, type, FormattedMessage()));
        }

        [Fact]
        public void WriteNoiseWritesMessageToEventSource()
        {
            eventSource.WriteNoise(type, message);
            Mock.Get(eventSource).Verify(_ => _.NoiseText(string.Empty, type, message));
        }

        [Fact]
        public void WriteNoiseWritesFormattedMessageToEventSource()
        {
            eventSource.WriteNoise(type, format, args);
            Mock.Get(eventSource).Verify(_ => _.NoiseText(string.Empty, type, FormattedMessage()));
        }

        [Fact]
        public void WriteNoiseWithIdWritesMessageToEventSource()
        {
            eventSource.WriteNoiseWithId(type, id, message);
            Mock.Get(eventSource).Verify(_ => _.NoiseText(id, type, message));
        }

        [Fact]
        public void WriteNoiseWithIdWritesFormattedMessageToEventSource()
        {
            eventSource.WriteNoiseWithId(type, id, format, args);
            Mock.Get(eventSource).Verify(_ => _.NoiseText(id, type, FormattedMessage()));
        }

        [Fact]
        public void WriteWarningWritesMessageToEventSource()
        {
            eventSource.WriteWarning(type, message);
            Mock.Get(eventSource).Verify(_ => _.WarningText(string.Empty, type, message));
        }

        [Fact]
        public void WriteWarningWritesFormattedMessageToEventSource()
        {
            eventSource.WriteWarning(type, format, args);
            Mock.Get(eventSource).Verify(_ => _.WarningText(string.Empty, type, FormattedMessage()));
        }

        [Fact]
        public void WriteWarningWithIdWritesMessageToEventSource()
        {
            eventSource.WriteWarningWithId(type, id, message);
            Mock.Get(eventSource).Verify(_ => _.WarningText(id, type, message));
        }

        [Fact]
        public void WriteWarningWithIdWritesFormattedMessageToEventSource()
        {
            eventSource.WriteWarningWithId(type, id, format, args);
            Mock.Get(eventSource).Verify(_ => _.WarningText(id, type, FormattedMessage()));
        }

        string FormattedMessage() =>
            string.Format(CultureInfo.InvariantCulture, format, args);

        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);
    }
}
