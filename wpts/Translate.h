// 翻訳支援処理の本体クラス（抽象）
#ifndef TranslateH
#define TranslateH

#include "stdafx.h"

namespace wpts {

	using namespace System;
	using namespace System::Net;

	// 翻訳支援処理を実装するための共通クラス
	ref class Translate abstract {
	public:
		// コンストラクタ
		Translate(LanguageInformation^, LanguageInformation^);
		// デストラクタ
		virtual ~Translate(){}

		// 翻訳支援処理実行
		virtual bool Run(String ^);

		// 処理を途中で終了させるためのフラグ
		bool CancellationPending;

		// ログ更新伝達イベント
		event EventHandler ^LogUpdate;
		// ログメッセージ
		property String ^Log {
			public:
				String^ get()
				{
					return _Log;
				}
			protected:
				void set(String ^i_Log)
				{
					_Log = ((i_Log != nullptr) ? i_Log : "");
					LogUpdate(this, EventArgs::Empty);
				}
		}

		// 変換後テキスト
		property String ^Text {
			public:
				String^ get()
				{
					return _Text;
				}
			protected:
				void set(String ^i_Text)
				{
					_Text = ((i_Text != nullptr) ? i_Text : "");
				}
		}

		// 改行コード
		static const String ^ENTER = "\r\n";

	protected:
		// 翻訳支援処理実行部の本体
		// ※継承クラスでは、この関数に処理を実装すること
		virtual bool runBody(String^) = 0;
		// 翻訳支援処理実行時の初期化処理
		void runInitialize(void);
		// ログメッセージを1行出力
		void logLine(String^);

		// 共通関数クラスのオブジェクト
		MYAPP::Cmn ^cmnAP;

		// 翻訳元／先言語の言語コード
		LanguageInformation ^source;
		LanguageInformation ^target;

	private:
		// ログメッセージ（property）
		String ^_Log;
		// 変換後テキスト（property）
		String ^_Text;
	};

	// Wikipedia用の翻訳支援処理実装クラス
	ref class TranslateNetworkObject abstract : public Translate {
	public:
		// コンストラクタ
		// ※
		TranslateNetworkObject(LanguageWithServerInformation ^i_Source, LanguageWithServerInformation ^i_Target)
			: Translate(i_Source, i_Target){}
		// デストラクタ
		virtual ~TranslateNetworkObject(){}

		// 翻訳支援処理実行
		virtual bool Run(String ^) override;

		// 通信時に使用するUserAgent
		String ^UserAgent;
		// 通信時に使用するReferer
		String ^Referer;

	private:
		// サーバー接続チェック
		bool ping(String^);
	};

}
#endif