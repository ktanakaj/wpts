// Wikipediaの記事を管理するためのクラス
#include "stdafx.h"
#include "WikipediaArticle.h"

using namespace wpts;

// WikipediaArticle::Link

/* 初期化 */
void WikipediaFormat::Link::Initialize(void)
{
	// コンストラクタの代わりに、必要ならこれで初期化
	Text = nullptr;
	Article = nullptr;
	Section = nullptr;
	PipeTexts = gcnew array<String^>(0);
	Code = nullptr;
	TemplateFlag = false;
	SubPageFlag = false;
	StartColonFlag = false;
	MsgnwFlag = false;
	EnterFlag = false;
}

/* 現在のText以外の値から、Textを生成 */
String^ WikipediaFormat::Link::MakeText(void)
{
	// 戻り値初期化
	String ^result = "";
	// 枠の設定
	String ^startSign = "[[";
	String ^endSign = "]]";
	if(TemplateFlag){
		startSign = "{{";
		endSign = "}}";
	}
	// 先頭の枠の付加
	result += startSign;
	// 先頭の : の付加
	if(StartColonFlag){
		result += ":";
	}
	// msgnw: （テンプレートを<nowiki>タグで挟む）の付加
	if(TemplateFlag && MsgnwFlag){
		result += const_cast<String^>(MSGNW);
	}
	// 言語コード・他プロジェクトコードの付加
	if(!String::IsNullOrEmpty(Code)){
		result += Code;
	}
	// リンクの付加
	if(!String::IsNullOrEmpty(Article)){
		result += Article;
	}
	// セクション名の付加
	if(!String::IsNullOrEmpty(Section)){
		result += ("#" + Section);
	}
	// 改行の付加
	if(EnterFlag){
		result += '\n';
	}
	// パイプ後の文字列の付加
	if(PipeTexts != nullptr){
		for each(String ^text in PipeTexts){
			result += "|";
			if(!String::IsNullOrEmpty(text)){
				result += text;
			}
		}
	}
	// 終わりの枠の付加
	result += endSign;
	return result;
}


// WikipediaFormat

/* コンストラクタ（サーバーを指定） */
WikipediaFormat::WikipediaFormat(WikipediaInformation ^i_Server)
{
	// ※必須な情報が設定されていない場合、ArgumentNullExceptionを返す
	if(i_Server == nullptr){
		throw gcnew ArgumentNullException("i_Server");
	}
	// メンバ変数の初期化
	_Server = i_Server;
}

/* 渡された記事名がカテゴリーかをチェック */
bool WikipediaFormat::IsCategory(String ^i_Name)
{
	// 指定された記事名がカテゴリー（Category:等で始まる）かをチェック
	String ^category = Server->GetNamespace(Server->CATEGORYNAMESPACENUMBER);
	if(category != ""){
		if(i_Name->ToLower()->StartsWith(category->ToLower() + ":") == true){
			return true;
		}
	}
	return false;
}

/* 渡された記事名が画像かをチェック */
bool WikipediaFormat::IsImage(String ^i_Name)
{
	// 指定された記事名が画像（Image:等で始まる）かをチェック
	// ※日本語版みたいに、image: と言語固有の 画像: みたいなのがあると思われるので、
	//   翻訳元言語と英語版の設定でチェック
	for(int i = 0 ; i < 2 ; i++){
		String ^image = Server->GetNamespace(Server->IMAGENAMESPACENUMBER);
		if(i == 1){
			if(Server->Code == "en"){
				continue;
			}
			WikipediaInformation en("en");
			image = en.GetNamespace(en.IMAGENAMESPACENUMBER);
		}
		if(image != ""){
			if(i_Name->ToLower()->StartsWith(image->ToLower() + ":") == true){
				return true;
			}
		}
	}
	return false;
}

/* 渡された記事名が標準名前空間以外かをチェック */
bool WikipediaFormat::IsNotMainNamespace(String ^i_Name)
{
	// 指定された記事名が標準名前空間以外の名前空間（Wikipedia:等で始まる）かをチェック
	for each(WikipediaInformation::Namespace ns in Server->Namespaces){
		if(i_Name->ToLower()->StartsWith(ns.Name->ToLower() + ":") == true){
			return true;
		}
	}
	return false;
}

