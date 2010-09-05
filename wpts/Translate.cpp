// 翻訳支援処理の本体クラス
#include "stdafx.h"
#include "Translate.h"

using namespace wpts;

// Translate

/* コンストラクタ */
Translate::Translate(LanguageInformation ^i_Source, LanguageInformation ^i_Target){
	// ※必須な情報が設定されていない場合、ArgumentNullExceptionを返す
	if(i_Source == nullptr){
		throw gcnew ArgumentNullException("i_Source");
	}
	else if(i_Target == nullptr){
		throw gcnew ArgumentNullException("i_Target");
	}
	// メンバ変数の初期化
	cmnAP = gcnew MYAPP::Cmn();
	source = i_Source;
	target = i_Target;
	runInitialize();
}

/* 翻訳支援処理実行 */
bool Translate::Run(String ^i_Name)
{
	// 変数を初期化
	runInitialize();
	// 翻訳支援処理実行部の本体を実行
	// ※以降の処理は、継承クラスにて定義
	return runBody(i_Name);
}

/* 翻訳支援処理実行時の初期化処理 */
void Translate::runInitialize(void)
{
	// 変数を初期化
	_Log = "";
	Text = "";
	CancellationPending = false;
}

/* ログメッセージを1行追加出力 */
void Translate::logLine(String ^i_Log)
{
	// 直前のログが改行されていない場合、改行して出力
	if(Log != "" && Log->EndsWith((String^)ENTER) == false){
		Log += (ENTER + i_Log + ENTER);
	}
	else{
		Log += (i_Log + ENTER);
	}
}


// TranslateNetworkObject

/* 翻訳支援処理実行 */
bool TranslateNetworkObject::Run(String ^i_Name)
{
	// 変数を初期化
	runInitialize();
	// サーバー接続チェック
	if(ping(static_cast<LanguageWithServerInformation^>(source)->Server) == false){
		return false;
	}
	// 翻訳支援処理実行部の本体を実行
	// ※以降の処理は、継承クラスにて定義
	return runBody(i_Name);
}

/* サーバー接続チェック */
bool TranslateNetworkObject::ping(String ^i_Server)
{
	// サーバー接続チェック
	NetworkInformation::Ping ^ping = gcnew NetworkInformation::Ping();
	try{
		NetworkInformation::PingReply ^reply = ping->Send(i_Server);
		if(reply->Status != NetworkInformation::IPStatus::Success){
			logLine(String::Format(cmnAP->Resource->GetString("ErrorMessage_MissNetworkAccess"), reply->Status.ToString()));
			return false;
		}
	}
	catch(Exception ^e){
		logLine(String::Format(cmnAP->Resource->GetString("ErrorMessage_MissNetworkAccess"), e->InnerException->Message));
		return false;
	}
	return true;
}
