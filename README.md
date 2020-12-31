![Vanara](/docs/icons/VanaraHeading.png)

[![Version](https://img.shields.io/github/release/dahall/Vanara.svg?style=flat-square)](https://github.com/dahall/Vanara/releases) [![Downloads](https://img.shields.io/nuget/dt/Vanara.Core.svg?style=flat-square)](https://www.nuget.org/packages/Vanara.Core/) [![Build status](https://ci.appveyor.com/api/projects/status/p6jj1j3sbt95opdr?svg=true)](https://ci.appveyor.com/project/dahall/vanara)

This project contains various .NET assemblies that contain P/Invoke functions, interfaces, enums and structures from Windows libraries. Each assembly is associated with one or a few tightly related libraries. For example, Shlwapi.dll has all the exported functions from shlwapi.lib; Kernel32.dll has all for both kernel32.lib and kernelbase.lib.

All assemblies are available via NuGet and provide builds against .NET 2.0, 3.5, 4.0, 4.5, Core 3.0, Core 3.1 and **.NET 5.0** (new in v3.2.20) and support [SourceLink](https://docs.microsoft.com/en-us/dotnet/standard/library-guidance/sourcelink). In all cases where a dependency doesn't disallow it, .NET Standard 2.0, .NET Core 2.0, and 2.1 builds are also included for use with UWP and other .NET Core and Standard projects.

This project releases new versions every few weeks after sufficient testing. New releases are cataloged, along with release notes, in the [Releases](https://github.com/dahall/Vanara/releases) section and all NuGet packages are published to [nuget.org](https://www.nuget.org/packages?q=dahall+Vanara). Each GitHub push triggers an [AppVeyor](https://ci.appveyor.com/project/dahall/vanara) build. The owners thank them for the free Open-Source account! The status of that build is in the header of this page. The NuGet packages from those builds are available for test purposes on AppVeyor's project NuGet source at https://ci.appveyor.com/nuget/vanara-prerelease.

## Use
1. Look for the function you need in Microsoft documentation. Note which library or DLL the function is in.
2. Confirm the Vanara library exists and has your function by looking at the Supported Libraries table below. Clicking on the Assembly link will take you to a drill down of that assembly's coverage. Find your function and if there is a matching implementation it will appear to the right. You can also use GitHub's project search (upper left of page) to search for your function, method or constant. Make sure to select "In this repository".
3. Add the assembly to your project via NuGet.
4. To use the function, you can:
   1. Call it directly `var bret = Vanara.PInvoke.Kernel32.GetComputerName(sb, ref sbSz);`
   2. Under C# 6.0 and later, use a static using directive and call it:
   ```
   using static Vanara.PInvoke.Kernel32;
   
   var bret = GetComputerName(sb, ref sbSz);
   ```
5. In some cases there is a corresponding helper/wrapper class in one of the [Supporting Assemblies](#Supporting-Assemblies), especially for Security, System Services, Forms and Shell. Go to their library page (click on link in section) and look through the classes included in each library.

## Design Concepts

I have tried to follow the concepts below in laying out the libraries.
* All functions that are imported from a single DLL should be placed into a single assembly that is named after the DLL.
  * (e.g. The assembly `Vanara.PInvoke.Gdi32.dll` hosts all functions and supporting enumerations, constants and structures that are exported from `gdi32.dll` in the system directory.)
* Any structure or macro or enumeration (no function) that is used by many libraries is put into either `Vanara.Core` or `Vanara.PInvoke.Shared`.
  * (e.g. The macro `HIWORD` and the structure `SIZE` are both in `Vanara.PInvoke.Shared` and classes to simplify interop calls and native memory management are in `Vanara.Core`.)
* Inside a project, all constructs are contained in a file named after the header file (*.h) in which they are defined in the Windows API.
  * (e.g. In the `Vanara.PInvoke.Kernel32` project directory, you'll find a FileApi.cs, a WinBase.cs and a WinNT.cs file representing fileapi.h, winbase.h and winnt.h respectively.)
* Where the direct interpretation of a structure or function leads to memory leaks or misuse, I have tried to simplify its use.
* Where a structure is always passed by reference and where that structure needs to clean up memory allocations, I have changed the structure to a class implementing `IDisposable`.
* Wherever possible, all handles have been turned into `SafeHandle` derivatives named after the Windows API handle. If those handles require a call to a function to release/close/destroy, a derived `SafeHANDLE` exists that performs that function on disposal.
  * e.g. `HTOKEN` is defined. `SafeHTOKEN` builds upon that handle with an automated release calling `CloseHandle`.
* Wherever possible, all functions that allocate memory that is to be freed by the caller use a safe memory handle.
* All PInvoke calls are in assemblies prefixed by `Vanara.PInvoke`.
* If a structure is to passed into a function as a constant, that structure is marshaled using the `in` statement which will pass the structure by reference without requiring the `ref` keyword.
  * Windows API: `BOOL MapDialogRect(HWND hDlg, LPRECT lpRect)`
  * Vanara: `bool MapDialogRect(HWND hDlg, in RECT lpRect);`
* If there are classes or extensions that make use of the PInvoke calls, they are in wrapper assemblies prefixed by `Vanara` and then followed by a logical name for the functionality. Today, those are Core, Security, SystemServices, Windows.Forms and Windows.Shell.

## Supported Libraries

Library/DLL | Assembly | Coverage | NuGet&nbsp;Link&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
--- | --- | --- | ---
aclui.dll | [Vanara.PInvoke.AclUI](https://github.com/dahall/Vanara/blob/master/PInvoke/AclUI/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.AclUI?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.AclUI?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.AclUI)
advapi32.dll, secur32.dll, authz.dll, sspicli.dll, schannel.dll | [Vanara.PInvoke.Security](https://github.com/dahall/Vanara/blob/master/PInvoke/Security/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.Security?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.Security?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.Security)
cabinet.dll | [Vanara.PInvoke.Cabinet](https://github.com/dahall/Vanara/blob/master/PInvoke/Cabinet/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.Cabinet?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.Cabinet?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.Cabinet)
CldApi.dll | [Vanara.PInvoke.CldApi](https://github.com/dahall/Vanara/blob/master/PInvoke/CldApi/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.CldApi?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.CldApi?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.CldApi)
comctl32.dll | [Vanara.PInvoke.ComCtl32](https://github.com/dahall/Vanara/blob/master/PInvoke/ComCtl32/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.ComCtl32?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.ComCtl32?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.ComCtl32)
ComDlg32.dll | [Vanara.PInvoke.ComDlg32](https://github.com/dahall/Vanara/blob/master/PInvoke/ComDlg32/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.ComDlg32?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.ComDlg32?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.ComDlg32)
credui.dll | [Vanara.PInvoke.CredUI](https://github.com/dahall/Vanara/blob/master/PInvoke/CredUI/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.CredUI?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.CredUI?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.CredUI)
crypt32.dll, bcrypt.dll, ncrypt.dll, tokenbinding.dll, cryptui.dll, cryptnet.dll, cryptdlg.dll | [Vanara.PInvoke.Cryptography](https://github.com/dahall/Vanara/blob/master/PInvoke/Cryptography/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.Cryptography?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.Cryptography?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.Cryptography)
d2d1.dll, dxgi.dll, dwrite.dll, windowscodecs.dll | [Vanara.PInvoke.Graphics](https://github.com/dahall/Vanara/blob/master/PInvoke/Graphics/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.Graphics?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.Graphics?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.Graphics)
DbgHelp.dll, ImageHlp.dll | [Vanara.PInvoke.DbgHelp](https://github.com/dahall/Vanara/blob/master/PInvoke/DbgHelp/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.DbgHelp?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.DbgHelp?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.DbgHelp)
Dhcpcsvc6.dll, Dhcpcsvc.dll | [Vanara.PInvoke.Dhcp](https://github.com/dahall/Vanara/blob/master/PInvoke/Dhcp/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.Dhcp?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.Dhcp?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.Dhcp)
DnsApi.dll | [Vanara.PInvoke.DnsApi](https://github.com/dahall/Vanara/blob/master/PInvoke/DnsApi/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.DnsApi?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.DnsApi?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.DnsApi)
Drt.dll | [Vanara.PInvoke.Drt](https://github.com/dahall/Vanara/blob/master/PInvoke/Drt/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.Drt?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.Drt?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.Drt)
dwmapi.dll | [Vanara.PInvoke.DwmApi](https://github.com/dahall/Vanara/blob/master/PInvoke/DwmApi/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.DwmApi?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.DwmApi?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.DwmApi)
FirewallApi.dll | [Vanara.PInvoke.FirewallApi](https://github.com/dahall/Vanara/blob/master/PInvoke/FirewallApi/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.FirewallApi?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.FirewallApi?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.FirewallApi)
gdi32.dll | [Vanara.PInvoke.Gdi32](https://github.com/dahall/Vanara/blob/master/PInvoke/Gdi32/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.Gdi32?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.Gdi32?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.Gdi32)
iphlpapi.dll | [Vanara.PInvoke.IpHlpApi](https://github.com/dahall/Vanara/blob/master/PInvoke/IpHlpApi/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.IpHlpApi?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.IpHlpApi?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.IpHlpApi)
kernel32.dll, kernelbase.dll, normaliz.dll, vertdll.dll | [Vanara.PInvoke.Kernel32](https://github.com/dahall/Vanara/blob/master/PInvoke/Kernel32/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.Kernel32?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.Kernel32?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.Kernel32)
ktmw32.dll | [Vanara.PInvoke.KtmW32](https://github.com/dahall/Vanara/blob/master/PInvoke/KtmW32/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.KtmW32?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.KtmW32?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.KtmW32)
magnification.dll | [Vanara.PInvoke.Magnification](https://github.com/dahall/Vanara/blob/master/PInvoke/Magnification/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.Magnification?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.Magnification?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.Magnification)
mpr.dll | [Vanara.PInvoke.Mpr](https://github.com/dahall/Vanara/blob/master/PInvoke/Mpr/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.Mpr?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.Mpr?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.Mpr)
Msi.dll | [Vanara.PInvoke.Msi](https://github.com/dahall/Vanara/blob/master/PInvoke/Msi/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/2%25-red.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.Msi?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.Msi?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.Msi)
netapi32.dll | [Vanara.PInvoke.NetApi32](https://github.com/dahall/Vanara/blob/master/PInvoke/NetApi32/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.NetApi32?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.NetApi32?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.NetApi32)
netprofm.dll | [Vanara.PInvoke.NetListMgr](https://github.com/dahall/Vanara/blob/master/PInvoke/NetListMgr/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.NetListMgr?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.NetListMgr?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.NetListMgr)
NewDev.dll | [Vanara.PInvoke.NewDev](https://github.com/dahall/Vanara/blob/master/PInvoke/NewDev/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.NewDev?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.NewDev?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.NewDev)
ntdll.dll | [Vanara.PInvoke.NTDll](https://github.com/dahall/Vanara/blob/master/PInvoke/NtDll/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/4%25-red.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.NTDll?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.NTDll?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.NTDll)
ntdsapi.dll | [Vanara.PInvoke.NTDSApi](https://github.com/dahall/Vanara/blob/master/PInvoke/NTDSApi/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.NTDSApi?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.NTDSApi?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.NTDSApi)
ole32.dll, oleaut32.dll, propsys.dll | [Vanara.PInvoke.Ole](https://github.com/dahall/Vanara/blob/master/PInvoke/Ole/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.Ole?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.Ole?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.Ole)
oleacc.dll | [Vanara.PInvoke.Accessibility](https://github.com/dahall/Vanara/blob/master/PInvoke/Accessibility/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.Accessibility?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.Accessibility?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.Accessibility)
OleDlg.dll | [Vanara.PInvoke.OleDlg](https://github.com/dahall/Vanara/blob/master/PInvoke/OleDlg/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.OleDlg?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.OleDlg?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.OleDlg)
opcservices.dll | [Vanara.PInvoke.Opc](https://github.com/dahall/Vanara/blob/master/PInvoke/Opc/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.Opc?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.Opc?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.Opc)
P2P.dll | [Vanara.PInvoke.P2P](https://github.com/dahall/Vanara/blob/master/PInvoke/P2P/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.P2P?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.P2P?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.P2P)
pdh.dll | [Vanara.PInvoke.Pdh](https://github.com/dahall/Vanara/blob/master/PInvoke/Pdh/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.Pdh?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.Pdh?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.Pdh)
PeerDist.dll | [Vanara.PInvoke.PeerDist](https://github.com/dahall/Vanara/blob/master/PInvoke/PeerDist/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.PeerDist?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.PeerDist?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.PeerDist)
powrprof.dll | [Vanara.PInvoke.PowrProf](https://github.com/dahall/Vanara/blob/master/PInvoke/PowrProf/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.PowrProf?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.PowrProf?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.PowrProf)
ProjectedFSLib.dll | [Vanara.PInvoke.ProjectedFSLib](https://github.com/dahall/Vanara/blob/master/PInvoke/ProjectedFSLib/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.ProjectedFSLib?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.ProjectedFSLib?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.ProjectedFSLib)
qmgr.dll | [Vanara.PInvoke.BITS](https://github.com/dahall/Vanara/blob/master/PInvoke/BITS/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.BITS?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.BITS?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.BITS)
rpcrt4.dll | [Vanara.PInvoke.Rpc](https://github.com/dahall/Vanara/blob/master/PInvoke/Rpc/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/5%25-red.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.Rpc?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.Rpc?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.Rpc)
SearchApi | [Vanara.PInvoke.SearchApi](https://github.com/dahall/Vanara/blob/master/PInvoke/SearchApi/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.SearchApi?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.SearchApi?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.SearchApi)
SetupAPI.dll | [Vanara.PInvoke.SetupAPI](https://github.com/dahall/Vanara/blob/master/PInvoke/SetupAPI/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/5%25-red.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.SetupAPI?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.SetupAPI?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.SetupAPI)
shell32.dll, url.dll | [Vanara.PInvoke.Shell32](https://github.com/dahall/Vanara/blob/master/PInvoke/Shell32/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.Shell32?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.Shell32?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.Shell32)
shlwapi.dll | [Vanara.PInvoke.ShlwApi](https://github.com/dahall/Vanara/blob/master/PInvoke/ShlwApi/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.ShlwApi?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.ShlwApi?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.ShlwApi)
taskschd.dll, mstask.dll | [Vanara.PInvoke.TaskSchd](https://github.com/dahall/Vanara/blob/master/PInvoke/TaskSchd/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.TaskSchd?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.TaskSchd?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.TaskSchd)
UrlMon.dll | [Vanara.PInvoke.UrlMon](https://github.com/dahall/Vanara/blob/master/PInvoke/UrlMon/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.UrlMon?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.UrlMon?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.UrlMon)
user32.dll | [Vanara.PInvoke.User32](https://github.com/dahall/Vanara/blob/master/PInvoke/User32/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.User32?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.User32?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.User32)
UserEnv.dll | [Vanara.PInvoke.UserEnv](https://github.com/dahall/Vanara/blob/master/PInvoke/UserEnv/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.UserEnv?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.UserEnv?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.UserEnv)
uxtheme.dll | [Vanara.PInvoke.UxTheme](https://github.com/dahall/Vanara/blob/master/PInvoke/UxTheme/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.UxTheme?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.UxTheme?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.UxTheme)
virtdisk.dll | [Vanara.PInvoke.VirtDisk](https://github.com/dahall/Vanara/blob/master/PInvoke/VirtDisk/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.VirtDisk?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.VirtDisk?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.VirtDisk)
WcmApi.dll | [Vanara.PInvoke.WcmApi](https://github.com/dahall/Vanara/blob/master/PInvoke/WcmApi/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.WcmApi?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.WcmApi?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.WcmApi)
wer.dll | [Vanara.PInvoke.Wer](https://github.com/dahall/Vanara/blob/master/PInvoke/Wer/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.Wer?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.Wer?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.Wer)
wininet.dll | [Vanara.PInvoke.WinINet](https://github.com/dahall/Vanara/blob/master/PInvoke/WinINet/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.WinINet?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.WinINet?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.WinINet)
winspool.drv, prntvpt.dll | [Vanara.PInvoke.Printing](https://github.com/dahall/Vanara/blob/master/PInvoke/Printing/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.Printing?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.Printing?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.Printing)
wintrust.dll | [Vanara.PInvoke.WinTrust](https://github.com/dahall/Vanara/blob/master/PInvoke/WinTrust/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.WinTrust?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.WinTrust?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.WinTrust)
WlanApi.dll, Wlanui.dll | [Vanara.PInvoke.WlanApi](https://github.com/dahall/Vanara/blob/master/PInvoke/WlanApi/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.WlanApi?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.WlanApi?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.WlanApi)
ws2_32.dll | [Vanara.PInvoke.Ws2_32](https://github.com/dahall/Vanara/blob/master/PInvoke/Ws2_32/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.Ws2_32?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.Ws2_32?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.Ws2_32)
WTSApi32.dll | [Vanara.PInvoke.WTSApi32](https://github.com/dahall/Vanara/blob/master/PInvoke/WTSApi32/CorrelationReport.md) | ![Coverage](https://img.shields.io/badge/100%25-green.svg?style=flat-square) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.WTSApi32?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.WTSApi32?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.WTSApi32)

## Supporting Assemblies

Assembly | &nbsp;&nbsp;&nbsp;NuGet&nbsp;Link&nbsp;&nbsp;&nbsp; | Description
--- | --- | --- 
[Vanara.BITS](https://github.com/dahall/Vanara/blob/master/BITS/AssemblyReport.md) | [![Nuget](https://img.shields.io/nuget/v/Vanara.BITS?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.BITS?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.BITS) | .NET classes to access Background Intelligent Transfer Service (BITS) functionality. Intelligently uses most recent library functions and gracefully fails when new features are not available on older OS versions.
[Vanara.Core](https://github.com/dahall/Vanara/blob/master/Core/AssemblyReport.md) | [![Nuget](https://img.shields.io/nuget/v/Vanara.Core?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.Core?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.Core) | This library includes shared methods, structures and constants for use throughout the Vanara assemblies. Think of it as windows.h with some useful extensions. It includes:<br>* Extension methods for working with enumerated types (enum), FILETIME, and method and property extractions via reflection<br>* Extension and helper methods to marshaling structures arrays and strings<br>* SafeHandle based classes for working with memory allocated via CoTaskMem, HGlobal, or Local calls that handles packing and extracting arrays, structures and raw memory<br>* Safe pinning of objects in memory<br>* Memory stream based on marshaled memory
[Vanara.PInvoke.Shared](https://github.com/dahall/Vanara/blob/master/PInvoke/Shared/AssemblyReport.md) | [![Nuget](https://img.shields.io/nuget/v/Vanara.PInvoke.Shared?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.PInvoke.Shared?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.PInvoke.Shared) | Shared methods, structures and constants for use throughout the Vanara.PInvoke assemblies. Includes:<br>* IEnumerable helpers for COM enumerations<br>* Custom marshaler for CoTaskMem pointers<br>* Enhanced error results classes for HRESULT, Win32Error and NTStatus<br>* Standard windows.h macros (e.g. HIWORD, MAKELONG, etc.)<br>* Overlapped method wrapper<br>* Resource ID holder<br>* Shared structures and enums (see release notes)
[Vanara.Security](https://github.com/dahall/Vanara/blob/master/Security/AssemblyReport.md) | [![Nuget](https://img.shields.io/nuget/v/Vanara.Security?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.Security?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.Security) | Classes for security related items derived from the Vanara PInvoke libraries. Includes extension methods for Active Directory and access control classes, methods for working with accounts, UAC, privileges, system access, impersonation and SIDs, and a full LSA wrapper.
[Vanara.SystemServices](https://github.com/dahall/Vanara/blob/master/System/AssemblyReport.md) | [![Nuget](https://img.shields.io/nuget/v/Vanara.SystemServices?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.SystemServices?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.SystemServices) | Classes for system related items derived from the Vanara PInvoke libraries. Includes extensions for Process (privileges and elavation), FileInfo (compression info), Shared Network Drives and Devices, and ServiceController (SetStartType) that pull extended information through native API calls.
[Vanara.VirtualDisk](https://github.com/dahall/Vanara/blob/master/VirtualDisk/AssemblyReport.md) | [![Nuget](https://img.shields.io/nuget/v/Vanara.VirtualDisk?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.VirtualDisk?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.VirtualDisk) | .NET classes to manage Windows Virtual Storage (VHD and VHDX) using P/Invoke functions from VirtDisk.dll.
[Vanara.Windows.Forms](https://github.com/dahall/Vanara/blob/master/Windows.Forms/AssemblyReport.md) | [![Nuget](https://img.shields.io/nuget/v/Vanara.Windows.Forms?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.Windows.Forms?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.Windows.Forms) | Classes for user interface related items derived from the Vanara PInvoke libraries. Includes extensions for almost all common controls to give post Vista capabilities, WinForms controls (panel, commandlink, enhanced combo boxes, IPAddress, split button, trackbar and themed controls), shutdown/restart/lock control, buffered painting, resource files, access control editor, simplifed designer framework for Windows.Forms.
[Vanara.Windows.Shell](https://github.com/dahall/Vanara/blob/master/Windows.Shell/AssemblyReport.md) | [![Nuget](https://img.shields.io/nuget/v/Vanara.Windows.Shell?label=%20&logo=nuget&style=flat-square)![Nuget](https://img.shields.io/nuget/dt/Vanara.Windows.Shell?label=%20&style=flat-square)](https://www.nuget.org/packages/Vanara.Windows.Shell) | Classes for Windows Shell items derived from the Vanara PInvoke libraries. Includes shell items, files, icons, links, and taskbar lists.

## Quick Links
* [Documentation](https://github.com/dahall/Vanara/wiki)
* [Issues](https://github.com/dahall/Vanara/issues)

## Sample Code
There are numerous examples in the [UnitTest](https://github.com/dahall/Vanara/tree/master/UnitTests) folder and in the [WinClassicSamplesCS](https://github.com/dahall/WinClassicSamplesCS) project that recreates the Windows Samples in C# using Vanara.
