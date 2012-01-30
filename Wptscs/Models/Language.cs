// ================================================================================================
// <summary>
//      言語に関する情報をあらわすモデルクラスソース</summary>
//
// <copyright file="Language.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
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
    using Honememo.Wptscs.Properties;

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

        /// <summary>
        /// 括弧のフォーマット。
        /// </summary>
        private string bracket;

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
                // ※必須な情報が設定されていない場合、例外を返す
                this.code = Validate.NotBlank(value, "code").ToLower();
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
                // ※必須な情報が設定されていない場合、例外を返す
                this.names = Validate.NotNull(value, "names");
            }
        }

        /// <summary>
        /// 括弧のフォーマット。
        /// </summary>
        /// <remarks>値が指定されていない場合、デフォルト値を返す。</remarks>
        public string Bracket
        {
            get
            {
                if (String.IsNullOrWhiteSpace(this.bracket))
                {
                    return Settings.Default.Bracket;
                }

                return this.bracket;
            }

            set
            {
                this.bracket = value;
            }
        }

        #endregion

        #region 公開メソッド

        /// <summary>
        /// <see cref="Bracket"/> を渡された値で書式化した文字列を返す。
        /// </summary>
        /// <param name="value">記事名。</param>
        /// <returns>書式化した文字列。<see cref="Bracket"/>が未設定の場合<c>null</c>。</returns>
        public string FormatBracket(string value)
        {
            return StringUtils.FormatDollarVariable(this.Bracket, value);
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
            XmlElement langElement = xml.DocumentElement;
            this.Code = langElement.GetAttribute("Code");
            this.Bracket = XmlUtils.InnerText(langElement.SelectSingleNode("Bracket"));

            // 言語の呼称情報
            foreach (XmlNode nameNode in langElement.SelectNodes("Names/LanguageName"))
            {
                XmlElement nameElement = nameNode as XmlElement;
                this.Names[nameElement.GetAttribute("Code")] = new LanguageName
                {
                    Name = XmlUtils.InnerText(nameElement.SelectSingleNode("Name")),
                    ShortName = XmlUtils.InnerText(nameElement.SelectSingleNode("ShortName"))
                };
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
            foreach (KeyValuePair<string, LanguageName> name in this.Names)
            {
                writer.WriteStartElement("LanguageName");
                writer.WriteAttributeString("Code", name.Key);
                writer.WriteElementString("Name", name.Value.Name);
                writer.WriteElementString("ShortName", name.Value.ShortName);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.WriteElementString("Bracket", this.bracket);
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
