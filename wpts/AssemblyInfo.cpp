#include "stdafx.h"

using namespace System;
using namespace System::Reflection;
using namespace System::Runtime::CompilerServices;
using namespace System::Runtime::InteropServices;
using namespace System::Security::Permissions;

//
// アセンブリに関する一般情報は以下の属性セットをとおして制御されます。
// アセンブリに関連付けられている情報を変更するには、
// これらの属性値を変更してください。
//
[assembly:AssemblyTitleAttribute("Wikipedia 翻訳支援ツール")];
[assembly:AssemblyDescriptionAttribute("Wikipediaでの言語間翻訳をサポートするためのツール")];
[assembly:AssemblyConfigurationAttribute("")];
[assembly:AssemblyCompanyAttribute("-")];
[assembly:AssemblyProductAttribute("Wikipedia 翻訳支援ツール")];
[assembly:AssemblyCopyrightAttribute("Copyright (c) Honeplus 2006")];
[assembly:AssemblyTrademarkAttribute("")];
[assembly:AssemblyCultureAttribute("")];

//
// アセンブリのバージョン情報は、以下の 4 つの値で構成されています:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// すべての値を指定するか、下のように '*' を使ってリビジョンおよびビルド番号を
// 既定値にすることができます:

[assembly:AssemblyVersionAttribute("0.72.*")];

[assembly:ComVisible(false)];

[assembly:CLSCompliantAttribute(true)];

[assembly:SecurityPermission(SecurityAction::RequestMinimum, UnmanagedCode = true)];
