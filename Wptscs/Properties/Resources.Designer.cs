﻿//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.225
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Honememo.Wptscs.Properties {
    using System;
    
    
    /// <summary>
    ///   ローカライズされた文字列などを検索するための、厳密に型指定されたリソース クラスです。
    /// </summary>
    // このクラスは StronglyTypedResourceBuilder クラスが ResGen
    // または Visual Studio のようなツールを使用して自動生成されました。
    // メンバーを追加または削除するには、.ResX ファイルを編集して、/str オプションと共に
    // ResGen を実行し直すか、または VS プロジェクトをビルドし直します。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   このクラスで使用されているキャッシュされた ResourceManager インスタンスを返します。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Honememo.Wptscs.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   厳密に型指定されたこのリソース クラスを使用して、すべての検索リソースに対し、
        ///   現在のスレッドの CurrentUICulture プロパティをオーバーライドします。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Wikipedia 翻訳支援ツール Ver{0}.{1:D2} に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ApplicationName {
            get {
                return ResourceManager.GetString("ApplicationName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   &lt;!-- {0}、[[:{1}:{2}]]（{3}(UTC)）より --&gt; に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ArticleFooter {
            get {
                return ResourceManager.GetString("ArticleFooter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   一時ファイルの作成に失敗しました。設定ファイルに異常が無いか、また以下のフォルダにファイルが作成可能かを確認してください。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ErrorMessage_TemporayError {
            get {
                return ResourceManager.GetString("ErrorMessage_TemporayError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   設定ファイル読み込み時にエラーが発生しました。ファイルが破損している可能性があります。
        ///
        ///{0} に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ErrorMessageConfigLordFailed {
            get {
                return ResourceManager.GetString("ErrorMessageConfigLordFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   設定ファイル{0}が見つかりません。インストールファイルを確認してください。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ErrorMessageConfigNotFound {
            get {
                return ResourceManager.GetString("ErrorMessageConfigNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   設定ファイル保存時にエラーが発生しました。現在の設定は保存されていません。
        ///
        ///{0} に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ErrorMessageConfigSaveFailed {
            get {
                return ResourceManager.GetString("ErrorMessageConfigSaveFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   サーバーへの接続に失敗しました。ネットワークの設定、またはサーバーの状態を確認してください。（{0}） に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ErrorMessageConnectionFailed {
            get {
                return ResourceManager.GetString("ErrorMessageConnectionFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   想定外のエラーが発生しました。プログラムが不安定な状態になった可能性があります。プログラムを再起動してください。
        ///問題が再発する場合は、設定ファイルを削除するなどしてから、プログラムを再起動してください。
        ///また、手順や現在の設定を確認し、開発者にご連絡ください。
        ///
        ///＜エラーの内容＞
        ///{0}
        ///{1} に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ErrorMessageDevelopmentError {
            get {
                return ResourceManager.GetString("ErrorMessageDevelopmentError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   エラー に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ErrorTitle {
            get {
                return ResourceManager.GetString("ErrorTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   {0} ({1}) に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string HeadingViewHeaderText {
            get {
                return ResourceManager.GetString("HeadingViewHeaderText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   現在、{0}には未対応ですm(__)m に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string InformationMessage_DevelopingMethod {
            get {
                return ResourceManager.GetString("InformationMessage_DevelopingMethod", resourceCulture);
            }
        }
        
        /// <summary>
        ///   お知らせ に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string InformationTitle {
            get {
                return ResourceManager.GetString("InformationTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   注意：この記事には、翻訳先言語の記事 [[{0}]] への言語間リンクが存在します。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string LogMessage_ArticleExistInterWiki {
            get {
                return ResourceManager.GetString("LogMessage_ArticleExistInterWiki", resourceCulture);
            }
        }
        
        /// <summary>
        ///   翻訳元として指定された記事は存在しません。記事名を確認してください。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string LogMessage_ArticleNothing {
            get {
                return ResourceManager.GetString("LogMessage_ArticleNothing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   言語間リンクの探索、定型句の変換を行います に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string LogMessage_CheckAndReplaceStart {
            get {
                return ResourceManager.GetString("LogMessage_CheckAndReplaceStart", resourceCulture);
            }
        }
        
        /// <summary>
        ///   
        ///処理結果を {0} に出力しました。このログは {1} に保存されます。
        /// に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string LogMessage_End {
            get {
                return ResourceManager.GetString("LogMessage_End", resourceCulture);
            }
        }
        
        /// <summary>
        ///   
        ///ファイル {0} の保存に失敗しました。（{1}）
        /// に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string LogMessage_ErrorFileSave {
            get {
                return ResourceManager.GetString("LogMessage_ErrorFileSave", resourceCulture);
            }
        }
        
        /// <summary>
        ///   要求したURLは {0} です。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string LogMessage_ErrorURL {
            get {
                return ResourceManager.GetString("LogMessage_ErrorURL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   {0} より [[{1}]] を取得。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string LogMessage_GetArticle {
            get {
                return ResourceManager.GetString("LogMessage_GetArticle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   言語間リンク無し に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string LogMessage_InterWikiNothing {
            get {
                return ResourceManager.GetString("LogMessage_InterWikiNothing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   記事無し に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string LogMessage_LinkArticleNothing {
            get {
                return ResourceManager.GetString("LogMessage_LinkArticleNothing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   リダイレクト に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string LogMessage_Redirect {
            get {
                return ResourceManager.GetString("LogMessage_Redirect", resourceCulture);
            }
        }
        
        /// <summary>
        ///   {0}、実行日時 {1}
        ///
        /// に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string LogMessage_Start {
            get {
                return ResourceManager.GetString("LogMessage_Start", resourceCulture);
            }
        }
        
        /// <summary>
        ///   
        ///処理を中止しました。このログは {0} に保存されます。
        /// に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string LogMessage_Stop {
            get {
                return ResourceManager.GetString("LogMessage_Stop", resourceCulture);
            }
        }
        
        /// <summary>
        ///   {{{{0}}}} の識別に失敗しました。{{{{0}}}} は {{1}}名前空間の記事して処理します。（{{2}}） に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string LogMessage_TemplateUnknown {
            get {
                return ResourceManager.GetString("LogMessage_TemplateUnknown", resourceCulture);
            }
        }
        
        /// <summary>
        ///    ※キャッシュ に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string LogMessageTranslation {
            get {
                return ResourceManager.GetString("LogMessageTranslation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   対象の記事には、翻訳先言語の記事 [[{0}]] への言語間リンクが存在します。処理を続けますか？ に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string QuestionMessage_ArticleExist {
            get {
                return ResourceManager.GetString("QuestionMessage_ArticleExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   {0}を行います。よろしいですか？ に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string QuestionMessage_CommonWorkQuestion {
            get {
                return ResourceManager.GetString("QuestionMessage_CommonWorkQuestion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   確認 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string QuestionTitle {
            get {
                return ResourceManager.GetString("QuestionTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   → に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string RightArrow {
            get {
                return ResourceManager.GetString("RightArrow", resourceCulture);
            }
        }
        
        /// <summary>
        ///   括弧の書式は、内部に {0} を含む必要があります。また、括弧書きを使用しない場合は、この項目を空欄にしてください。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string WarningMessage_FormatUncompleateBracket {
            get {
                return ResourceManager.GetString("WarningMessage_FormatUncompleateBracket", resourceCulture);
            }
        }
        
        /// <summary>
        ///   翻訳元の言語が指定されていません。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string WarningMessage_NotSelectedSource {
            get {
                return ResourceManager.GetString("WarningMessage_NotSelectedSource", resourceCulture);
            }
        }
        
        /// <summary>
        ///   翻訳先の言語が指定されていません。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string WarningMessage_NotSelectedTarget {
            get {
                return ResourceManager.GetString("WarningMessage_NotSelectedTarget", resourceCulture);
            }
        }
        
        /// <summary>
        ///   翻訳元／先に同じ言語コードが指定されています。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string WarningMessage_SourceEqualTarget {
            get {
                return ResourceManager.GetString("WarningMessage_SourceEqualTarget", resourceCulture);
            }
        }
        
        /// <summary>
        ///   出力先フォルダに無効なパスが指定されています。出力先を確認してください。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string WarningMessage_UnuseSaveDirectory {
            get {
                return ResourceManager.GetString("WarningMessage_UnuseSaveDirectory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   キャッシュ有効期限には0以上の数値を指定してください。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string WarningMessageCacheExpireValue {
            get {
                return ResourceManager.GetString("WarningMessageCacheExpireValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   キャッシュの保存に失敗しました。今回参照したページの情報は、アプリケーション終了まで有効です。
        ///
        ///{0} に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string WarningMessageCacheSaveFailed {
            get {
                return ResourceManager.GetString("WarningMessageCacheSaveFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   翻訳元言語、記事名、翻訳先言語は必須です。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string WarningMessageEmptyTranslationDictionary {
            get {
                return ResourceManager.GetString("WarningMessageEmptyTranslationDictionary", resourceCulture);
            }
        }
        
        /// <summary>
        ///   名前空間のIDには数値を指定してください。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string WarningMessageNamespaceNumberValue {
            get {
                return ResourceManager.GetString("WarningMessageNamespaceNumberValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   言語コードが重複しています。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string WarningMessageRedundantCodeColumn {
            get {
                return ResourceManager.GetString("WarningMessageRedundantCodeColumn", resourceCulture);
            }
        }
        
        /// <summary>
        ///   取得日時には日付または空欄を指定してください。
        ///空欄の場合、この置き換えは無期限で有効になります。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string WarningMessageUnformatedTimestamp {
            get {
                return ResourceManager.GetString("WarningMessageUnformatedTimestamp", resourceCulture);
            }
        }
        
        /// <summary>
        ///   略称を指定する場合、呼称も入力してください。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string WarningMessageUnsetArticleNameColumn {
            get {
                return ResourceManager.GetString("WarningMessageUnsetArticleNameColumn", resourceCulture);
            }
        }
        
        /// <summary>
        ///   言語コードは必須です。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string WarningMessageUnsetCodeColumn {
            get {
                return ResourceManager.GetString("WarningMessageUnsetCodeColumn", resourceCulture);
            }
        }
        
        /// <summary>
        ///   警告 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string WarningTitle {
            get {
                return ResourceManager.GetString("WarningTitle", resourceCulture);
            }
        }
    }
}
