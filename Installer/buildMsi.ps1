<#
    MSIインストーラビルドバッチ

    Wikipedia 翻訳支援ツールのMSI形式のインストーラを作成する。
    実行環境にはWiX並びにWindows SDK（binとサンプルスクリプト）がインストールされている必要がある。
    また、バッチ実行前に別途Visual Studioによるリリースビルドを行っておく必要がある。
#>
$ErrorActionPreference = "Stop"

# WiX 3.14 のパスを検出
$wixPaths = @(
    "C:\Program Files (x86)\WiX Toolset v3.14\bin"
)
$wixBin = $wixPaths | Where-Object { Test-Path $_ } | Select-Object -First 1
if (-not $wixBin) {
    throw "WiX Toolset が見つかりませんでした。WiX 3.14 をインストールしてください。"
}
$env:PATH = "$wixBin;$env:PATH"

# Windows SDK のパスを検出
$sdkRoot = "C:\Program Files (x86)\Windows Kits\10\bin"
$sdkVersion = Get-ChildItem $sdkRoot -Directory |
              Where-Object { $_.Name -match '^\d' } |
              Sort-Object Name -Descending |
              Select-Object -First 1
if (-not $sdkVersion) {
    throw "Windows SDK が見つかりませんでした。Windows 10 SDK をインストールしてください。"
}
$sdkBin = Join-Path $sdkVersion.FullName "x86"
Write-Host "Windows SDK found: $sdkBin"
$env:PATH = "$sdkBin;$env:PATH"

# WiXビルドファイル（以下、ファイル名はこのバッチからの相対パス）
$WXS = "Wptscs.wxs"
$WXL_EN = "Wptscs.en-us.wxl"
$WXL_JA = "Wptscs.ja-jp.wxl"
# MSIインストーラ
$MSI = "wptscs.msi"
# 中間ファイル
$OBJ = "setup.wixobj"
$MSI_EN = "setup-en.msi"
$MSI_JA = "setup-ja.msi"
$MST_JA = "setup-ja.mst"

# スクリプトのフォルダに移動
Set-Location $PSScriptRoot

#コンパイル
candle.exe -out $OBJ $WXS

# リンク（英語版インストーラ）
light.exe -ext WixUIExtension -ext WixNetFxExtension `
    -cultures:en-us -loc $WXL_EN `
    -out $MSI_EN $OBJ

# リンク（日本語版インストーラ）
light.exe -ext WixUIExtension -ext WixNetFxExtension `
    -cultures:ja-jp -loc $WXL_JA `
    -out $MSI_JA $OBJ

# 英語版インストーラを基準に、言語トランスファー情報作成
& "$sdkBin\msitran.exe" -g $MSI_EN $MSI_JA $MST_JA

# 英語版インストーラと言語トランスファー情報から、マルチランゲージインストーラを作成
Copy-Item $MSI_EN $MSI -Force

cscript "$sdkBin\WiSubStg.vbs" $MSI $MST_JA 1041
cscript "$sdkBin\WiLangId.vbs" $MSI Package 1033,1041
