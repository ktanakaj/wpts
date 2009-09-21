// Wikipediaの各種情報・設定などを管理するためのクラス
#include "stdafx.h"

using namespace wpts;

// WikipediaNamespace

/* 配列のソート用メソッド */
int WikipediaInformation::Namespace::CompareTo(Object ^obj)
{
	// 名前空間の番号でソート
	WikipediaInformation::Namespace ^ns = dynamic_cast<WikipediaInformation::Namespace^>(obj);
	return this->Key.CompareTo(ns->Key);
}


// WikipediaInformation

/* メンバ変数の初期値設定 */
void WikipediaInformation::setDefault(void)
{
	// メンバ変数の領域確保・初期設定
	// ※各初期値は2006年9月時点のWikipedia英語版より
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

/* 指定した言語での名称を 記事名|略称 の形式で取得 */
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

/* 指定された番号の名前空間を取得 */
String^ WikipediaInformation::GetNamespace(int i_Key)
{
	for each(WikipediaInformation::Namespace ^ns in Namespaces){
		if(ns->Key == i_Key){
			return ns->Name;
		}
	}
	return "";
}

/* 指定された文字列がWikipediaのシステム変数に相当かを判定 */
bool WikipediaInformation::ChkSystemVariable(String ^i_Text)
{
	String ^text = ((i_Text != nullptr) ? i_Text : "");
	// 基本は全文一致だが、定数が : で終わっている場合、textの:より前のみを比較
	// ※ {{ns:1}}みたいな場合に備えて
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