/* 渡されたWikipediaの内部リンクを解析 */
WikipediaFormat::Link WikipediaFormat::ParseInnerLink(String ^i_Text)
{
	// 出力値初期化
	Link result;
	result.Initialize();
	// 入力値確認
	if(i_Text->StartsWith("[[") == false){
		return result;
	}

	// 構文を解析して、[[]]内部の文字列を取得
	// ※構文はWikipediaのプレビューで色々試して確認、足りなかったり間違ってたりするかも・・・
	String ^article = "";
	String ^section = "";
	array<String^> ^pipeTexts = gcnew array<String^>(0);
	int lastIndex = -1;
	int pipeCounter = 0;
	bool sharpFlag = false;
	for(int i = 2 ; i < i_Text->Length ; i++){
		wchar_t c = i_Text[i];
		// ]]が見つかったら、処理正常終了
		if(MYAPP::Cmn::ChkTextInnerWith(i_Text, i, "]]") == true){
			lastIndex = ++i;
			break;
		}
		// | が含まれている場合、以降の文字列は表示名などとして扱う
		if(c == '|'){
			++pipeCounter;
			MYAPP::Cmn::AddArray(pipeTexts, "");
			continue;
		}
		// 変数（[[{{{1}}}]]とか）の再帰チェック
		String ^dummy = "";
		String ^variable = "";
		int index = ChkVariable(variable, dummy, i_Text, i);
		if(index != -1){
			i = index;
			if(pipeCounter > 0){
				pipeTexts[pipeCounter - 1] += variable;
			}
			else if(sharpFlag){
				section += variable;
			}
			else{
				article += variable;
			}
			continue;
		}

		// | の前のとき
		if(pipeCounter <= 0){
			// 変数以外で { } または < > [ ] \n が含まれている場合、リンクは無効
			if((c == '<') || (c == '>') || (c == '[') || (c == ']') || (c == '{') || (c == '}') || (c == '\n')){
				break;
			}

			// # の前のとき
			if(!sharpFlag){
				// #が含まれている場合、以降の文字列は見出しへのリンクとして扱う（1つめの#のみ有効）
				if(c == '#'){
					sharpFlag = true;
				}
				else{
					article += c;
				}
			}
			// # の後のとき
			else{
				section += c;
			}
		}
		// | の後のとき
		else{
			// コメント（<!--）が含まれている場合、リンクは無効
			if(MYAPP::Cmn::ChkTextInnerWith(i_Text, i, const_cast<String^>(COMMENTSTART)) == true){
				break;
			}
			// nowikiのチェック
			String ^nowiki = "";
			index = ChkNowiki(nowiki, i_Text, i);
			if(index != -1){
				i = index;
				pipeTexts[pipeCounter - 1] += nowiki;
				continue;
			}
			// リンク [[ {{ （[[image:xx|[[test]]の画像]]とか）の再帰チェック
			Link link;
			index = ChkLinkText(link, i_Text, i);
			if(index != -1){
				i = index;
				pipeTexts[pipeCounter - 1] += link.Text;
				continue;
			}
			pipeTexts[pipeCounter - 1] += c;
		}
	}
	// 解析に成功した場合、結果を戻り値に設定
	if(lastIndex != -1){
		// 変数ブロックの文字列をリンクのテキストに設定
		result.Text = i_Text->Substring(0, lastIndex + 1);
		// 前後のスペースは削除（見出しは後ろのみ）
		result.Article = article->Trim();
		result.Section = section->TrimEnd();
		// | 以降はそのまま設定
		result.PipeTexts = pipeTexts;
		// 記事名から情報を抽出
		// サブページ
		if(result.Article->StartsWith("/") == true){
			result.SubPageFlag = true;
		}
		// 先頭が :
		else if(result.Article->StartsWith(":")){
			result.StartColonFlag = true;
			result.Article = result.Article->TrimStart(':')->TrimStart();
		}
		// 標準名前空間以外で[[xxx:yyy]]のようになっている場合、言語コード
		if(result.Article->Contains(":") == true && !IsNotMainNamespace(result.Article)){
			// ※本当は、言語コード等の一覧を作り、其処と一致するものを・・・とすべきだろうが、
			//   メンテしきれないので : を含む名前空間以外を全て言語コード等と判定
			result.Code = result.Article->Substring(0, result.Article->IndexOf(':'))->TrimEnd();
			result.Article = result.Article->Substring(result.Article->IndexOf(':') + 1)->TrimStart();
		}
	}
	return result;
}

