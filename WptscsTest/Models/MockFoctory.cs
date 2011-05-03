// ================================================================================================
// <summary>
//      モックオブジェクト生成処理をまとめたファクトリークラスソース</summary>
//
// <copyright file="MockFactory.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Models
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;
    using Honememo.Wptscs.Websites;

    /// <summary>
    /// モックオブジェクト生成処理をまとめたファクトリークラスです。
    /// </summary>
    public class MockFactory
    {
        #region 定数

        /// <summary>
        /// テスト用のconfig.xmlファイルパス。
        /// </summary>
        public static readonly string TestConfigXml = "Data\\config.xml";

        /// <summary>
        /// テストデータが格納されているフォルダパス。
        /// </summary>
        public static readonly string TestMediaWikiDir = "Data\\MediaWiki";

        #endregion

        #region private変数

        /// <summary>
        /// テスト用設定。
        /// </summary>
        private Config config;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// テスト用のconfig.xmlを元に、モックファクトリーを生成する。
        /// </summary>
        public MockFactory()
        {
            this.config = MockFactory.GetConfig(MockFactory.TestConfigXml);
        }

        #endregion

        #region テスト支援静的メソッド

        /// <summary>
        /// ファイルからアプリケーションの設定を取得する。
        /// </summary>
        /// <param name="file">設定ファイル名。</param>
        /// <returns>作成したインスタンス。</returns>
        public static Config GetConfig(string file)
        {
            // 設定ファイルを読み込み
            using (Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                return (Config)new XmlSerializer(typeof(Config)).Deserialize(stream);
            }
        }

        #endregion

        #region テスト支援メソッド

        /// <summary>
        /// アプリケーションの設定を取得する。
        /// </summary>
        /// <returns>作成したインスタンス。</returns>
        public Config GetConfig()
        {
            return this.config;
        }

        /// <summary>
        /// 指定された言語のMediaWikiを取得する。
        /// </summary>
        /// <param name="lang">言語コード。</param>
        /// <returns>ウェブサイトの情報。</returns>
        public MediaWiki GetMediaWiki(string lang)
        {
            Website site = this.config.GetWebsite(lang);
            MediaWiki wiki = null;
            if (site != null)
            {
                wiki = site as MediaWiki;
            }

            if (wiki == null)
            {
                wiki = new MediaWiki(new Language(lang));
            }

            // テスト用にサーバー設定を書き換え
            // ※ フルパスじゃないとURIで取得できないので、ここで書き換える必要有り
            UriBuilder b = new UriBuilder("file", "");
            b.Path = Path.GetFullPath(MockFactory.TestMediaWikiDir) + "\\";
            wiki.Location = new Uri(b.Uri, lang + "/").ToString();
            wiki.ExportPath = "{0}.xml";
            wiki.NamespacePath = "_api.xml";

            return wiki;
        }


        #endregion
    }
}
