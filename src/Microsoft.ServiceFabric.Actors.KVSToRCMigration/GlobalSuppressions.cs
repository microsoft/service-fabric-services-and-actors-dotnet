// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

// This assembly uses Migration APIs which are deprecated but the entire assembly is also deprecated
[assembly: SuppressMessage("Microsoft.Design", "CS0618:Type or member is obsolete", Scope = "namespaceanddescendants", Target = "Microsoft.ServiceFabric.Actors.KVSToRCMigration")]