/* 渡されたWikipediaのテンプレートを解析 */
WikipediaFormat::Link WikipediaFormat::ParseTemplate(String ^i_Text)
{
	// 出力値初期化
	Link result;
	result.Initialize();
	result.TemplateFlag = true;
	// 入力値確認
	if(i_Text->StartsWith("{{") == false){
		return result;
	}

	// 構文を解析して、{{}}内部の文字列を取得
	// ※構文はWikipediaのプレビューで色々試して確認、足りなかったり間違ってたりするかも・・・
	String ^article = "";
	array<String^> ^pipeTexts = gcnew array<String^>(0);
	int lastIndex = -1;
	int pipeCounter = 0;
	for(int i = 2 ; i < i_Text->Length ; i++){
		wchar_t c = i_Text[i];
		// }}が見つかったら、処理正常終了
		if(MYAPP::Cmn::ChkTextInnerWith(i_Text, i, "}}") == true){
			lastIndex = ++i;
			break;
		}
		// | が含まれている場合、以降の文字列は引数などとして扱う
		if(c == '|'){
			++pipeCounter;
			MYAPP::Cmn::AddArray(pipeTexts, "");
			continue;
		}
		// 変数（[[{{{1}}}]]とか）の再帰チェック
		String ^dummy = "";
		String ^variable = "";
		int index = ChkVariable(variable, dummy, i_Text, i);
		if(index != -1){
			i = index;
			if(pipeCounter > 0){
				pipeTexts[pipeCounter - 1] += variable;
			}
			else{
				article += variable;
			}
			continue;
		}

		// | の前のとき
		if(pipeCounter <= 0){
			// 変数以外で < > [ ] { } が含まれている場合、リンクは無効
			if((c == '<') || (c == '>') || (c == '[') || (c == ']') || (c == '{') || (c == '}')){
				break;
			}
			article += c;
		}
		// | の後のとき
		else{
			// コメント（<!--）が含まれている場合、リンクは無効
			if(MYAPP::Cmn::ChkTextInnerWith(i_Text, i, const_cast<String^>(COMMENTSTART)) == true){
				break;
			}
			// nowikiのチェック
			String ^nowiki = "";
			index = ChkNowiki(nowiki, i_Text, i);
			if(index != -1){
				i = index;
				pipeTexts[pipeCounter - 1] += nowiki;
				continue;
			}
			// リンク [[ {{ （{{test|[[例]]}}とか）の再帰チェック
			Link link;
			index = ChkLinkText(link, i_Text, i);
			if(index != -1){
				i = index;
				pipeTexts[pipeCounter - 1] += link.Text;
				continue;
			}
			pipeTexts[pipeCounter - 1] += c;
		}
	}
	// 解析に成功した場合、結果を戻り値に設定
	if(lastIndex != -1){
		// 変数ブロックの文字列をリンクのテキストに設定
		result.Text = i_Text->Substring(0, lastIndex + 1);
		// 前後のスペース・改行は削除（見出しは後ろのみ）
		result.Article = article->Trim();
		// | 以降はそのまま設定
		result.PipeTexts = pipeTexts;
		// 記事名から情報を抽出
		// サブページ
		if(result.Article->StartsWith("/") == true){
			result.SubPageFlag = true;
		}
		// 先頭が :
		else if(result.Article->StartsWith(":")){
			result.StartColonFlag = true;
			result.Article = result.Article->TrimStart(':')->TrimStart();
		}
		// 先頭が msgnw:
		result.MsgnwFlag = result.Article->ToLower()->StartsWith(const_cast<String^>(MSGNW)->ToLower());
		if(result.MsgnwFlag){
			result.Article = result.Article->Substring(const_cast<String^>(MSGNW)->Length);
		}
		// 記事名直後の改行の有無
		if(article->TrimEnd(' ')->EndsWith("\n")){
			result.EnterFlag = true;
		}
	}
	return result;
}

