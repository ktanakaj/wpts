# Translation Support for Wikipedia / Wikipedia 翻訳支援ツール
The *Translation Support for Wikipedia* is a Windows application designed to support inter-language translation on [Wikipedia](https://www.wikipedia.org/). It verifies the internal links present in a specified Wikipedia article and retrieves the inter-language links.

『Wikipedia翻訳支援ツール』は、[Wikipedia](https://www.wikipedia.org/) での言語間翻訳をサポートするためのWindows用アプリです。指定されたWikipediaの記事に存在する内部リンク先を確認し、その言語間リンクを取得します。また、登録されていれば、見出しも変換します。  
同じく[MediaWiki](https://www.mediawiki.org/)を使用している [Wiktionary](https://www.wiktionary.org/), [Wikivoyage](https://www.wikivoyage.org/) においても使用可能です。

![日本語版アプリ主画面](https://github.com/ktanakaj/wpts/assets/12389173/b8a9a26a-31d3-4961-8f5a-e0e53077feaf)

**※ 2015年より提供されている[MediaWiki公式のコンテンツ翻訳ツール](https://www.mediawiki.org/wiki/Content_translation/ja)とは異なります。このツールはそれ以前に開発されたものです。**

## Install
* 詳細は「[インストール方法](https://github.com/ktanakaj/wpts/wiki/%E3%82%A4%E3%83%B3%E3%82%B9%E3%83%88%E3%83%BC%E3%83%AB%E6%96%B9%E6%B3%95)」を参照。

Windows 10/11 にて動作を確認。  
.NET Framework 4.7.2 が必要です。

[Releases](https://github.com/ktanakaj/wpts/releases)から最新バージョンのインストーラ (`wptscs132.msi`) をダウンロードして、画面の指示に従いインストールを進めてください。

## Usage
* 詳細は「[使い方](https://github.com/ktanakaj/wpts/wiki/%E4%BD%BF%E3%81%84%E6%96%B9)」を参照。

翻訳元／先言語と、処理結果の出力先フォルダを指定してから、翻訳したい翻訳元記事名を入力して実行してください。  
出力先フォルダに、記事の本文の内部リンク先を置き換えたテキストを出力します。

なお、このプログラムはあくまで「翻訳支援ツール」です。  
出力された文章は、その後Wikipediaの編集画面やプレビューなりで本格的に修正されることを前提としています。  
間違っても、そのままWikipediaに投稿するなどしないでください。  
また、当プログラムではWikipediaへの書き込みは一切行いません。

※ 特にテンプレート等の複雑な書式の記事で使用した場合、一部が正常に処理されない可能性があります。これらの記事で使用する際は、処理結果に異常が無いか注意しながらご利用ください。

## License
[BSD 3-Clause License](https://github.com/ktanakaj/wpts/blob/master/LICENSE)
