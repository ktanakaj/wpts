// Wikipediaの記事を管理するためのクラス
#ifndef WikipediaArticleH
#define WikipediaArticleH

#include "stdafx.h"

namespace wpts {

	using namespace System;
	using namespace System::IO;
	using namespace System::Net;
	using namespace System::Xml;
	using namespace System::Windows::Forms;

	// Wikipediaの記事の書式を扱うためのクラス
	ref class WikipediaFormat {
	public:
		// Wikipediaのリンクの要素を格納するための構造体
		value struct Link {
			String ^Text;				// リンクのテキスト（[[～]]）
			String ^Article;			// リンクの記事名
			String ^Section;			// リンクのセクション名（#）
			array<String^> ^PipeTexts;	// リンクのパイプ後の文字列（|）
			String ^Code;				// 言語間または他プロジェクトへのリンクの場合、コード
			bool TemplateFlag;			// テンプレート（{{～}}）かを示すフラグ
			bool SubPageFlag;			// 記事名の先頭が / で始まるかを示すフラグ
			bool StartColonFlag;		// リンクの先頭が : で始まるかを示すフラグ
			bool MsgnwFlag;				// テンプレートの場合、msgnw: が付加されているかを示すフラグ
			bool EnterFlag;				// テンプレートの場合、記事名の後で改行されるかを示すフラグ

			// 初期化
			void Initialize(void);
			// 現在のText以外の値から、Textを生成
			String^ MakeText(void);
		};

		// コンストラクタ（サーバーを指定）
		WikipediaFormat(WikipediaInformation^);
		// デストラクタ
		virtual ~WikipediaFormat(){}

		// 渡された記事名がカテゴリーかをチェック
		virtual bool IsCategory(String^);
		// 渡された記事名が画像かをチェック
		virtual bool IsImage(String^);
		// 渡された記事名が標準名前空間以外かをチェック
		virtual bool IsNotMainNamespace(String^);

		// 渡されたWikipediaの内部リンク・テンプレートを解析
		virtual Link ParseInnerLink(String^);
		virtual Link ParseTemplate(String^);
		// 渡されたテキストの指定された位置に存在するWikipediaの内部リンク・テンプレートをチェック
		int ChkLinkText(Link %, String^, int);

		// 渡されたテキストの指定された位置に存在する変数をチェック
		virtual int ChkVariable(String^ %, String^ %, String^, int);
		// 渡されたテキストの指定された位置からnowiki区間かのチェック
		static int ChkNowiki(String^ %, String^, int);
		// 渡されたテキストの指定された位置からコメント区間かのチェック
		static int ChkComment(String^ %, String^, int);

		// Wikipediaの固定値の書式
		static const String ^COMMENTSTART = "<!--";
		static const String ^COMMENTEND = "-->";
		static const String ^NOWIKISTART = "<nowiki>";
		static const String ^NOWIKIEND = "</nowiki>";
		static const String ^MSGNW = "msgnw:";

		// 記事が所属するサーバー情報
		property WikipediaInformation ^Server {
			WikipediaInformation^ get()
			{
				return _Server;
			}
		}

	protected:
		// 記事が所属するサーバー情報（property）
		WikipediaInformation ^_Server;
	};

	// Wikipediaの記事を管理するためのクラス
	ref class WikipediaArticle : WikipediaFormat {
	public:
		// コンストラクタ（サーバーと記事名を指定）
		WikipediaArticle(WikipediaInformation ^i_Server, String ^i_Name)
			: WikipediaFormat(i_Server){
			// 初期設定
			Initialize(i_Name);
		}
		// デストラクタ
		virtual ~WikipediaArticle(){}

		// 初期設定
		void Initialize(String^);

		// 記事をサーバーより取得
		// ※GetInterWiki(), GetNamespaces(), IsRedirect()、それに多くのpropertyは、
		//   事前にGetArticle()を実行しておく必要がある
		virtual bool GetArticle(String^, String^, TimeSpan);
		bool GetArticle(String^, String^);
		bool GetArticle(void);
		// 指定された言語コードへの言語間リンクを返す
		virtual String^ GetInterWiki(String^);
		// 記事のXMLから名前空間情報を取得
		virtual array<WikipediaInformation::Namespace>^ GetNamespaces(void);

		// 記事がリダイレクトかをチェック
		virtual bool IsRedirect(void);
		// 記事がカテゴリーかをチェック
		virtual bool IsCategory(void);
		// 記事が画像かをチェック
		virtual bool IsImage(void);
		// 記事が標準名前空間以外かをチェック
		bool IsNotMainNamespace(void);

		// 記事名
		property String ^Title {
			String^ get()
			{
				return _Title;
			}
		}
		// 記事のXMLデータのURL（property）
		property Uri ^Url {
			Uri^ get()
			{
				return _Url;
			}
		}
		// 記事のXMLデータ
		property XmlDocument ^Xml {
			XmlDocument^ get()
			{
				return _Xml;
			}
		}
		// 記事の最終更新日時（UTC）
		property DateTime Timestamp {
			DateTime get()
			{
				return _Timestamp;
			}
		}
		// 記事本文
		property String ^Text {
			String^ get()
			{
				return _Text;
			}
		}
		// リダイレクト先記事名
		property String ^Redirect {
			String^ get()
			{
				return _Redirect;
			}
		}
		// GetArticle実行時のHttpStatus
		property HttpStatusCode GetArticleStatus {
			HttpStatusCode get()
			{
				return _GetArticleStatus;
			}
		}
		// GetArticle例外発生時の例外情報
		// ※GetArticle()がfalseで、GetArticleStatusがNotFound以外のとき、設定される
		property Exception ^GetArticleException {
			Exception^ get()
			{
				return _GetArticleException;
			}
		}

		// WikipediaのXMLの固定値の書式
		const static String ^XMLNS = "http://www.mediawiki.org/xml/export-0.3/";

	protected:
		// 記事のXMLをサーバーより取得
		bool getServerArticle(String^, String^);
		// 記事のXMLをキャッシュより取得
		bool getCacheArticle(TimeSpan);
		// 記事本文の指定された位置に存在するWikipediaのLinkを解析
		virtual int chkLinkText(Link %, int);
		// 記事本文の指定された位置からがコメント区間かのチェック
		virtual int chkComment(String^ %, int);

		// 記事名（property）
		String ^_Title;
		// 記事のXMLデータのURL（property）
		Uri ^_Url;
		// 記事のXMLデータ（property）
		XmlDocument ^_Xml;
		// 記事の最終更新日時（UTC）（property）
		DateTime _Timestamp;
		// 記事本文（property）
		String ^_Text;
		// リダイレクト先記事名（property）
		String ^_Redirect;
		// GetArticle実行時のHttpStatus（property）
		HttpStatusCode _GetArticleStatus;
		// GetArticle例外発生時の例外情報（property）
		Exception ^_GetArticleException;
	};
}
#endif