/* 渡されたテキストの指定された位置に存在するWikipediaの内部リンク・テンプレートをチェック */
// ※正常時の戻り値には、]]の後ろの]の位置のインデックスを返す。異常時は-1
int WikipediaFormat::ChkLinkText(Link %o_Link, String ^i_Text, int i_Index)
{
	// 出力値初期化
	int lastIndex = -1;
	o_Link.Initialize();
	// 入力値に応じて、処理を振り分け
	if(MYAPP::Cmn::ChkTextInnerWith(i_Text, i_Index, "[[") == true){
		// 内部リンク
		o_Link = ParseInnerLink(i_Text->Substring(i_Index));
	}
	else if(MYAPP::Cmn::ChkTextInnerWith(i_Text, i_Index, "{{") == true){
		// テンプレート
		o_Link = ParseTemplate(i_Text->Substring(i_Index));
	}
	// 処理結果確認
	if(!String::IsNullOrEmpty(o_Link.Text)){
		lastIndex = i_Index + o_Link.Text->Length - 1;
	}
	return lastIndex;
}

/* 渡されたテキストの指定された位置に存在する変数を解析 */
int WikipediaFormat::ChkVariable(String ^%o_Variable, String ^%o_Value, String ^i_Text, int i_Index)
{
	// 出力値初期化
	int lastIndex = -1;
	o_Variable = "";
	o_Value = "";
	// 入力値確認
	if(MYAPP::Cmn::ChkTextInnerWith(i_Text->ToLower(), i_Index, "{{{") == false){
		return lastIndex;
	}
	// ブロック終了まで取得
	bool pipeFlag = false;
	for(int i = i_Index + 3; i < i_Text->Length ; i++){
		// 終了条件のチェック
		if(MYAPP::Cmn::ChkTextInnerWith(i_Text, i, "}}}") == true){
			lastIndex = i + 2;
			break;
		}
		// コメント（<!--）のチェック
		String ^dummy = "";
		int index = WikipediaArticle::ChkComment(dummy, i_Text, i);
		if(index != -1){
			i = index;
			continue;
		}
		// | が含まれている場合、以降の文字列は代入された値として扱う
		if(i_Text[i] == '|'){
			pipeFlag = true;
		}
		// | の前のとき
		else if(!pipeFlag){
			// ※Wikipediaの仕様上は、{{{1{|表示}}} のように変数名の欄に { を
			//   含めることができるようだが、判別しきれないので、エラーとする
			//   （どうせ意図してそんなことする人は居ないだろうし・・・）
			if(i_Text[i] == '{'){
				break;
			}
		}
		// | の後のとき
		else{
			// nowikiのチェック
			String ^nowiki = "";
			index = ChkNowiki(nowiki, i_Text, i);
			if(index != -1){
				i = index;
				o_Value += nowiki;
				continue;
			}
			// 変数（{{{1|{{{2}}}}}}とか）の再帰チェック
			String ^variable = "";
			index = ChkVariable(variable, dummy, i_Text, i);
			if(index != -1){
				i = index;
				o_Value += variable;
				continue;
			}
			// リンク [[ {{ （{{{1|[[test]]}}}とか）の再帰チェック
			Link link;
			index = ChkLinkText(link, i_Text, i);
			if(index != -1){
				i = index;
				o_Value += link.Text;
				continue;
			}
			o_Value += i_Text[i];
		}
	}
	// 変数ブロックの文字列を出力値に設定
	if(lastIndex != -1){
		o_Variable = i_Text->Substring(i_Index, lastIndex - i_Index + 1);
	}
	// 正常な構文ではなかった場合、出力値をクリア
	else{
		o_Variable = "";
		o_Value = "";
	}
	return lastIndex;
}

