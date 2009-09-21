// 翻訳支援処理の本体クラス、Wikipedia用
#ifndef TranslateWikipediaH
#define TranslateWikipediaH

#include "stdafx.h"
#include "Translate.h"
#include "WikipediaArticle.h"

namespace wpts {

	using namespace System;
	using namespace System::Windows::Forms;

	// Wikipedia用の翻訳支援処理実装クラス
	ref class TranslateWikipedia : TranslateNetworkObject {
	public:
		// コンストラクタ
		TranslateWikipedia(WikipediaInformation ^i_Source, WikipediaInformation ^i_Target)
			: TranslateNetworkObject(i_Source, i_Target){}
		// デストラクタ
		virtual ~TranslateWikipedia(){}

	protected:
		// 翻訳支援処理実行部の本体
		virtual bool runBody(String^) override;

		// 翻訳支援対象の記事を確認・取得
		WikipediaArticle^ chkTargetArticle(String^);
		// 指定された記事を取得、言語間リンクを確認、返す
		virtual String^ getInterWiki(String^, bool);
		String^ getInterWiki(String^);
		// 渡されたテキストを解析し、言語間リンク・見出し等の変換を行う
		String^ replaceText(String^, String^, bool);
		String^ replaceText(String^, String^);
		// リンクの解析・置換を行う
		int replaceLink(String^ %, String^, int, String^);
		// 内部リンクの文字列を変換する
		String^ replaceInnerLink(WikipediaFormat::Link, String^);
		// テンプレートの文字列を変換する
		String^ replaceTemplate(WikipediaFormat::Link, String^);
		// 指定されたインデックスの位置に存在する見出しを解析し、可能であれば変換して返す
		virtual int chkTitleLine(String^ %, String^, int);

		// 翻訳元コードでの定型句に相当する、翻訳先言語での定型句を取得
		virtual String^ getKeyWord(String^);
	};
}
#endif