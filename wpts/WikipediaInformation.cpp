// Wikipedia�̊e����E�ݒ�Ȃǂ��Ǘ����邽�߂̃N���X
#include "stdafx.h"

using namespace wpts;

// WikipediaNamespace

/* �z��̃\�[�g�p���\�b�h */
int WikipediaInformation::Namespace::CompareTo(Object ^obj)
{
	// ���O��Ԃ̔ԍ��Ń\�[�g
	WikipediaInformation::Namespace ^ns = dynamic_cast<WikipediaInformation::Namespace^>(obj);
	return this->Key.CompareTo(ns->Key);
}


// WikipediaInformation

/* �����o�ϐ��̏����l�ݒ� */
void WikipediaInformation::setDefault(void)
{
	// �����o�ϐ��̗̈�m�ہE�����ݒ�
	// ���e�����l��2006�N9�����_��Wikipedia�p��ł��
	Server = String::Format("{0}.wikipedia.org", Code);
	ArticleXmlPath = "wiki/Special:Export/";
	SystemVariables = gcnew array<String^>{
		"CURRENTMONTH",
		"CURRENTMONTHNAME",
		"CURRENTDAY",
		"CURRENTDAYNAME",
		"CURRENTYEAR",
		"CURRENTTIME",
		"NUMBEROFARTICLES",
		"SITENAME",
		"SERVER",
		"NAMESPACE",
		"PAGENAME",
		"ns:",
		"localurl:",
		"fullurl:",
		"#if:"};
	Bracket = " ({0}) ";
	Redirect = "#REDIRECT";
	Namespaces = gcnew array<Namespace>(0);
	TitleKeys = gcnew array<String^>(0);
}

/* �w�肵������ł̖��̂� �L����|���� �̌`���Ŏ擾 */
String ^WikipediaInformation::GetFullName(String ^i_Code)
{
	for each(LanguageName ^name in Names){
		if(name->Code == i_Code){
			if(name->ShortName != ""){
				return (name->Name + "|" + name->ShortName);
			}
			else{
				return name->Name;
			}
		}
	}
	return "";
}

/* �w�肳�ꂽ�ԍ��̖��O��Ԃ��擾 */
String^ WikipediaInformation::GetNamespace(int i_Key)
{
	for each(WikipediaInformation::Namespace ^ns in Namespaces){
		if(ns->Key == i_Key){
			return ns->Name;
		}
	}
	return "";
}

/* �w�肳�ꂽ������Wikipedia�̃V�X�e���ϐ��ɑ������𔻒� */
bool WikipediaInformation::ChkSystemVariable(String ^i_Text)
{
	String ^text = ((i_Text != nullptr) ? i_Text : "");
	// ��{�͑S����v�����A�萔�� : �ŏI����Ă���ꍇ�Atext��:���O�݂̂��r
	// �� {{ns:1}}�݂����ȏꍇ�ɔ�����
	for each(String ^variable in SystemVariables){
		if(variable->EndsWith(":") == true){
			if(text->StartsWith(variable) == true){
				return true;
			}
		}
		else if(text == variable){
			return true;
		}
	}
	return false;
}