/* nowiki区間のチェック */
int WikipediaFormat::ChkNowiki(String ^%o_Text, String ^i_Text, int i_Index)
{
	// 出力値初期化
	int lastIndex = -1;
	o_Text = "";
	// 入力値確認
	if(MYAPP::Cmn::ChkTextInnerWith(i_Text->ToLower(), i_Index, const_cast<String^>(NOWIKISTART)->ToLower()) == false){
		return lastIndex;
	}
	// ブロック終了まで取得
	for(int i = i_Index + const_cast<String^>(NOWIKISTART)->Length; i < i_Text->Length ; i++){
		// 終了条件のチェック
		if(MYAPP::Cmn::ChkTextInnerWith(i_Text, i, const_cast<String^>(NOWIKIEND)) == true){
			lastIndex = i + const_cast<String^>(NOWIKIEND)->Length - 1;
			break;
		}
		// コメント（<!--）のチェック
		String ^dummy = "";
		int index = WikipediaArticle::ChkComment(dummy, i_Text, i);
		if(index != -1){
			i = index;
			continue;
		}
	}
	// 終わりが見つからない場合は、全てnowikiブロックと判断
	if(lastIndex == -1){
		lastIndex = i_Text->Length - 1;
	}
	o_Text = i_Text->Substring(i_Index, lastIndex - i_Index + 1);
	return lastIndex;
}

/* コメント区間のチェック */
int WikipediaFormat::ChkComment(String ^%o_Text, String ^i_Text, int i_Index)
{
	// 出力値初期化
	int lastIndex = -1;
	o_Text = "";
	// 入力値確認
	if(MYAPP::Cmn::ChkTextInnerWith(i_Text, i_Index, const_cast<String^>(COMMENTSTART)) == false){
		return lastIndex;
	}
	// コメント終了まで取得
	for(int i = i_Index + const_cast<String^>(COMMENTSTART)->Length; i < i_Text->Length ; i++){
		if(MYAPP::Cmn::ChkTextInnerWith(i_Text, i, const_cast<String^>(COMMENTEND)) == true){
			lastIndex = i + const_cast<String^>(COMMENTEND)->Length - 1;
			break;
		}
	}
	// 終わりが見つからない場合は、全てコメントと判断
	if(lastIndex == -1){
		lastIndex = i_Text->Length - 1;
	}
	o_Text = i_Text->Substring(i_Index, lastIndex - i_Index + 1);
	return lastIndex;
}


// WikipediaArticle

/* 初期設定 */
void WikipediaArticle::Initialize(String ^i_Title)
{
	// ※必須な情報が設定されていない場合、ArgumentNullExceptionを返す
	if(MYAPP::Cmn::NullCheckAndTrim(i_Title)->TrimStart(':') == ""){
		throw gcnew ArgumentNullException("i_Title");
	}
	// メンバ変数の初期化
	_Title = i_Title->Trim()->TrimStart(':');
	UriBuilder ^uri = gcnew UriBuilder("http", Server->Server);
	uri->Path = (Server->ArticleXmlPath + Title);
	_Url = uri->Uri;
	_Xml = nullptr;
	_Timestamp = DateTime::MinValue;
	_Text = "";
	_Redirect = "";
	_GetArticleStatus = HttpStatusCode::PaymentRequired;
	_GetArticleException = nullptr;
}

