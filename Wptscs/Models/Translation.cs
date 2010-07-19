// ================================================================================================
// <summary>
//      言語間の対訳表をあらわすモデルクラスソース</summary>
//
// <copyright file="Translation.cs" company="honeplusのメモ帳">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Models
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Serialization;
    using Honememo.Wptscs.Properties;

    /// <summary>
    /// 言語間の対訳表をあらわすモデルクラスです。
    /// </summary>
    public class Translation : Dictionary<string, Translation.Goal>, IXmlSerializable
    {
        #region private変数

        /// <summary>
        /// 翻訳元言語コード。
        /// </summary>
        private string from;

        /// <summary>
        /// 翻訳先言語コード。
        /// </summary>
        private string to;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ（通常）。
        /// </summary>
        /// <param name="from">翻訳元言語コード。</param>
        /// <param name="to">翻訳先言語コード。</param>
        public Translation(string from, string to)
        {
            // メンバ変数の初期設定
            this.From = from;
            this.To = to;
        }

        /// <summary>
        /// コンストラクタ（シリアライズ or 拡張用）。
        /// </summary>
        protected Translation()
        {
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// 翻訳元言語コード。
        /// </summary>
        public string From
        {
            get
            {
                return this.from;
            }

            set
            {
                // ※必須な情報が設定されていない場合、ArgumentNullExceptionを返す
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("from");
                }

                this.from = value;
            }
        }

        /// <summary>
        /// 翻訳先言語コード。
        /// </summary>
        public string To
        {
            get
            {
                return this.to;
            }

            set
            {
                // ※必須な情報が設定されていない場合、ArgumentNullExceptionを返す
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("to");
                }

                this.to = value;
            }
        }

        #endregion

        #region XMLシリアライズ用メソッド

        /// <summary>
        /// シリアライズするXMLのスキーマ定義を返す。
        /// </summary>
        /// <returns>XML表現を記述するXmlSchema。</returns>
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// XMLからオブジェクトをデシリアライズする。
        /// </summary>
        /// <param name="reader">デシリアライズ元のXmlReader</param>
        public void ReadXml(XmlReader reader)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(reader);

            // ※ 以下、基本的に無かったらNGの部分はいちいちチェックしない。例外飛ばす
            XmlElement tableElement = xml.SelectSingleNode("Translation") as XmlElement;
            this.From = tableElement.GetAttribute("From");
            this.To = tableElement.GetAttribute("To");

            // 各対訳の読み込み
            foreach (XmlNode itemNode in tableElement.SelectNodes("Item"))
            {
                XmlElement itemElement = itemNode as XmlElement;
                Goal goal = new Goal();
                goal.Word = itemElement.GetAttribute("To");
                goal.Redirect = itemElement.GetAttribute("Redirect");
                string timestamp = itemElement.GetAttribute("Timestamp");
                if (!String.IsNullOrEmpty(timestamp))
                {
                    goal.Timestamp = DateTime.Parse(timestamp);

                    // 登録日時が有効期限より古い場合は破棄する
                    if (DateTime.Now - Settings.Default.CacheExpire > goal.Timestamp.Value)
                    {
                        continue;
                    }
                }

                this[itemElement.GetAttribute("From")] = goal;
            }
        }

        /// <summary>
        /// オブジェクトをXMLにシリアライズする。
        /// </summary>
        /// <param name="writer">シリアライズ先のXmlWriter</param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("From", this.From);
            writer.WriteAttributeString("To", this.To);

            // 各対訳の出力
            foreach (KeyValuePair<string, Goal> item in this)
            {
                writer.WriteStartElement("Item");
                writer.WriteAttributeString("From", item.Key);
                writer.WriteAttributeString("To", item.Value.Word);
                if (!String.IsNullOrWhiteSpace(item.Value.Redirect))
                {
                    writer.WriteAttributeString("Redirect", item.Value.Redirect);
                }

                if (item.Value.Timestamp.HasValue)
                {
                    writer.WriteAttributeString("Timestamp", item.Value.Timestamp.Value.ToString("U"));
                }

                writer.WriteEndElement();
            }
        }

        #endregion

        #region 構造体

        /// <summary>
        /// 対訳表の翻訳先をあらわす構造体です。
        /// </summary>
        public struct Goal
        {
            /// <summary>
            /// 翻訳先語句。
            /// </summary>
            /// <remarks>翻訳先の語句が存在しないことを明示する場合<c>null</c>または空。</remarks>
            public string Word;

            /// <summary>
            /// 登録日時。
            /// </summary>
            /// <remarks>有効期間としても使用。無期限である場合<c>null</c>。</remarks>
            public DateTime? Timestamp;

            /// <summary>
            /// 翻訳先別名。
            /// </summary>
            /// <remarks>Wikipediaのリダイレクト等を意図。別名が無い場合<c>null</c>または空。</remarks>
            public string Redirect;
        }

        #endregion
    }
}
