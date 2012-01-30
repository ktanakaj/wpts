// ================================================================================================
// <summary>
//      言語間の対訳表をあらわすモデルクラスソース</summary>
//
// <copyright file="TranslationTable.cs" company="honeplusのメモ帳">
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
    using Honememo.Utilities;

    /// <summary>
    /// 言語間の対訳表をあらわすモデルクラスです。
    /// </summary>
    public class TranslationTable : List<IDictionary<string, string>>, IXmlSerializable
    {
        #region プロパティ
        
        /// <summary>
        /// 翻訳元言語コード。
        /// </summary>
        public string From
        {
            get;
            set;
        }

        /// <summary>
        /// 翻訳先言語コード。
        /// </summary>
        public string To
        {
            get;
            set;
        }

        #endregion

        #region 公開メソッド

        /// <summary>
        /// 指定された言語の対訳語を取得する。
        /// </summary>
        /// <param name="from">翻訳元言語コード。</param>
        /// <param name="to">翻訳先言語コード。</param>
        /// <param name="word">翻訳元語。</param>
        /// <returns>対訳語。登録されていない場合 <c>null</c>。</returns>
        /// <remarks>大文字小文字は区別しない。</remarks>
        public string GetWord(string from, string to, string word)
        {
            // nullは不可。以降でエラーになるためここでチェック
            Validate.NotNull(from, "from");
            Validate.NotNull(to, "to");
            Validate.NotNull(word, "word");

            string w = word.ToLower();
            foreach (IDictionary<string, string> record in this)
            {
                if (record.ContainsKey(from) && record[from].ToLower() == w)
                {
                    string c = null;
                    if (record.ContainsKey(to))
                    {
                        c = record[to];
                    }

                    return c;
                }
            }

            return null;
        }

        /// <summary>
        /// 指定されている言語の組み合わせで対訳語を取得する。
        /// </summary>
        /// <param name="word">翻訳元語。</param>
        /// <returns>対訳語。登録されていない場合 <c>null</c>。</returns>
        /// <remarks>大文字小文字は区別しない。</remarks>
        public string GetWord(string word)
        {
            if (String.IsNullOrEmpty(this.From) || String.IsNullOrEmpty(this.To))
            {
                throw new InvalidOperationException("empty from or to");
            }

            return this.GetWord(this.From, this.To, word);
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
            XmlElement tableElement = xml.DocumentElement;

            // 各対訳の読み込み
            this.Clear();
            foreach (XmlNode recordNode in tableElement.SelectNodes("Group"))
            {
                IDictionary<string, string> record = new Dictionary<string, string>();
                foreach (XmlNode wordNode in recordNode)
                {
                    XmlElement wordElement = wordNode as XmlElement;
                    record[wordElement.GetAttribute("Lang")] = wordElement.InnerText;
                }

                this.Add(record);
            }
        }

        /// <summary>
        /// オブジェクトをXMLにシリアライズする。
        /// </summary>
        /// <param name="writer">シリアライズ先のXmlWriter</param>
        public void WriteXml(XmlWriter writer)
        {
            // 各対訳の出力
            foreach (IDictionary<string, string> record in this)
            {
                writer.WriteStartElement("Group");
                foreach (KeyValuePair<string, string> word in record)
                {
                    writer.WriteStartElement("Word");
                    writer.WriteAttributeString("Lang", word.Key);
                    writer.WriteValue(word.Value);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }

        #endregion
    }
}
