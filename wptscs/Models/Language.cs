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
    using System.Xml.Serialization;

    /// <summary>
    /// 言語に関する情報をあらわすモデルクラスです。
    /// </summary>
    public class Language : IComparable
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
        /// コンストラクタ（シリアライズ用）。
        /// </summary>
        public Language()
            : this("unknown")
        {
            // ↑適当な値で通常のコンストラクタを実行
            System.Diagnostics.Debug.WriteLine("Language.Language : 推奨されないコンストラクタを使用しています");
        }

        /// <summary>
        /// コンストラクタ（通常）。
        /// </summary>
        /// <param name="code">言語のコード。。</param>
        public Language(string code)
        {
            // メンバ変数の初期設定
            this.Code = code;
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// 言語のコード。
        /// </summary>
        [XmlAttributeAttribute("Code")]
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

        #region メソッド

        /// <summary>
        /// 配列のソート用メソッド。
        /// </summary>
        /// <param name="obj">比較対象のオブジェクト。</param>
        /// <returns>比較対象オブジェクトの相対順序を示す整数値。</returns>
        public int CompareTo(object obj)
        {
            // 言語コードでソート
            Language lang = obj as Language;
            return this.Code.CompareTo(lang.Code);
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