/* 記事の詳細情報を取得 */
bool WikipediaArticle::GetArticle(String ^i_UserAgent, String ^i_Referer, TimeSpan i_CacheEnabledSpan)
{
	// 初期化と値チェック
	_Xml = nullptr;
	_Timestamp = DateTime::MinValue;
	_Text = "";
	_Redirect = "";
	_GetArticleStatus = HttpStatusCode::PaymentRequired;
	_GetArticleException = nullptr;
	// 記事のデータをキャッシュやWikipediaサーバーから取得し、XMLに格納
	if(getCacheArticle(i_CacheEnabledSpan) == false){
		if(getServerArticle(i_UserAgent, i_Referer) == false){
			return false;
		}
	}
	// 取得されたXMLを解析し、メンバ変数に設定
	// 名前空間情報の上書き
	_Server->Namespaces = GetNamespaces();
	// 記事情報の設定
	XmlNamespaceManager ^nsMgr = gcnew XmlNamespaceManager(Xml->NameTable);
	nsMgr->AddNamespace("ns", const_cast<String^>(XMLNS));
	XmlElement ^pageElement = safe_cast<XmlElement^>(Xml->SelectSingleNode("/ns:mediawiki/ns:page", nsMgr));
	if(pageElement != nullptr){
		// 記事名の上書き
		XmlElement ^titleElement = safe_cast<XmlElement^>(pageElement->SelectSingleNode("ns:title", nsMgr));
		_Title = (!String::IsNullOrEmpty(titleElement->InnerText) ? titleElement->InnerText : Title);
		// 最終更新日時
		XmlElement ^timeElement = safe_cast<XmlElement^>(pageElement->SelectSingleNode("ns:revision/ns:timestamp", nsMgr));
		_Timestamp = DateTime::Parse(timeElement->InnerText);
		// 記事本文
		XmlElement ^textElement = safe_cast<XmlElement^>(pageElement->SelectSingleNode("ns:revision/ns:text", nsMgr));
		_Text = textElement->InnerText;
		// リダイレクトのチェックを行っておく
		IsRedirect();
	}
	// 記事が存在しない場合、XMLは取得できるがpageノードが無いので、404エラーと同様に扱う
	else{
		_GetArticleStatus = HttpStatusCode::NotFound;
		return false;
	}
	return true;
}

/* 記事の詳細情報を取得（キャッシュ有効期間はデフォルト） */
bool WikipediaArticle::GetArticle(String ^i_UserAgent, String ^i_Referer)
{
	// キャッシュ有効期間1週間でGetArticleを実行
	// ※記事の有無や名称、リダイレクト、言語間リンク等はそんなに更新されないだろう
	//   ・・・ということで、この期間に。
	//   必要であれば、キャッシュを使わない設定で本メソッドを直接呼ぶこと
	return GetArticle(i_UserAgent, i_Referer, TimeSpan(7, 0, 0, 0));
}

/* 記事の詳細情報を取得（UserAgent, Referer, キャッシュ有効期間はデフォルト） */
bool WikipediaArticle::GetArticle(void)
{
	// 既定値でGetArticleを実行
	return GetArticle("", "");
}

/* 記事のXMLをサーバーより取得 */
bool WikipediaArticle::getServerArticle(String ^i_UserAgent, String ^i_Referer)
{
	// 初期化と値チェック
	_Xml = nullptr;
	_GetArticleStatus = HttpStatusCode::PaymentRequired;
	_GetArticleException = nullptr;
	// 記事のXMLデータをWikipediaサーバーから取得
	try{
		HttpWebRequest ^req = safe_cast<HttpWebRequest^>(WebRequest::Create(Url));
		// UserAgent設定
		// ※WikipediaはUserAgentが空の場合エラーとなるので、必ず設定する
		if(!String::IsNullOrEmpty(i_UserAgent)){
			req->UserAgent = i_UserAgent;
		}
		else{
			Version ^ver = System::Reflection::Assembly::GetExecutingAssembly()->GetName()->Version;
			req->UserAgent = "WikipediaTranslationSupportTool/" + ver->Major + "." + String::Format("{0:D2}",ver->Minor);
		}
		// Referer設定
		if(!String::IsNullOrEmpty(i_Referer)){
			req->Referer = i_Referer;
		}
		HttpWebResponse ^res = safe_cast<HttpWebResponse^>(req->GetResponse());
		_GetArticleStatus = res->StatusCode;

		// 応答データを受信するためのStreamを取得し、データを取得
		// ※取得したXMLが正常かは、ここでは確認しない
		_Xml = gcnew XmlDocument();
		_Xml->Load(res->GetResponseStream());
		res->Close();

		// 取得したXMLを一時フォルダに保存
		try{
			// 一時フォルダを確認
			String ^tmpDir = Path::Combine(Path::GetTempPath(), Path::GetFileNameWithoutExtension(Application::ExecutablePath));
			if(Directory::Exists(tmpDir) == false){
				// 一時フォルダを作成
				Directory::CreateDirectory(tmpDir);
			}
			// ファイルの保存
			Xml->Save(Path::Combine(tmpDir, MYAPP::Cmn::ReplaceInvalidFileNameChars(Title) + ".xml"));
		}
		catch(Exception ^e){
			System::Diagnostics::Debug::WriteLine("WikipediaArticle::getServerArticle > 一時ファイルの保存に失敗しました : " + e->Message);
		}
	}
	catch(WebException ^e){
		// ProtocolErrorエラーの場合、ステータスコードを保持
		_Xml = nullptr;
		if(e->Status == WebExceptionStatus::ProtocolError){
			_GetArticleStatus = static_cast<HttpWebResponse^>(e->Response)->StatusCode;
		}
		_GetArticleException = e;
		return false;
	}
	catch(Exception ^e){
		_Xml = nullptr;
		_GetArticleException = e;
		return false;
	}
	return true;
}

