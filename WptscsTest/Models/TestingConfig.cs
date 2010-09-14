// ================================================================================================
// <summary>
//      テスト用に拡張したConfigクラスソース。</summary>
//
// <copyright file="TestingConfig.cs.cs" company="honeplusのメモ帳">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Honememo.Wptscs.Models
{
    /// <summary>
    /// テスト用に拡張したConfigクラスです。
    /// </summary>
    public class TestingConfig : Config
    {        
        #region テスト支援メソッド

        /// <summary>
        /// アプリケーションの設定を取得する。
        /// </summary>
        /// <param name="file">設定ファイル名。</param>
        /// <returns>作成したインスタンス。</returns>
        /// <remarks>テスト用のため、特に親クラスのようなシングルトンといった制御はせず。</remarks>
        public static new Config GetInstance(string file)
        {
            // 設定ファイルを読み込み
            using (Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                return new XmlSerializer(typeof(Config)).Deserialize(stream) as Config;
            }
        }

        #endregion
    }
}
