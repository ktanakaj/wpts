using System;
using Honememo;

namespace Honememo.Wptscs.Models
{
    // Wikipediaの記事の書式を扱うためのクラス
    public class WikipediaFormat
    {
		// Wikipediaのリンクの要素を格納するための構造体
		public struct Link {
			public String Text;				// リンクのテキスト（[[～]]）
			public String Article;			// リンクの記事名
			public String Section;			// リンクのセクション名（#）
			public String[] PipeTexts;	// リンクのパイプ後の文字列（|）
			public String Code;				// 言語間または他プロジェクトへのリンクの場合、コード
			public bool TemplateFlag;			// テンプレート（{{～}}）かを示すフラグ
			public bool SubPageFlag;			// 記事名の先頭が / で始まるかを示すフラグ
			public bool StartColonFlag;		// リンクの先頭が : で始まるかを示すフラグ
			public bool MsgnwFlag;				// テンプレートの場合、msgnw: が付加されているかを示すフラグ
			public bool EnterFlag;				// テンプレートの場合、記事名の後で改行されるかを示すフラグ

            /* 初期化 */
            public void Initialize()
            {
	            // コンストラクタの代わりに、必要ならこれで初期化
	            Text = null;
	            Article = null;
	            Section = null;
	            PipeTexts = new String[0];
	            Code = null;
	            TemplateFlag = false;
	            SubPageFlag = false;
	            StartColonFlag = false;
	            MsgnwFlag = false;
	            EnterFlag = false;
            }

            /* 現在のText以外の値から、Textを生成 */
            public String MakeText()
            {
	            // 戻り値初期化
	            String result = "";
	            // 枠の設定
	            String startSign = "[[";
	            String endSign = "]]";
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
		            result += MSGNW;
	            }
	            // 言語コード・他プロジェクトコードの付加
	            if(!String.IsNullOrEmpty(Code)){
		            result += Code;
	            }
	            // リンクの付加
	            if(!String.IsNullOrEmpty(Article)){
		            result += Article;
	            }
	            // セクション名の付加
	            if(!String.IsNullOrEmpty(Section)){
		            result += ("#" + Section);
	            }
	            // 改行の付加
	            if(EnterFlag){
		            result += '\n';
	            }
	            // パイプ後の文字列の付加
	            if(PipeTexts != null){
		            foreach(String text in PipeTexts){
			            result += "|";
			            if(!String.IsNullOrEmpty(text)){
				            result += text;
			            }
		            }
	            }
	            // 終わりの枠の付加
	            result += endSign;
	            return result;
            }
		};

        /* コンストラクタ（サーバーを指定） */
        public WikipediaFormat(WikipediaInformation i_Server)
        {
	        // ※必須な情報が設定されていない場合、ArgumentNullExceptionを返す
	        if(i_Server == null){
		        throw new ArgumentNullException("i_Server");
	        }
	        // メンバ変数の初期化
	        _Server = i_Server;
        }

        /* 渡された記事名がカテゴリーかをチェック */
        public virtual bool IsCategory(String i_Name)
        {
	        // 指定された記事名がカテゴリー（Category:等で始まる）かをチェック
            String category = Server.GetNamespace(WikipediaInformation.CATEGORYNAMESPACENUMBER);
	        if(category != ""){
		        if(i_Name.ToLower().StartsWith(category.ToLower() + ":") == true){
			        return true;
		        }
	        }
	        return false;
        }

        /* 渡された記事名が画像かをチェック */
        public virtual bool IsImage(String i_Name)
        {
	        // 指定された記事名が画像（Image:等で始まる）かをチェック
	        // ※日本語版みたいに、image: と言語固有の 画像: みたいなのがあると思われるので、
	        //   翻訳元言語と英語版の設定でチェック
	        for(int i = 0 ; i < 2 ; i++){
                String image = Server.GetNamespace(WikipediaInformation.IMAGENAMESPACENUMBER);
		        if(i == 1){
			        if(Server.Code == "en"){
				        continue;
			        }
			        WikipediaInformation en = new WikipediaInformation("en");
                    image = en.GetNamespace(WikipediaInformation.IMAGENAMESPACENUMBER);
		        }
		        if(image != ""){
			        if(i_Name.ToLower().StartsWith(image.ToLower() + ":") == true){
				        return true;
			        }
		        }
	        }
	        return false;
        }

