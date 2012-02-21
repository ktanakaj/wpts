// ================================================================================================
// <summary>
//      言語間の対訳表をあらわすモデルクラスソース</summary>
//
// <copyright file="TranslationTable.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using System.Xml.Serialization;
    using Honememo.Utilities;

    /// <summary>
    /// 言語間の対訳表をあらわすモデルクラスです。
    /// </summary>
    public class TranslationTable : List<IDictionary<string, string[]>>, IXmlSerializable
    {
        #region プロパティ
        
        /// <summary>
        /// 翻訳元言語コード。
        /// </summary>
        /// <remarks><see cref="GetWord(string)"/>の呼び出しを簡略化するためのプロパティ。</remarks>
        public string From
        {
            get;
            set;
        }

        /// <summary>
        /// 翻訳先言語コード。
        /// </summary>
        /// <remarks><see cref="GetWord(string)"/>の呼び出しを簡略化するためのプロパティ。</remarks>
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
        /// <param name="word">翻訳元語句。</param>
        /// <returns>対訳語句。登録されていない場合 <c>null</c>。</returns>
        /// <exception cref="ArgumentNullException"><para>from</para>, <para>to</para>, <para>word</para>のいずれかが<c>null</c>の場合。</exception>
        /// <remarks><para>word</para>の大文字小文字は区別しない。</remarks>
        public string GetWord(string from, string to, string word)
        {
            // nullは不可。以降でエラーになるためここでチェック
            Validate.NotNull(from, "from");
            Validate.NotNull(to, "to");
            string w = Validate.NotNull(word, "word").ToLower();

            // 翻訳元言語の項目を探索
            foreach (IDictionary<string, string[]> record in this)
            {
                if (record.ContainsKey(from) && CollectionUtils.ContainsIgnoreCase(record[from], w))
                {
                    // 翻訳元を発見した場合、それに対応する翻訳先の語句を返す
                    string s = null;
                    if (record.ContainsKey(to))
                    {
                        // 代表で先頭の値を取得
                        s = record[to].First();
                    }

                    return s;
                }
            }

            return null;
        }

        /// <summary>
        /// 指定されている言語の組み合わせで対訳語を取得する。
        /// </summary>
        /// <param name="word">翻訳元語。</param>
        /// <returns>対訳語。登録されていない場合 <c>null</c>。</returns>
        /// <exception cref="InvalidOperationException"><see cref="From"/>, <see cref="To"/>のいずれかが空の場合。</exception>
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
                IDictionary<string, string[]> record = new Dictionary<string, string[]>();
                foreach (XmlNode wordNode in recordNode)
                {
                    // 一つの言語に複数の値が登録可能なため、その場合配列に積む
                    XmlElement wordElement = wordNode as XmlElement;
                    string lang = wordElement.GetAttribute("Lang");
                    string word = wordElement.InnerText;
                    List<string> list = new List<string>();
                    string[] words;
                    if (record.TryGetValue(lang, out words))
                    {
                        list.AddRange(words);
                    }

                    // 既に登録されている場合、代表であれば先頭に、それ以外なら後ろに追加
                    bool head;
                    if (bool.TryParse(wordElement.GetAttribute("Head"), out head) && head)
                    {
                        list.Insert(0, word);
                    }
                    else
                    {
                        list.Add(word);
                    }

                    record[lang] = list.ToArray();
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
            foreach (IDictionary<string, string[]> record in this)
            {
                writer.WriteStartElement("Group");
                foreach (KeyValuePair<string, string[]> words in record)
                {
                    bool first = true;
                    foreach (string word in words.Value)
                    {
                        writer.WriteStartElement("Word");
                        writer.WriteAttributeString("Lang", words.Key);
                        if (first && words.Value.Length > 1)
                        {
                            // 先頭項目は変換先として使用されるため、複数ある場合は代表フラグを出力
                            // ※ 2番目以降は同格のため、先頭以外の並び順は保障しない
                            writer.WriteAttributeString("Head", bool.TrueString);
                            first = false;
                        }

                        writer.WriteValue(word);
                        writer.WriteEndElement();
                    }
                }

                writer.WriteEndElement();
            }
        }

        #endregion
    }
}
