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
    public class Translation : IXmlSerializable
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

        /// <summary>
        /// 対訳パターン。
        /// </summary>
        private IDictionary<string, Goal> table = new Dictionary<string, Goal>();

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
                this.to = value;
            }
        }

        /// <summary>
        /// 対訳パターン。
        /// </summary>
        public IDictionary<string, Goal> Table
        {
            get
            {
                return this.table;
            }

            set
            {
                this.table = value;
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

            XmlElement tableElement = xml.SelectSingleNode("Translation") as XmlElement;
            if (tableElement == null)
            {
                return;
            }

            this.From = tableElement.GetAttribute("From");
            this.To = tableElement.GetAttribute("To");

            // 各対訳の読み込み
            foreach (XmlNode itemNode in tableElement.SelectNodes("Item"))
            {
                XmlElement itemElement = itemNode as XmlElement;
                Goal goal = new Goal();
                goal.Word = itemElement.GetAttribute("To");
                string timestamp = itemElement.GetAttribute("Timestamp");
                if (String.IsNullOrEmpty(timestamp))
                {
                    goal.Timestamp = DateTime.Parse(timestamp);

                    // 登録日時が有効期限より古い場合は破棄する
                    if (goal.Timestamp.Value + Settings.Default.CacheExpire > DateTime.Now)
                    {
                        continue;
                    }
                }

                this.Table[itemElement.GetAttribute("From")] = goal;
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
            foreach (KeyValuePair<string, Goal> item in this.Table)
            {
                writer.WriteStartElement("Item");
                writer.WriteAttributeString("From", item.Key);
                writer.WriteAttributeString("To", item.Value.Word);
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
            /// <remarks>翻訳先の語句が存在しないことを明示する場合<c>null</c>。</remarks>
            public string Word;

            /// <summary>
            /// 登録日時。
            /// </summary>
            public DateTime? Timestamp;
        }

        #endregion
    }
}