        /* 渡された記事名が標準名前空間以外かをチェック */
        public virtual bool IsNotMainNamespace(String i_Name)
        {
	        // 指定された記事名が標準名前空間以外の名前空間（Wikipedia:等で始まる）かをチェック
	        foreach(WikipediaInformation.Namespace ns in Server.Namespaces){
		        if(i_Name.ToLower().StartsWith(ns.Name.ToLower() + ":") == true){
			        return true;
		        }
	        }
	        return false;
        }

        /* 渡されたWikipediaの内部リンクを解析 */
        public virtual Link ParseInnerLink(String i_Text)
        {
	        // 出力値初期化
	        Link result = new Link();
	        result.Initialize();
	        // 入力値確認
	        if(i_Text.StartsWith("[[") == false){
		        return result;
	        }

	        // 構文を解析して、[[]]内部の文字列を取得
	        // ※構文はWikipediaのプレビューで色々試して確認、足りなかったり間違ってたりするかも・・・
	        String article = "";
	        String section = "";
	        String[] pipeTexts = new String[0];
	        int lastIndex = -1;
	        int pipeCounter = 0;
	        bool sharpFlag = false;
	        for(int i = 2 ; i < i_Text.Length ; i++){
		        char c = i_Text[i];
		        // ]]が見つかったら、処理正常終了
		        if(Honememo.Cmn.ChkTextInnerWith(i_Text, i, "]]") == true){
			        lastIndex = ++i;
			        break;
		        }
		        // | が含まれている場合、以降の文字列は表示名などとして扱う
		        if(c == '|'){
			        ++pipeCounter;
			        Honememo.Cmn.AddArray(ref pipeTexts, "");
			        continue;
		        }
		        // 変数（[[{{{1}}}]]とか）の再帰チェック
		        String dummy = "";
		        String variable = "";
		        int index = ChkVariable(ref variable, ref dummy, i_Text, i);
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
			        if(Honememo.Cmn.ChkTextInnerWith(i_Text, i, COMMENTSTART)){
				        break;
			        }
			        // nowikiのチェック
			        String nowiki = "";
			        index = ChkNowiki(ref nowiki, i_Text, i);
			        if(index != -1){
				        i = index;
				        pipeTexts[pipeCounter - 1] += nowiki;
				        continue;
			        }
			        // リンク [[ {{ （[[image:xx|[[test]]の画像]]とか）の再帰チェック
			        Link link = new Link();
			        index = ChkLinkText(ref link, i_Text, i);
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
		        result.Text = i_Text.Substring(0, lastIndex + 1);
		        // 前後のスペースは削除（見出しは後ろのみ）
		        result.Article = article.Trim();
		        result.Section = section.TrimEnd();
		        // | 以降はそのまま設定
		        result.PipeTexts = pipeTexts;
		        // 記事名から情報を抽出
		        // サブページ
		        if(result.Article.StartsWith("/") == true){
			        result.SubPageFlag = true;
		        }
		        // 先頭が :
		        else if(result.Article.StartsWith(":")){
			        result.StartColonFlag = true;
			        result.Article = result.Article.TrimStart(':').TrimStart();
		        }
		        // 標準名前空間以外で[[xxx:yyy]]のようになっている場合、言語コード
		        if(result.Article.Contains(":") == true && !IsNotMainNamespace(result.Article)){
			        // ※本当は、言語コード等の一覧を作り、其処と一致するものを・・・とすべきだろうが、
			        //   メンテしきれないので : を含む名前空間以外を全て言語コード等と判定
			        result.Code = result.Article.Substring(0, result.Article.IndexOf(':')).TrimEnd();
			        result.Article = result.Article.Substring(result.Article.IndexOf(':') + 1).TrimStart();
		        }
	        }
	        return result;
        }

