// ================================================================================================
// <summary>
//      ページ要素を複数格納する要素をあらわすモデルクラスソース</summary>
//
// <copyright file="ListElement.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Honememo.Utilities;

    /// <summary>
    /// ページ要素を複数格納する要素をあらわすモデルクラスです。
    /// </summary>
    public class ListElement : List<IElement>, IElement
    {
        #region インタフェース実装プロパティ
        
        /// <summary>
        /// 要素がParse等により生成された場合の解析元文字列。
        /// </summary>
        /// <remarks>
        /// <see cref="ToString"/>に生成した値を返して欲しい場合、この値を明示的に<c>null</c>にすべき。
        /// 元の文字列と完全に同じ文字列を生成できるクラスであれば、常に未設定でも問題ない。
        /// </remarks>
        public virtual string ParsedString
        {
            get;
            set;
        }

        #endregion

        #region インタフェース実装メソッド

        /// <summary>
        /// 各要素を書式化したテキスト形式で返す。
        /// </summary>
        /// <returns>書式化したテキスト。<c>null</c>は返さない。</returns>
        /// <remarks>
        /// <see cref="ParsedString"/>が設定されている場合は、その値を返す。
        /// </remarks>
        public override string ToString()
        {
            return this.ParsedString != null ? this.ParsedString : StringUtils.DefaultString(this.ToStringImpl());
        }

        #endregion

        #region 内部実装メソッド

        /// <summary>
        /// この要素に格納されている要素のToStringを連結して返す。
        /// </summary>
        /// <returns>この要素に格納されている要素のテキスト。</returns>
        protected virtual string ToStringImpl()
        {
            StringBuilder b = new StringBuilder();
            foreach (IElement element in this)
            {
                b.Append(element.ToString());
            }

            return b.ToString();
        }

        #endregion
    }
}
