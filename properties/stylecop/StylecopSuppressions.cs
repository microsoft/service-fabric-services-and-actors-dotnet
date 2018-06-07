// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "StyleCop.CSharp.MaintainabilityRules",
    "SA1119:StatementMustNotUseUnnecessaryParenthesis",
    Justification = "The code is more redable, especially when using parenthesis for the return statements.")]

[assembly: SuppressMessage(
    "StyleCop.CSharp.ReadabilityRules",
    "SA1124:DoNotUseRegions",
    Justification = "In large files region allows creating logical sections in the file for better readability.")]

[assembly: SuppressMessage(
    "Style",
    "IDE1006:Naming Styles",
    Justification = "Prefer SA1307-SA1307AccessibleFieldsMustBeginWithUpperCaseLetter over IDE:1006.")]
