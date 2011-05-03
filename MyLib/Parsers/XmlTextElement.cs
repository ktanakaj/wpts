// ================================================================================================
// <summary>
//      XML中のテキスト要素をあらわすモデルクラスソース</summary>
//
// <copyright file="XmlTextElement.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Parsers
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml;
    using Honememo.Utilities;

    /// <summary>
    /// XML中のテキスト要素をあらわすモデルクラスです。
    /// </summary>
    public class XmlTextElement : TextElement
    {
        #region コンストラクタ

        /// <summary>
        /// 指定されたXMLエンコードされていないテキストを用いて要素を作成する。
        /// </summary>
        /// <param name="text">テキスト文字列、未指定時は<c>null</c>。</param>
        public XmlTextElement(string text = null)
            : base(text)
        {
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// このテキスト要素のテキスト。
        /// </summary>
        /// <remarks>実際のデータは<see cref="Raw"/>に格納。</remarks>
        public override string Text
        {
            get
            {
                if (this.Raw == null)
                {
                    return this.Raw;
                }
                else
                {
                    return XmlUtils.XmlDecode(this.Raw);
                }
            }

            set
            {
                if (value == null)
                {
                    this.Raw = null;
                }
                else
                {
                    this.Raw = XmlUtils.XmlEncode(value);
                }
            }
        }

        /// <summary>
        /// このテキスト要素のエンコードされていない生のテキスト。
        /// </summary>
        public virtual string Raw
        {
            get;
            set;
        }

        #endregion

        #region 実装支援用抽象メソッド実装

        /// <summary>
        /// このテキスト要素のXMLエンコードしたテキストを返す。
        /// </summary>
        /// <returns>このテキスト要素のテキスト。</returns>
        protected override string ToStringImpl()
        {
            return this.Raw;
        }

        #endregion
    }
}