        /* 渡されたWikipediaのテンプレートを解析 */
        public virtual Link ParseTemplate(String i_Text)
        {
	        // 出力値初期化
	        Link result = new Link();
	        result.Initialize();
	        result.TemplateFlag = true;
	        // 入力値確認
	        if(i_Text.StartsWith("{{") == false){
		        return result;
	        }

	        // 構文を解析して、{{}}内部の文字列を取得
	        // ※構文はWikipediaのプレビューで色々試して確認、足りなかったり間違ってたりするかも・・・
	        String article = "";
	        String[] pipeTexts = new String[0];
	        int lastIndex = -1;
	        int pipeCounter = 0;
	        for(int i = 2 ; i < i_Text.Length ; i++){
		        char c = i_Text[i];
		        // }}が見つかったら、処理正常終了
		        if(Honememo.Cmn.ChkTextInnerWith(i_Text, i, "}}") == true){
			        lastIndex = ++i;
			        break;
		        }
		        // | が含まれている場合、以降の文字列は引数などとして扱う
		        if(c == '|'){
			        ++pipeCounter;
			        Honememo.Cmn.AddArray(ref pipeTexts, "");
			        continue;
		        }
		        // 変数（[[{{{1}}}]]とか）の再帰チェック
		        String dummy = "";
		        String variable = "";
		        int index = ChkVariable(ref variable, ref dummy, i_Text, i);
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
			        if(Honememo.Cmn.ChkTextInnerWith(i_Text, i, COMMENTSTART)){
				        break;
			        }
			        // nowikiのチェック
			        String nowiki = "";
			        index = ChkNowiki(ref nowiki, i_Text, i);
			        if(index != -1){
				        i = index;
				        pipeTexts[pipeCounter - 1] += nowiki;
				        continue;
			        }
			        // リンク [[ {{ （{{test|[[例]]}}とか）の再帰チェック
			        Link link = new Link();
			        index = ChkLinkText(ref link, i_Text, i);
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
		        result.Text = i_Text.Substring(0, lastIndex + 1);
		        // 前後のスペース・改行は削除（見出しは後ろのみ）
		        result.Article = article.Trim();
		        // | 以降はそのまま設定
		        result.PipeTexts = pipeTexts;
		        // 記事名から情報を抽出
		        // サブページ
		        if(result.Article.StartsWith("/") == true){
			        result.SubPageFlag = true;
		        }
		        // 先頭が :
		        else if(result.Article.StartsWith(":")){
			        result.StartColonFlag = true;
			        result.Article = result.Article.TrimStart(':').TrimStart();
		        }
		        // 先頭が msgnw:
		        result.MsgnwFlag = result.Article.ToLower().StartsWith(MSGNW.ToLower());
		        if(result.MsgnwFlag){
			        result.Article = result.Article.Substring(MSGNW.Length);
		        }
		        // 記事名直後の改行の有無
		        if(article.TrimEnd(' ').EndsWith("\n")){
			        result.EnterFlag = true;
		        }
	        }
	        return result;
        }

        /* 渡されたテキストの指定された位置に存在するWikipediaの内部リンク・テンプレートをチェック */
        // ※正常時の戻り値には、]]の後ろの]の位置のインデックスを返す。異常時は-1
        public int ChkLinkText(ref Link o_Link, String i_Text, int i_Index)
        {
	        // 出力値初期化
	        int lastIndex = -1;
	        o_Link.Initialize();
	        // 入力値に応じて、処理を振り分け
	        if(Honememo.Cmn.ChkTextInnerWith(i_Text, i_Index, "[[") == true){
		        // 内部リンク
		        o_Link = ParseInnerLink(i_Text.Substring(i_Index));
	        }
	        else if(Honememo.Cmn.ChkTextInnerWith(i_Text, i_Index, "{{") == true){
		        // テンプレート
		        o_Link = ParseTemplate(i_Text.Substring(i_Index));
	        }
	        // 処理結果確認
	        if(!String.IsNullOrEmpty(o_Link.Text)){
		        lastIndex = i_Index + o_Link.Text.Length - 1;
	        }
	        return lastIndex;
        }