/* 記事のXMLをキャッシュより取得 */
bool WikipediaArticle::getCacheArticle(TimeSpan i_CacheEnabledSpan)
{
	// 初期化と値チェック
	_Xml = nullptr;
	_GetArticleStatus = HttpStatusCode::PaymentRequired;
	_GetArticleException = nullptr;
	// キャッシュを使用する場合のみ
	if(i_CacheEnabledSpan > TimeSpan(0)){
		// 記事のXMLデータをキャッシュファイルから取得
		try{
			// 一時ファイルにアクセス
			String ^tmpFile = Path::Combine(Path::Combine(Path::GetTempPath(), Path::GetFileNameWithoutExtension(Application::ExecutablePath)), MYAPP::Cmn::ReplaceInvalidFileNameChars(Title) + ".xml");
			if(File::Exists(tmpFile) == true){
				// ファイルが有効期限内のものかを確認
				if((DateTime::UtcNow - File::GetLastWriteTimeUtc(tmpFile)) < i_CacheEnabledSpan){
					// ファイルをStreamで開き、データを取得
					XmlDocument ^tmpXml = gcnew XmlDocument();
					FileStream ^fs = File::OpenRead(tmpFile);
					try{
						tmpXml->Load(fs);
					}
					finally{
						fs->Close();
					}
					// 取得したXMLファイルが、目的とする記事のものかを確認
					XmlNamespaceManager ^nsMgr = gcnew XmlNamespaceManager(tmpXml->NameTable);
					nsMgr->AddNamespace("ns", const_cast<String^>(XMLNS));
					XmlElement ^rootElement = tmpXml->DocumentElement;
					XmlElement ^pageElement = safe_cast<XmlElement^>(tmpXml->SelectSingleNode("/ns:mediawiki/ns:page/ns:title", nsMgr));
					if(pageElement != nullptr){
						// 言語コード・記事名をチェック。大文字・小文字が異なる場合、別の記事と判別する
						// ※Low Earth orbitへのリダイレクトでLow earth orbitみたいなのがあるため
						//   ただし先頭はWikipediaの技術的制限で常に大文字なため、大文字で処理する
						String ^title = wchar_t::ToUpper(Title[0]).ToString();
						if(Title->Length > 1){
							title += Title->Substring(1);
						}
						if(rootElement->GetAttribute("xml:lang") == Server->Code &&
						   pageElement->InnerText == title){
							// XMLをメンバ変数に設定し、正常終了
							System::Diagnostics::Debug::WriteLine("WikipediaArticle::getCacheArticle > キャッシュ読込み : " + MYAPP::Cmn::ReplaceInvalidFileNameChars(Title) + ".xml");
							_Xml = tmpXml;
							return true;
						}
					}
				}
			}
		}
		catch(Exception ^e){
			_Xml = nullptr;
			_GetArticleException = e;
			return false;
		}
	}
	_GetArticleStatus = HttpStatusCode::NotFound;
	return false;
}

