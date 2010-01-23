// ================================================================================================
// <summary>
//      XMLへの設定保存用クラスソース</summary>
//
// <copyright file="Config.cs" company="honeplusのメモ帳">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Models
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// XMLへの設定保存用クラスです。
    /// </summary>
    public class Config
    {
        /// <summary>
        /// クライアントとしての機能関係の設定を保存。
        /// </summary>
        public ClientConfig Client;

        /// <summary>
        /// 言語ごとの情報（サーバーの設定なども）を保存。
        /// </summary>
        [XmlArrayItem(typeof(LanguageInformation)),
        XmlArrayItem(typeof(LanguageWithServerInformation)),
        XmlArrayItem(typeof(WikipediaInformation))]
        public LanguageInformation[] Languages;

        /// <summary>
        /// インスタンスのファイル名。
        /// </summary>
        private string path;

        /// <summary>
        /// コンストラクタ（通常）。
        /// </summary>
        public Config()
        {
            // メンバ変数の領域確保・初期設定
            this.Client = new ClientConfig();
            this.Languages = new LanguageInformation[0];
        }

        /// <summary>
        /// コンストラクタ（ファイル読み込みあり）。
        /// </summary>
        /// <param name="path">設定ファイルパス。</param>
        public Config(string path)
        {
            // ファイルから設定を読み込み
            this.path = Honememo.Cmn.NullCheckAndTrim(path);
            if (this.Load() == false)
            {
                // 失敗した場合、通常のコンストラクタと同じ処理で初期化
                this.Client = new ClientConfig();
                this.Languages = new LanguageInformation[0];
            }
        }

        /// <summary>
        /// プログラムの処理モードを示す列挙値です。
        /// </summary>
        public enum RunType
        {
            /// <summary>
            /// Wikipedia・または姉妹サイト
            /// </summary>
            [XmlEnum(Name = "Wikipedia")]
            Wikipedia
        }

        /// <summary>
        /// 設定をファイルに書き出し。
        /// </summary>
        /// <returns><c>true</c> 書き出し成功</returns>
        public bool Save()
        {
            // 設定をシリアライズ化
            if (this.path == String.Empty)
            {
                return false;
            }

            return Honememo.Cmn.XmlSerialize(this, this.path);
        }

        /// <summary>
        /// 設定をファイルから読み込み。
        /// </summary>
        /// <returns><c>true</c> 読み込み成功</returns>
        public bool Load()
        {
            // 設定をデシリアライズ化
            if (this.path == String.Empty)
            {
                return false;
            }

            object obj = null;
            if (Honememo.Cmn.XmlDeserialize(ref obj, this.GetType(), this.path) == true)
            {
                Config config = obj as Config;
                if (config != null)
                {
                    this.Client = config.Client;
                    this.Languages = config.Languages;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 指定されたコードの言語情報（サーバー情報）を取得。
        /// ※ 存在しない場合、<c>null</c>
        /// </summary>
        /// <param name="code">言語コード。</param>
        /// <param name="mode">処理モード。</param>
        /// <returns>言語情報（サーバー情報）</returns>
        public LanguageInformation GetLanguage(string code, RunType mode)
        {
            Type type;
            if (mode == RunType.Wikipedia)
            {
                type = typeof(WikipediaInformation);
            }
            else
            {
                type = typeof(LanguageInformation);
            }

            foreach (LanguageInformation lang in this.Languages)
            {
                if (lang.GetType() == type)
                {
                    if (lang.Code == code)
                    {
                        return lang;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 指定されたコードの言語情報（サーバー情報）を取得（RunTypeの型）。
        /// </summary>
        /// <param name="code">言語コード。</param>
        /// <returns>言語情報（サーバー情報）</returns>
        public LanguageInformation GetLanguage(string code)
        {
            return this.GetLanguage(code, this.Client.RunMode);
        }

        /// <summary>
        /// クライアントとしての機能関係の設定を格納するクラスです。
        /// </summary>
        public class ClientConfig
        {
            /// <summary>
            /// プログラムの処理対象。
            /// </summary>
            public RunType RunMode;

            /// <summary>
            /// 実行結果を保存するフォルダ。
            /// </summary>
            public string SaveDirectory;

            /// <summary>
            /// 最後に指定していた翻訳元言語。
            /// </summary>
            public string LastSelectedSource;

            /// <summary>
            /// 最後に指定していた翻訳先言語。
            /// </summary>
            public string LastSelectedTarget;

            /// <summary>
            /// 通信時に使用するUserAgent。
            /// </summary>
            public string UserAgent;

            /// <summary>
            /// 通信時に使用するReferer。
            /// </summary>
            public string Referer;

            /// <summary>
            /// コンストラクタ。
            /// </summary>
            public ClientConfig()
            {
                // メンバ変数の領域確保・初期設定
                this.RunMode = RunType.Wikipedia;
                this.SaveDirectory = String.Empty;
                this.LastSelectedSource = "en";
                this.LastSelectedTarget = "ja";
                this.UserAgent = String.Empty;
                this.Referer = String.Empty;
            }
        }
    }
}
