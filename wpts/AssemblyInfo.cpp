#include "stdafx.h"

using namespace System;
using namespace System::Reflection;
using namespace System::Runtime::CompilerServices;
using namespace System::Runtime::InteropServices;
using namespace System::Security::Permissions;

//
// �A�Z���u���Ɋւ����ʏ��͈ȉ��̑����Z�b�g���Ƃ����Đ��䂳��܂��B
// �A�Z���u���Ɋ֘A�t�����Ă������ύX����ɂ́A
// �����̑����l��ύX���Ă��������B
//
[assembly:AssemblyTitleAttribute("Wikipedia �|��x���c�[��")];
[assembly:AssemblyDescriptionAttribute("Wikipedia�ł̌���Ԗ|����T�|�[�g���邽�߂̃c�[��")];
[assembly:AssemblyConfigurationAttribute("")];
[assembly:AssemblyCompanyAttribute("-")];
[assembly:AssemblyProductAttribute("Wikipedia �|��x���c�[��")];
[assembly:AssemblyCopyrightAttribute("Copyright (c) Honeplus 2006")];
[assembly:AssemblyTrademarkAttribute("")];
[assembly:AssemblyCultureAttribute("")];

//
// �A�Z���u���̃o�[�W�������́A�ȉ��� 4 �̒l�ō\������Ă��܂�:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// ���ׂĂ̒l���w�肷�邩�A���̂悤�� '*' ���g���ă��r�W��������уr���h�ԍ���
// ����l�ɂ��邱�Ƃ��ł��܂�:

[assembly:AssemblyVersionAttribute("0.72.*")];

[assembly:ComVisible(false)];

[assembly:CLSCompliantAttribute(true)];

[assembly:SecurityPermission(SecurityAction::RequestMinimum, UnmanagedCode = true)];