/* 指定された言語コードへの言語間リンクを返す */
String^ WikipediaArticle::GetInterWiki(String ^i_Code)
{
	// 初期化と値チェック
	String ^interWiki = "";
	if(Text == ""){
		// GetArticleを行っていない場合、InvalidOperationExceptionを返す
		throw gcnew InvalidOperationException();
	}
	// 記事に存在する指定言語への言語間リンクを取得
	for(int i = 0 ; i < Text->Length ; i++){
		// コメント（<!--）のチェック
		String ^comment = "";
		int index = chkComment(comment, i);
		if(index != -1){
			i = index;
		}
		// 指定言語への言語間リンクの場合、内容を取得し、処理終了
		else if(MYAPP::Cmn::ChkTextInnerWith(Text, i, "[[" + i_Code + ":") == true){
			Link link = ParseInnerLink(Text->Substring(i));
			if(!String::IsNullOrEmpty(link.Text)){
				interWiki = link.Article;
				break;
			}
		}
	}
	return interWiki;
}

/* 記事のXMLから名前空間情報を取得 */
array<WikipediaInformation::Namespace>^ WikipediaArticle::GetNamespaces(void)
{
	// XMLから名前空間情報を取得
	array<WikipediaInformation::Namespace> ^namespaces = gcnew array<WikipediaInformation::Namespace>(0);
	if(Xml == nullptr){
		// GetArticleを行っていない場合、InvalidOperationExceptionを返す
		throw gcnew InvalidOperationException();
	}
	XmlNamespaceManager ^nsMgr = gcnew XmlNamespaceManager(Xml->NameTable);
	nsMgr->AddNamespace("ns", const_cast<String^>(XMLNS));
	XmlNodeList ^nodeList = Xml->SelectNodes("/ns:mediawiki/ns:siteinfo/ns:namespaces/ns:namespace", nsMgr);
	for each(XmlNode ^node in nodeList){
		XmlElement ^e = safe_cast<XmlElement^>(node);
		if(e != nullptr){
			try{
				WikipediaInformation::Namespace ns = {Decimal::ToInt16(Decimal::Parse(e->GetAttribute("key"))), e->InnerText};
				MYAPP::Cmn::AddArray(namespaces, ns);
			}
			catch(...){}
		}
	}
	return namespaces;
}

/* 記事がリダイレクトかをチェック */
bool WikipediaArticle::IsRedirect(void)
{
	// 値チェック
	if(Text == ""){
		// GetArticleを行っていない場合、InvalidOperationExceptionを返す
		throw gcnew InvalidOperationException();
	}
	// 指定された記事がリダイレクト記事（#REDIRECT等）かをチェック
	// ※日本語版みたいに、#REDIRECTと言語固有の#転送みたいなのがあると思われるので、
	//   翻訳元言語と英語版の設定でチェック
	for(int i = 0 ; i < 2 ; i++){
		String ^redirect = static_cast<String^>(Server->Redirect->Clone());
		if(i == 1){
			if(Server->Code == "en"){
				continue;
			}
			WikipediaInformation en("en");
			redirect = en.Redirect;
		}
		if(redirect != ""){
			if(Text->ToLower()->StartsWith(redirect->ToLower()) == true){
				Link link = ParseInnerLink(Text->Substring(redirect->Length)->TrimStart());
				if(!String::IsNullOrEmpty(link.Text)){
					_Redirect = link.Article;
					return true;
				}
			}
		}
	}
	return false;
}

/* 記事がカテゴリーかをチェック */
bool WikipediaArticle::IsCategory(void)
{
	return IsCategory(Title);
}

/* 記事が画像かをチェック */
bool WikipediaArticle::IsImage(void)
{
	return IsImage(Title);
}

/* 記事が標準名前空間以外かをチェック */
bool WikipediaArticle::IsNotMainNamespace(void)
{
	return IsNotMainNamespace(Title);
}

/* 渡された内部リンク・テンプレートを解析 */
int WikipediaArticle::chkLinkText(WikipediaFormat::Link %o_Link, int i_Index)
{
	return ChkLinkText(o_Link, Text, i_Index);
}

/* 記事本文の指定された位置に存在する内部リンク・テンプレートを解析 */
int WikipediaArticle::chkComment(String ^%o_Text, int i_Index)
{
	return ChkComment(o_Text, Text, i_Index);
}
