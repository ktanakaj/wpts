// ================================================================================================
// <summary>
//      IElementを実装するための実装支援用抽象クラスソース</summary>
//
// <copyright file="AbstractElement.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Parsers
{
    using System;
    using Honememo.Utilities;

    /// <summary>
    /// <see cref="IElement"/>を実装するための実装支援用抽象クラスです。
    /// </summary>
    public abstract class AbstractElement : IElement
    {        
        #region インタフェース実装プロパティ

        /// <summary>
        /// 要素が<see cref="IParser.Parse"/>等により生成された場合の解析元文字列。
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

        #region 実装支援用抽象メソッド

        /// <summary>
        /// 各要素を書式化したテキスト形式で返す。
        /// </summary>
        /// <returns>書式化したテキスト。</returns>
        protected abstract string ToStringImpl();

        #endregion
    }
}
