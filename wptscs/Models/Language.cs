// ================================================================================================
// <summary>
//      言語に関する情報をあらわすモデルクラスソース</summary>
//
// <copyright file="Language.cs" company="honeplusのメモ帳">
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
    /// 言語に関する情報をあらわすモデルクラスです。
    /// </summary>
    public class Language : IXmlSerializable
    {
        #region private変数

        /// <summary>
        /// 言語のコード。
        /// </summary>
        private string code;

        /// <summary>
        /// この言語の、各言語での名称。
        /// </summary>
        private IDictionary<string, LanguageName> names = new Dictionary<string, LanguageName>();

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ（通常）。
        /// </summary>
        /// <param name="code">言語のコード。</param>
        public Language(string code)
        {
            // メンバ変数の初期設定
            this.Code = code;
        }

        /// <summary>
        /// コンストラクタ（シリアライズ or 拡張用）。
        /// </summary>
        protected Language()
        {
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// 言語のコード。
        /// </summary>
        public string Code
        {
            get
            {
                return this.code;
            }

            set
            {
                // ※必須な情報が設定されていない場合、ArgumentNullExceptionを返す
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("code");
                }

                this.code = value.ToLower();
            }
        }

        /// <summary>
        /// この言語の、各言語での名称。
        /// </summary>
        /// <remarks>空でもオブジェクトは存在。</remarks>
        public IDictionary<string, LanguageName> Names
        {
            get
            {
                return this.names;
            }

            set
            {
                // ※必須な情報が設定されていない場合、ArgumentNullExceptionを返す
                if (value == null)
                {
                    throw new ArgumentNullException("names");
                }

                this.names = value;
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

            // Webサイトの言語情報
            // ※ 以下、基本的に無かったらNGの部分はいちいちチェックしない。例外飛ばす
            XmlElement langElement = xml.SelectSingleNode("Language") as XmlElement;
            this.Code = langElement.GetAttribute("Code");

            // 言語の呼称情報
            foreach (XmlNode nameNode in langElement.SelectNodes("Names/LanguageName"))
            {
                XmlElement nameElement = nameNode as XmlElement;
                Language.LanguageName name = new Language.LanguageName();
                name.Name = XmlUtils.InnerText(nameElement.SelectSingleNode("Name"));
                name.ShortName = XmlUtils.InnerText(nameElement.SelectSingleNode("ShortName"));
                this.Names[nameElement.GetAttribute("Code")] = name;
            }
        }

        /// <summary>
        /// オブジェクトをXMLにシリアライズする。
        /// </summary>
        /// <param name="writer">シリアライズ先のXmlWriter</param>
        public void WriteXml(XmlWriter writer)
        {
            // Webサイトの言語情報
            writer.WriteAttributeString("Code", this.Code);

            // 言語の呼称情報
            writer.WriteStartElement("Names");
            foreach (KeyValuePair<string, Language.LanguageName> name in this.Names)
            {
                writer.WriteStartElement("LanguageName");
                writer.WriteAttributeString("Code", name.Key);
                writer.WriteElementString("Name", name.Value.Name);
                writer.WriteElementString("ShortName", name.Value.ShortName);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        #endregion

        #region 構造体

        /// <summary>
        /// ある言語の、各言語での名称・略称を格納するための構造体です。
        /// </summary>
        public struct LanguageName
        {
            /// <summary>
            /// 言語の名称。
            /// </summary>
            public string Name;

            /// <summary>
            /// 言語の略称。
            /// </summary>
            public string ShortName;
        }

        #endregion
    }
}
