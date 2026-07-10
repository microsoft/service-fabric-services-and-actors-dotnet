// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Xml;

namespace Microsoft.ServiceFabric.Services.Communication.Wcf.Runtime;

sealed class WcfGlobalErrorHandler(ChannelDispatcher dispatcher) : IErrorHandler
{
    readonly ChannelDispatcher dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
    static readonly DataContractSerializer serializer = new(typeof(ServiceExceptionData));

    bool IErrorHandler.HandleError(Exception error) => error is not FaultException;

    void IErrorHandler.ProvideFault(Exception error, MessageVersion version, ref Message fault)
    {
        if (error is null)
            throw new ArgumentNullException(nameof(error));

        if (error is FaultException)
            return;

        if (dispatcher.Listener.State != CommunicationState.Opened)
        {
            FaultException faultException = new(FaultReason(error), WcfRemoteExceptionInformation.FaultCodeRetry);
            MessageFault messageFault = faultException.CreateMessageFault();
            fault = Message.CreateMessage(version, messageFault, null);
        }
    }

    static FaultReason FaultReason(Exception exception)
    {
        var message = new StringBuilder()
            .AppendFormat(CultureInfo.CurrentCulture, Services.Wcf.SR.ErrorExceptionSerializationFailed1, exception.GetType().FullName)
            .AppendLine()
            .AppendFormat(CultureInfo.CurrentCulture, Services.Wcf.SR.ErrorExceptionSerializationFailed2, exception);

        var exceptionData = new ServiceExceptionData(exception.GetType().FullName, message.ToString());

        using var stringWriter = new StringWriter();
        using var textStream = XmlWriter.Create(stringWriter);
        serializer.WriteObject(textStream, exceptionData);
        textStream.Flush();
        return new FaultReason(stringWriter.ToString());
    }
}
