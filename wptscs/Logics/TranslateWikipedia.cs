// ================================================================================================
// <summary>
//      Wikipedia用の翻訳支援処理実装クラスソース</summary>
//
// <copyright file="TranslateWikipedia.cs" company="honeplusのメモ帳">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Logics
{
    using System;
    using System.Net;
    using System.Windows.Forms;

    using Honememo.Wptscs.Models;
    using Honememo.Wptscs.Properties;

    /// <summary>
    /// Wikipedia用の翻訳支援処理実装クラスです。
    /// </summary>
    public class TranslateWikipedia : TranslateNetworkObject
    {
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="source">翻訳元言語</param>
        /// <param name="target">翻訳先言語</param>
        public TranslateWikipedia(
            WikipediaInformation source, WikipediaInformation target)
            : base(source, target)
        {
        }

        /// <summary>
        /// 翻訳支援処理実行部の本体。
        /// ※継承クラスでは、この関数に処理を実装すること
        /// </summary>
        /// <param name="i_Name">記事名</param>
        /// <returns><c>true</c> 処理成功</returns>
        protected override bool RunBody(string i_Name)
        {
            System.Diagnostics.Debug.WriteLine("\nTranslateWikipedia.runBody > " + i_Name);
            // 対象記事を取得
            WikipediaArticle article = chkTargetArticle(i_Name);
            if(article.Text == ""){
                return false;
            }
            // 対象記事に言語間リンクが存在する場合、処理を継続するか確認
            String interWiki = article.GetInterWiki(target.Code);
            if(interWiki != ""){
                if(MessageBox.Show(
                        String.Format(Resources.QuestionMessage_ArticleExist, interWiki),
                        Resources.QuestionTitle,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question)
                   == System.Windows.Forms.DialogResult.No){
                    LogLine(ENTER + String.Format(Resources.QuestionMessage_ArticleExist, interWiki));
                    return false;
                }
                else{
                    LogLine("→ " + String.Format(Resources.LogMessage_ArticleExistInterWiki, interWiki));
                }
            }

            // 冒頭部を作成
            Text += "'''xxx'''";
            String bracket = ((WikipediaInformation) target).Bracket;
            if(bracket.Contains("{0}") == true){
                String originalName = "";
                String langTitle = ((WikipediaInformation) source).GetFullName(target.Code);
                if(langTitle != ""){
                    originalName = "[[" + langTitle + "]]: ";
                }
                Text += String.Format(bracket, originalName + "'''" + i_Name + "'''");
            }
            Text += "\n\n";
            // 言語間リンク・定型句の変換
            LogLine(ENTER + "→ " + String.Format(Resources.LogMessage_CheckAndReplaceStart, interWiki));
            Text += replaceText(article.Text, article.Title);
            // ユーザーからの中止要求をチェック
            if(CancellationPending){
                return false;
            }
            // 新しい言語間リンクと、コメントを追記
            Text += ("\n\n[[" + source.Code + ":" + i_Name + "]]\n");
            Text += (String.Format(Resources.ArticleFooter, Honememo.Cmn.GetProductName(),
                source.Code, i_Name, article.Timestamp.ToString("U")) + "\n");
            // ダウンロードされるテキストがLFなので、ここで全てCRLFに変換
            // ※ダウンロード時にCRLFにするような仕組みが見つかれば、そちらを使う
            //   その場合、上のように\nをべたに吐いている部分を修正する
            Text = Text.Replace("\n", ENTER);

            System.Diagnostics.Debug.WriteLine("TranslateWikipedia.runBody > Success!");
            return true;
        }

        /// <summary>
        /// 翻訳支援対象の記事を取得。
        /// </summary>
        /// <param name="i_Name">記事名</param>
        /// <returns>取得した記事</returns>
        protected WikipediaArticle chkTargetArticle(string i_Name)
        {
            // 指定された記事の生データをWikipediaから取得
            LogLine(String.Format(Resources.LogMessage_GetArticle, "http://" + ((WikipediaInformation) source).Server, i_Name));
            WikipediaArticle article = new WikipediaArticle((WikipediaInformation) source, i_Name);
            if(article.GetArticle(UserAgent, Referer, new TimeSpan(0)) == false){
                if(article.GetArticleStatus == HttpStatusCode.NotFound){
                    LogLine("→ " + Resources.LogMessage_ArticleNothing);
                }
                else{
                    LogLine("→ " + article.GetArticleException.Message);
                    LogLine("→ " + String.Format(Resources.LogMessage_ErrorURL, article.Url));
                }
            }
            else{
                // Wikipediaへの初回アクセス時に、名前空間情報を取得する
                ((WikipediaInformation) source).Namespaces = article.GetNamespaces();
                // リダイレクトかをチェックし、リダイレクトであれば、その先の記事を取得
                if(article.IsRedirect()){
                    LogLine("→ " + Resources.LogMessage_Redirect + " [[" + article.Redirect + "]]");
                    article = new WikipediaArticle((WikipediaInformation) source, article.Redirect);
                    if(article.GetArticle(UserAgent, Referer, new TimeSpan(0)) == false){
                        if(article.GetArticleStatus == HttpStatusCode.NotFound){
                            LogLine("→ " + Resources.LogMessage_ArticleNothing);
                        }
                        else{
                            LogLine("→ " + article.GetArticleException.Message);
                            LogLine("→ " + String.Format(Resources.LogMessage_ErrorURL, article.Url));
                        }
                    }
                }
            }
            return article;
        }

        /// <summary>
        /// 指定された記事を取得し、言語間リンクを確認、返す。
        /// </summary>
        /// <param name="i_Name">記事名</param>
        /// <param name="i_TemplateFlag"><c>true</c> テンプレート</param>
        /// <returns>言語間リンク先の記事、存在しない場合 <c>null</c></returns>
        protected virtual string getInterWiki(string i_Name, bool i_TemplateFlag)
        {
            // 指定された記事の生データをWikipediaから取得
            // ※記事自体が存在しない場合、NULLを返す
            String interWiki = null;
            String name = i_Name;
            if(!i_TemplateFlag){
                Log += "[[" + name + "]] → ";
            }
            else{
                Log += "{{" + name + "}} → ";
            }
            WikipediaArticle article = new WikipediaArticle((WikipediaInformation) source, i_Name);
            if(article.GetArticle(UserAgent, Referer) == false){
                if(article.GetArticleStatus == HttpStatusCode.NotFound){
                    Log += Resources.LogMessage_LinkArticleNothing;
                }
                else{
                    LogLine("→ " + article.GetArticleException.Message);
                    LogLine("→ " + String.Format(Resources.LogMessage_ErrorURL, article.Url));
                }
            }
            else{
                // リダイレクトかをチェックし、リダイレクトであれば、その先の記事を取得
                if(article.IsRedirect()){
                    Log += (Resources.LogMessage_Redirect + " [[" + article.Redirect + "]] → ");
                    article = new WikipediaArticle((WikipediaInformation) source, article.Redirect);
                    if(article.GetArticle(UserAgent, Referer) == false){
                        if(article.GetArticleStatus == HttpStatusCode.NotFound){
                            LogLine("→ " + Resources.LogMessage_ArticleNothing);
                        }
                        else{
                            LogLine("→ " + article.GetArticleException.Message);
                            LogLine("→ " + String.Format(Resources.LogMessage_ErrorURL, article.Url));
                        }
                    }
                }
                if(article.Text != ""){
                    // 翻訳先言語への言語間リンクを捜索
                    interWiki = article.GetInterWiki(target.Code);
                    if(interWiki != ""){
                        Log += ("[[" + interWiki + "]]");
                    }
                    else{
                        Log += Resources.LogMessage_InterWikiNothing;
                    }
                }
            }
            // 改行が出力されていない場合（正常時）、改行
            if(Log.EndsWith(ENTER) == false){
                Log += ENTER;
            }
            return interWiki;
        }

        /// <summary>
        /// 指定された記事を取得し、言語間リンクを確認、返す（テンプレート以外）。
        /// </summary>
        /// <param name="name">記事名</param>
        /// <returns>言語間リンク先の記事、存在しない場合 <c>null</c></returns>
        protected String getInterWiki(string name)
        {
            return getInterWiki(name, false);
        }

        /// <summary>
        /// 渡されたテキストを解析し、言語間リンク・見出し等の変換を行う。
        /// </summary>
        /// <param name="i_Text">記事テキスト</param>
        /// <param name="i_Parent"></param>
        /// <param name="i_TitleFlag"></param>
        /// <returns>変換後の記事テキスト</returns>
        protected string replaceText(string i_Text, string i_Parent, bool i_TitleFlag)
        {
            // 指定された記事の言語間リンク・見出しを探索し、翻訳先言語での名称に変換し、それに置換した文字列を返す
            String result = "";
            bool enterFlag = true;
            WikipediaFormat wikiAP = new WikipediaFormat((WikipediaInformation)source);
            for(int i = 0 ; i < i_Text.Length ; i++){
                // ユーザーからの中止要求をチェック
                if(CancellationPending == true){
                    break;
                }
                char c = i_Text[i];
                // 見出しも処理対象の場合
                if(i_TitleFlag){
                    // 改行の場合、次のループで見出し行チェックを行う
                    if(c == '\n'){
                        enterFlag = true;
                        result += c;
                        continue;
                    }
                    // 行の始めでは、その行が見出しの行かのチェックを行う
                    if(enterFlag){
                        String newTitleLine = "";
                        int index2 = chkTitleLine(ref newTitleLine, i_Text, i);
                        if (index2 != -1)
                        {
                            // 行の終わりまでインデックスを移動
                            i = index2;
                            // 置き換えられた見出し行を出力
                            result += newTitleLine;
                            continue;
                        }
                        else{
                            enterFlag = false;
                        }
                    }
                }
                // コメント（<!--）のチェック
                String comment = "";
                int index = WikipediaFormat.ChkComment(ref comment, i_Text, i);
                if(index != -1){
                    i = index;
                    result += comment;
                    if(comment.Contains("\n") == true){
                        enterFlag = true;
                    }
                    continue;
                }
                // nowikiのチェック
                String nowiki = "";
                index = WikipediaFormat.ChkNowiki(ref nowiki, i_Text, i);
                if(index != -1){
                    i = index;
                    result += nowiki;
                    continue;
                }
                // 変数（{{{1}}}とか）のチェック
                String variable = "";
                String value = "";
                index = wikiAP.ChkVariable(ref variable, ref value, i_Text, i);
                if(index != -1){
                    i = index;
                    // 変数の | 以降に値が記述されている場合、それに対して再帰的に処理を行う
                    int valueIndex = variable.IndexOf('|');
                    if(valueIndex != -1 && !String.IsNullOrEmpty(value)){
                        variable = (variable.Substring(0, valueIndex + 1) + replaceText(value, i_Parent) + "}}}");
                    }
                    result += variable;
                    continue;
                }
                // 内部リンク・テンプレートのチェック＆変換、言語間リンクを取得し出力する
                String text = "";
                index = replaceLink(ref text, i_Text, i, i_Parent);
                if(index != -1){
                    i = index;
                    result += text;
                    continue;
                }
                // 通常はそのままコピー
                result += i_Text[i];
            }
            return result;
        }

        /// <summary>
        /// 渡されたテキストを解析し、言語間リンク・見出し等の変換を行う。
        /// </summary>
        /// <param name="text">記事テキスト</param>
        /// <param name="parent"></param>
        /// <returns>変換後の記事テキスト</returns>
        protected String replaceText(string text, string parent)
        {
            return replaceText(text, parent, true);
        }

        /// <summary>
        /// リンクの解析・置換を行う。
        /// </summary>
        /// <param name="o_Link"></param>
        /// <param name="i_Text"></param>
        /// <param name="i_Index"></param>
        /// <param name="i_Parent"></param>
        /// <returns></returns>
        protected int replaceLink(ref string o_Link, string i_Text, int i_Index, string i_Parent)
        {
            // 出力値初期化
            int lastIndex = -1;
            o_Link = "";
            WikipediaFormat.Link link = new WikipediaFormat.Link();
            // 内部リンク・テンプレートの確認と解析
            WikipediaFormat wikiAP = new WikipediaFormat((WikipediaInformation) source);
            lastIndex = wikiAP.ChkLinkText(ref link, i_Text, i_Index);
            if(lastIndex != -1){
                // 記事名に変数が使われている場合があるので、そのチェックと展開
                int index = link.Article.IndexOf("{{{");
                if(index != -1){
                    String variable = "";
                    String value = "";
                    int lastIndex2 = wikiAP.ChkVariable(ref variable, ref value, link.Article, index);
                    if (lastIndex2 != -1 && !String.IsNullOrEmpty(value))
                    {
                        // 変数の | 以降に値が記述されている場合、それに置き換える
                        String newArticle = (link.Article.Substring(0, index) + value);
                        if (lastIndex2 + 1 < link.Article.Length)
                        {
                            newArticle += link.Article.Substring(lastIndex2 + 1);
                        }
                        link.Article = newArticle;
                    }
                    // 値が設定されていない場合、処理してもしょうがないので、除外
                    else{
                        System.Diagnostics.Debug.WriteLine("TranslateWikipedia.replaceLink > 対象外 : " + link.Text);
                        return -1;
                    }
                }

                String newText = null;
                // 内部リンクの場合
                if(i_Text[i_Index] == '['){
                    // 内部リンクの変換後文字列を取得
                    newText = replaceInnerLink(link, i_Parent);
                }
                // テンプレートの場合
                else if(i_Text[i_Index] == '{'){
                    // テンプレートの変換後文字列を取得
                    newText = replaceTemplate(link, i_Parent);
                }
                // 上記以外の場合は、対象外
                else{
                    System.Diagnostics.Debug.WriteLine("TranslateWikipedia.replaceLink > プログラムミス : " + link.Text);
                }
                // 変換後文字列がNULL以外
                if(newText != null){
                    o_Link = newText;
                }
                else{
                    lastIndex = -1;
                }
            }
            return lastIndex;
        }

        /// <summary>
        /// 内部リンクの文字列を変換する。
        /// </summary>
        /// <param name="i_Link"></param>
        /// <param name="i_Parent"></param>
        /// <returns></returns>
        protected string replaceInnerLink(WikipediaFormat.Link i_Link, string i_Parent)
        {
            // 変数初期設定
            String result = "[[";
            String comment = "";
            WikipediaFormat.Link link = i_Link;
            // 記事内を指している場合（[[#関連項目]]だけとか）以外
            if(!String.IsNullOrEmpty(link.Article) &&
               !(link.Article == i_Parent && String.IsNullOrEmpty(link.Code) && !String.IsNullOrEmpty(link.Section))){
                // 変換の対象外とするリンクかをチェック
                WikipediaArticle article = new WikipediaArticle((WikipediaInformation)source, link.Article);
                // サブページの場合、記事名を補填
                if(link.SubPageFlag){
                    link.Article = i_Parent + link.Article;
                }
                // 言語間リンク・姉妹プロジェクトへのリンク・画像は対象外
                else if(!String.IsNullOrEmpty(link.Code) || article.IsImage()){
                    result = "";
                    // 先頭が : でない、翻訳先言語への言語間リンクの場合
                    if(!link.StartColonFlag && link.Code == target.Code){
                        // 削除する。正常終了で、置換後文字列なしを返す
                        System.Diagnostics.Debug.WriteLine("TranslateWikipedia.replaceInnerLink > " + link.Text + " を削除");
                        return "";
                    }
                    // それ以外は対象外
                    System.Diagnostics.Debug.WriteLine("TranslateWikipedia.replaceInnerLink > 対象外 : " + link.Text);
                    return null;
                }
                // リンクを辿り、対象記事の言語間リンクを取得
                String interWiki = getInterWiki(link.Article);
                // 記事自体が存在しない（赤リンク）場合、リンクはそのまま
                if(interWiki == null){
                    result += link.Article;
                }
                // 言語間リンクが存在しない場合、[[:en:xxx]]みたいな形式に置換
                else if(interWiki == ""){
                    result += (":" + source.Code + ":" + link.Article);
                }
                // 言語間リンクが存在する場合、そちらを指すように置換
                else{
                    // 前の文字列を復元
                    if(link.SubPageFlag){
                        int index = interWiki.IndexOf('/');
                        if(index == -1){
                            index = 0;
                        }
                        result += interWiki.Substring(index);
                    }
                    else if(link.StartColonFlag){
                        result += (":" + interWiki);
                    }
                    else{
                        result += interWiki;
                    }
                }
                // カテゴリーの場合は、コメントで元の文字列を追加する
                if(article.IsCategory() && !link.StartColonFlag){
                    comment = (WikipediaFormat.COMMENTSTART + " " + link.Text + " " + WikipediaFormat.COMMENTEND);
                    // カテゴリーで[[:en:xxx]]みたいな形式にした場合、| 以降は不要なので削除
                    if(interWiki == ""){
                        link.PipeTexts = new String[0];
                    }
                }
                // 表示名が存在しない場合、元の名前を表示名に設定
                else if(link.PipeTexts.Length == 0 && interWiki != null){
                    Honememo.Cmn.AddArray(ref link.PipeTexts, article.Title);
                }
            }
            // 見出し（[[#関連項目]]とか）を出力
            if(!String.IsNullOrEmpty(link.Section)){
                // 見出しは、定型句変換を通す
                result += ("#" + getKeyWord(link.Section));
            }
            // 表示名を出力
            foreach(String text in link.PipeTexts){
                result += "|";
                if(!String.IsNullOrEmpty(text)){
                    // 画像の場合、| の後に内部リンクやテンプレートが書かれている場合があるが、
                    // 画像は処理対象外でありその中のリンクは個別に再度処理されるため、ここでは特に何もしない
                    result += text;
                }
            }
            // リンクを閉じる
            result += "]]";
            // コメントを付加
            if(comment != ""){
                result += comment;
            }
            System.Diagnostics.Debug.WriteLine("TranslateWikipedia.replaceInnerLink > " + link.Text);
            return result;
        }

        /// <summary>
        /// テンプレートの文字列を変換する。
        /// </summary>
        /// <param name="i_Link"></param>
        /// <param name="i_Parent"></param>
        /// <returns></returns>
        protected String replaceTemplate(WikipediaFormat.Link i_Link, string i_Parent)
        {
            // 変数初期設定
            String result = "";
            WikipediaFormat.Link link = i_Link;
            // テンプレートは記事名が必須
            if(String.IsNullOrEmpty(link.Article)){
                System.Diagnostics.Debug.WriteLine("TranslateWikipedia.replaceTemplate > 対象外 : " + link.Text);
                return null;
            }
            // システム変数の場合は対象外
            if(((WikipediaInformation) source).ChkSystemVariable(link.Article) == true){
                System.Diagnostics.Debug.WriteLine("TranslateWikipedia.replaceTemplate > システム変数 : " + link.Text);
                return null;
            }
            // テンプレート名前空間か、普通の記事かを判定
            if(!link.StartColonFlag && !link.SubPageFlag){
                String templateStr = ((WikipediaInformation) source).GetNamespace(WikipediaInformation.TEMPLATENAMESPACENUMBER);
                if(templateStr != "" && !link.Article.StartsWith(templateStr + ":")){
                    WikipediaArticle article = new WikipediaArticle((WikipediaInformation) source, templateStr + ":" + link.Article);
                    // 記事が存在する場合、テンプレートをつけた名前で取得
                    if(article.GetArticle(UserAgent, Referer) == true){
                        link.Article = article.Title;
                    }
                    // 記事が取得できない場合も、404でない場合は存在するとして処理
                    else if(article.GetArticleStatus != HttpStatusCode.NotFound){
                        LogLine(String.Format(Resources.LogMessage_TemplateUnknown, link.Article, templateStr, article.GetArticleException.Message));
                        link.Article = article.Title;
                    }
                }
            }
            // サブページの場合、記事名を補填
            else if(link.SubPageFlag){
                link.Article = i_Parent + link.Article;
            }
            // リンクを辿り、対象記事の言語間リンクを取得
            String interWiki = getInterWiki(link.Article, true);
            // 記事自体が存在しない（赤リンク）場合、リンクはそのまま
            if(interWiki == null){
                result += link.Text;
            }
            // 言語間リンクが存在しない場合、[[:en:Template:xxx]]みたいな普通のリンクに置換
            else if(interWiki == ""){
                // おまけで、元のテンプレートの状態をコメントでつける
                result += ("[[:" + source.Code + ":" + link.Article + "]]" + WikipediaFormat.COMMENTSTART + " " + link.Text + " " + WikipediaFormat.COMMENTEND);
            }
            // 言語間リンクが存在する場合、そちらを指すように置換
            else{
                result += "{{";
                // 前の文字列を復元
                if(link.StartColonFlag){
                    result += ":";
                }
                if(link.MsgnwFlag){
                    result += WikipediaFormat.MSGNW;
                }
                // : より前の部分を削除して出力（: が無いときは-1+1で0から）
                result += interWiki.Substring(interWiki.IndexOf(':') + 1);
                // 改行を復元
                if(link.EnterFlag){
                    result += "\n";
                }
                // | の後を付加
                foreach(String text in link.PipeTexts){
                    result += "|";
                    if(!String.IsNullOrEmpty(text)){
                        // | の後に内部リンクやテンプレートが書かれている場合があるので、再帰的に処理する
                        result += replaceText(text, i_Parent);
                    }
                }
                // リンクを閉じる
                result += "}}";
            }
            System.Diagnostics.Debug.WriteLine("TranslateWikipedia.replaceTemplate > " + link.Text);
            return result;
        }

        /// <summary>
        /// 指定されたインデックスの位置に存在する見出し(==関連項目==みたいなの)を解析し、可能であれば変換して返す。
        /// </summary>
        /// <param name="o_Title"></param>
        /// <param name="i_Text"></param>
        /// <param name="i_Index"></param>
        /// <returns></returns>
        protected virtual int chkTitleLine(ref String o_Title, String i_Text, int i_Index)
        {
            // 初期化
            // ※見出しではない、構文がおかしいなどの場合、-1を返す
            int lastIndex = -1;
            o_Title = "";
/*          // 入力値確認、ファイルの先頭、または改行後の==で始まっているかをチェック
            // ※コメント時とか考えるとムズいので、==だけチェックして、後は呼び出し元で行う
            if(i_Index != 0){
                if((MYAPP.Cmn.ChkTextInnerWith(i_Text, i_Index - 1, "\n==") == false){
                    return lastIndex;
                }
            }
            else if(MYAPP.Cmn.ChkTextInnerWith(i_Text, i_Index, "==") == false){
                return lastIndex;
            }
*/          // 構文を解析して、1行の文字列と、=の個数を取得
            // ※構文はWikipediaのプレビューで色々試して確認、足りなかったり間違ってたりするかも・・・
            // ※Wikipediaでは <!--test-.=<!--test-.=関連項目<!--test-.==<!--test-. みたいなのでも
            //   正常に認識するので、できるだけ対応する
            // ※変換が正常に行われた場合、コメントは削除される
            bool startFlag = true;
            int startSignCounter = 0;
            String nonCommentLine = "";
            for(lastIndex = i_Index ; lastIndex < i_Text.Length ; lastIndex++){
                char c = i_Text[lastIndex];
                // 改行まで
                if(c == '\n'){
                    break;
                }
                // コメントは無視する
                String comment = "";
                int index = WikipediaArticle.ChkComment(ref comment, i_Text, lastIndex);
                if(index != -1){
                    o_Title += comment;
                    lastIndex = index;
                    continue;
                }
                // 先頭部の場合、=の数を数える
                else if(startFlag){
                    if(c == '='){
                        ++startSignCounter;
                    }
                    else{
                        startFlag = false;
                    }
                }
                nonCommentLine += c;
                o_Title += c;
            }
            // 改行文字、または文章の最後+1になっているはずなので、1文字戻す
            --lastIndex;
            // = で始まる行ではない場合、処理対象外
            if(startSignCounter < 1){
                o_Title = "";
                return -1;
            }
            // 終わりの = の数を確認
            // ※↓の処理だと中身の無い行（====とか）は弾かれてしまうが、どうせ処理できないので許容する
            int endSignCounter = 0;
            for(int i = nonCommentLine.Length - 1 ; i >= startSignCounter ; i--){
                if(nonCommentLine[i] == '='){
                    ++endSignCounter;
                }
                else{
                    break;
                }
            }
            // = で終わる行ではない場合、処理対象外
            if(endSignCounter < 1){
                o_Title = "";
                return -1;
            }
            // 始まりと終わり、=の少ないほうにあわせる（==test===とか用の処理）
            int signCounter = startSignCounter;
            if(startSignCounter > endSignCounter){
                signCounter = endSignCounter;
            }
            // 定型句変換
            String oldText = nonCommentLine.Substring(signCounter, nonCommentLine.Length - (signCounter * 2)).Trim();
            String newText = getKeyWord(oldText);
            if(oldText != newText){
                String sign = "=";
                for(int i = 1 ; i < signCounter ; i++){
                    sign += "=";
                }
                String newTitle = (sign + newText + sign);
                LogLine(ENTER + o_Title + " → " + newTitle);
                o_Title = newTitle;
            }
            else{
                LogLine(ENTER + o_Title);
            }
            return lastIndex;
        }

        /// <summary>
        /// 指定されたコードでの定型句に相当する、別の言語での定型句を取得。
        /// </summary>
        /// <param name="i_Key"></param>
        /// <returns></returns>
        protected virtual String getKeyWord(String i_Key)
        {
            // ※設定が存在しない場合、入力定型句をそのままを返す
            String key = ((i_Key != null) ? i_Key : "");
            WikipediaInformation src = (WikipediaInformation) source;
            WikipediaInformation tar = (WikipediaInformation) target;
            if(key.Trim() == ""){
                return key;
            }
            for(int i = 0 ; i < src.TitleKeys.Length ; i++){
                if(src.TitleKeys[i].ToLower() == key.Trim().ToLower()){
                    if(tar.TitleKeys.Length > i){
                        if(tar.TitleKeys[i] != ""){
                            key = tar.TitleKeys[i];
                        }
                        break;
                    }
                }
            }
            return key;
        }
    }
}