        /* 渡されたテキストの指定された位置に存在する変数を解析 */
        public virtual int ChkVariable(ref String o_Variable, ref String o_Value, String i_Text, int i_Index)
        {
	        // 出力値初期化
	        int lastIndex = -1;
	        o_Variable = "";
	        o_Value = "";
	        // 入力値確認
	        if(Honememo.Cmn.ChkTextInnerWith(i_Text.ToLower(), i_Index, "{{{") == false){
		        return lastIndex;
	        }
	        // ブロック終了まで取得
	        bool pipeFlag = false;
	        for(int i = i_Index + 3; i < i_Text.Length ; i++){
		        // 終了条件のチェック
		        if(Honememo.Cmn.ChkTextInnerWith(i_Text, i, "}}}") == true){
			        lastIndex = i + 2;
			        break;
		        }
		        // コメント（<!--）のチェック
		        String dummy = "";
		        int index = WikipediaArticle.ChkComment(ref dummy, i_Text, i);
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
			        String nowiki = "";
			        index = ChkNowiki(ref nowiki, i_Text, i);
			        if(index != -1){
				        i = index;
				        o_Value += nowiki;
				        continue;
			        }
			        // 変数（{{{1|{{{2}}}}}}とか）の再帰チェック
			        String variable = "";
			        index = ChkVariable(ref variable, ref dummy, i_Text, i);
			        if(index != -1){
				        i = index;
				        o_Value += variable;
				        continue;
			        }
			        // リンク [[ {{ （{{{1|[[test]]}}}とか）の再帰チェック
			        Link link = new Link();
			        index = ChkLinkText(ref link, i_Text, i);
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
		        o_Variable = i_Text.Substring(i_Index, lastIndex - i_Index + 1);
	        }
	        // 正常な構文ではなかった場合、出力値をクリア
	        else{
		        o_Variable = "";
		        o_Value = "";
	        }
	        return lastIndex;
        }

        /* nowiki区間のチェック */
        public static int ChkNowiki(ref String o_Text, String i_Text, int i_Index)
        {
	        // 出力値初期化
	        int lastIndex = -1;
	        o_Text = "";
	        // 入力値確認
	        if(Honememo.Cmn.ChkTextInnerWith(i_Text.ToLower(), i_Index, NOWIKISTART.ToLower()) == false){
		        return lastIndex;
	        }
	        // ブロック終了まで取得
	        for(int i = i_Index + NOWIKISTART.Length; i < i_Text.Length ; i++){
		        // 終了条件のチェック
		        if(Honememo.Cmn.ChkTextInnerWith(i_Text, i, NOWIKIEND)){
			        lastIndex = i + NOWIKIEND.Length - 1;
			        break;
		        }
		        // コメント（<!--）のチェック
		        String dummy = "";
		        int index = WikipediaArticle.ChkComment(ref dummy, i_Text, i);
		        if(index != -1){
			        i = index;
			        continue;
		        }
	        }
	        // 終わりが見つからない場合は、全てnowikiブロックと判断
	        if(lastIndex == -1){
		        lastIndex = i_Text.Length - 1;
	        }
	        o_Text = i_Text.Substring(i_Index, lastIndex - i_Index + 1);
	        return lastIndex;
        }

        /* コメント区間のチェック */
        public static int ChkComment(ref String o_Text, String i_Text, int i_Index)
        {
	        // 出力値初期化
	        int lastIndex = -1;
	        o_Text = "";
	        // 入力値確認
	        if(Honememo.Cmn.ChkTextInnerWith(i_Text, i_Index, COMMENTSTART) == false){
		        return lastIndex;
	        }
	        // コメント終了まで取得
	        for(int i = i_Index + COMMENTSTART.Length; i < i_Text.Length ; i++){
		        if(Honememo.Cmn.ChkTextInnerWith(i_Text, i, COMMENTEND)){
			        lastIndex = i + COMMENTEND.Length - 1;
			        break;
		        }
	        }
	        // 終わりが見つからない場合は、全てコメントと判断
	        if(lastIndex == -1){
		        lastIndex = i_Text.Length - 1;
	        }
	        o_Text = i_Text.Substring(i_Index, lastIndex - i_Index + 1);
	        return lastIndex;
        }

		// Wikipediaの固定値の書式
        public static readonly String COMMENTSTART = "<!--";
        public static readonly String COMMENTEND = "-->";
        public static readonly String NOWIKISTART = "<nowiki>";
        public static readonly String NOWIKIEND = "</nowiki>";
        public static readonly String MSGNW = "msgnw:";

		// 記事が所属するサーバー情報
		public WikipediaInformation Server
        {
			get {
				return _Server;
			}
		}

		// 記事が所属するサーバー情報（property）
		protected WikipediaInformation _Server;
    }
}
