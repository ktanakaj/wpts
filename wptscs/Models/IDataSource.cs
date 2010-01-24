// ================================================================================================
// <summary>
//      データ取得処理の抽象化用インタフェースソース</summary>
//
// <copyright file="IDataSource.cs" company="honeplusのメモ帳">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// データ取得処理の抽象化用インタフェースです。
    /// </summary>
    public interface IDataSource
    {
        object GetData();


    }
}
