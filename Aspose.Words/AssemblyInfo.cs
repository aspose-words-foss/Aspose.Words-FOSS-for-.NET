// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/06/2010 by Roman Korchagin

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Aspose.Words;

//
// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//
// I need this because it is used for the COM type library name.
[assembly: AssemblyDescription(AssemblyConstants.Product)]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Aspose Pty Ltd")]
[assembly: AssemblyProduct(AssemblyConstants.Product)]
[assembly: AssemblyCopyright("© Aspose Pty Ltd 2001-2026. All Rights Reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Make compiler check for CLS compliance.
[assembly: CLSCompliant(true)]

// We rely on the "simple" approach and expose this assembly to COM "as-is".
[assembly: ComVisible(true)]
[module: SuppressMessage("Microsoft.Design", "CA1017:MarkAssembliesWithComVisible")]

// Aspose versioning standard is <major version>.<minor version>[.<hotfix version>], no leading zero.
// E.g. "17.4" for Normal planned release and 17.12.9 for Hot-fix release.
[assembly: AssemblyVersion(AssemblyConstants.Version)]

[assembly: AssemblyInformationalVersion(AssemblyConstants.Version)]

// SecurityPermission only needed for legacy .NET 3.5
#if NET35 && !NET40
[assembly: System.Security.AllowPartiallyTrustedCallers]
[assembly: System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.RequestMinimum, Execution = true)]
#endif

#pragma warning disable CS0618
// This makes sure a useful type library for all public members can be created.
[assembly: ClassInterface(ClassInterfaceType.AutoDual)]
#pragma warning restore CS0618


#if DEBUG
// SonarIgnoreStart
[assembly: InternalsVisibleTo("Aspose.Words.Tests")]
[assembly: InternalsVisibleTo("Aspose.Words.TestFx")]
// SonarIgnoreEnd
#endif


