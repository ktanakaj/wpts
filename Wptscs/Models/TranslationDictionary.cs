// ================================================================================================
// <summary>
//      言語間の翻訳パターンをあらわすモデルクラスソース</summary>
//
// <copyright file="TranslationDictionary.cs" company="honeplusのメモ帳">
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
    using Honememo.Models;
    using Honememo.Utilities;
    using Honememo.Wptscs.Properties;

    /// <summary>
    /// 言語間の翻訳パターンをあらわすモデルクラスです。
    /// </summary>
    /// <remarks>用途上、大文字小文字の違いは無視する。</remarks>
    public class TranslationDictionary : IgnoreCaseDictionary<TranslationDictionary.Item>, IXmlSerializable
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
        public TranslationDictionary(string from, string to)
        {
            // メンバ変数の初期設定
            this.From = from;
            this.To = to;
        }

        /// <summary>
        /// コンストラクタ（シリアライズ or 拡張用）。
        /// </summary>
        protected TranslationDictionary()
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
                // ※必須な情報が設定されていない場合、例外を返す
                this.from = Validate.NotBlank(value, "from");
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
                // ※必須な情報が設定されていない場合、例外を返す
                this.to = Validate.NotBlank(value, "to");
            }
        }

        #endregion

        #region 静的メソッド

        /// <summary>
        /// コレクションから指定された言語の翻訳パターンを取得する。
        /// 存在しない場合は空のインスタンスを生成、コレクションに追加して返す。
        /// </summary>
        /// <param name="collection">翻訳パターンを含んだコレクション。</param>
        /// <param name="from">翻訳元言語。</param>
        /// <param name="to">翻訳先言語。</param>
        /// <returns>翻訳パターン。存在しない場合は新たに作成した翻訳パターンを返す。</returns>
        public static TranslationDictionary GetDictionaryNeedCreate(
            ICollection<TranslationDictionary> collection, string from, string to)
        {
            // 設定が存在すれば取得した値を返す
            foreach (TranslationDictionary d in collection)
            {
                if (d.From == from && d.To == to)
                {
                    return d;
                }
            }

            // 存在しない場合、作成した翻訳パターンをコレクションに追加し、返す
            TranslationDictionary dic = new TranslationDictionary(from, to);
            collection.Add(dic);
            return dic;
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
            this.From = tableElement.GetAttribute("From");
            this.To = tableElement.GetAttribute("To");

            // 各対訳の読み込み
            foreach (XmlNode itemNode in tableElement.SelectNodes("Item"))
            {
                XmlElement itemElement = itemNode as XmlElement;
                Item item = new Item();
                item.Word = itemElement.GetAttribute("To");
                item.Alias = itemElement.GetAttribute("Redirect");
                string timestamp = itemElement.GetAttribute("Timestamp");
                if (!String.IsNullOrEmpty(timestamp))
                {
                    item.Timestamp = DateTime.Parse(timestamp);

                    // 登録日時が有効期限より古い場合は破棄する
                    if (DateTime.Now - Settings.Default.CacheExpire > item.Timestamp.Value)
                    {
                        continue;
                    }
                }

                this[itemElement.GetAttribute("From")] = item;
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
            foreach (KeyValuePair<string, Item> item in this)
            {
                writer.WriteStartElement("Item");
                writer.WriteAttributeString("From", item.Key);
                writer.WriteAttributeString("To", item.Value.Word);
                if (!String.IsNullOrWhiteSpace(item.Value.Alias))
                {
                    writer.WriteAttributeString("Redirect", item.Value.Alias);
                }

                if (item.Value.Timestamp.HasValue)
                {
                    writer.WriteAttributeString(
                        "Timestamp",
                        item.Value.Timestamp.Value.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'"));
                }

                writer.WriteEndElement();
            }
        }

        #endregion

        #region 構造体

        /// <summary>
        /// 対訳表の翻訳先をあらわす構造体です。
        /// </summary>
        public struct Item
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
            public string Alias;
        }

        #endregion
    }
}
