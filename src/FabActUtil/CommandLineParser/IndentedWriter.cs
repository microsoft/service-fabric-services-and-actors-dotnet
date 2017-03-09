// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace FabActUtil.CommandLineParser
{
    using System;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Stream which writes at different indents.
    /// </summary>
    public class IndentedWriter : StreamWriter
    {
        /// <summary>
        /// Create a new Indented Writer.
        /// </summary>
        /// <param name="stream"></param>
        public IndentedWriter(Stream stream)
            : base(stream)
        {
            this.SetNewLine();
        }

        /// <summary>
        /// The current number of spaces to indent.
        /// </summary>
        public int Indent
        {
            get { return this.indent; }
            set
            {
                this.indent = value;
                this.SetNewLine();
            }
        }

        private void SetNewLine()
        {
            var s = new StringBuilder();
            s.Append(CommandLineUtility.NewLine);
            s.Append(' ', this.indent);
            this.NewLine = s.ToString();
        }

        private int indent;
    }

    /// <summary>
    /// 
    /// </summary>
    public struct Indent : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="indent"></param>
        public Indent(IndentedWriter writer, int indent)
        {
            this.writer = writer;
            this.indent = indent;

            writer.Indent += indent;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.writer.Indent -= this.indent;
            this.writer = null;
        }

        private IndentedWriter writer;
        private readonly int indent;
    }
}