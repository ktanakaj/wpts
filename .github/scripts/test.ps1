<#
    CLI用のユニットテストスクリプト。

    .NET Frameworkプロジェクトのユニットテストは、CLIでは
    vstestでビルド済のDLLを指定して実行する必要があるため、専用のスクリプトを用意。
#>
# インストールされている vstest.console.exe のパスを探索
$vsRoot = "C:\Program Files\Microsoft Visual Studio"
$vsVersions = Get-ChildItem $vsRoot -Directory |
                Where-Object { $_.Name -match '^\d+$' } |
                Sort-Object Name -Descending
$editions = @("Enterprise", "Professional", "Community")
$relativePaths = @(
    "Common7\IDE\Extensions\TestPlatform\vstest.console.exe",
    "Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe"
)

$vstest = $null

foreach ($ver in $vsVersions) {
    foreach ($edition in $editions) {
        foreach ($rel in $relativePaths) {
            $path = Join-Path $vsRoot "$($ver.Name)\$edition\$rel"
            if (Test-Path $path) {
                $vstest = $path
                break
            }
        }
        if ($vstest) { break }
    }
    if ($vstest) { break }
}

if (-not $vstest) {
  Write-Error "VSTest not found at expected location."
  exit 1
}

# ユニットテストプロジェクトのビルド済みのDLLを探索
# （二重実行防止のため、Debugビルドのみ対象）
$dlls = Get-ChildItem -Recurse -Filter "*Test.dll" |
          Where-Object { $_.FullName -match "\\bin\\Debug\\" } |
          Select-Object -ExpandProperty FullName

if ($dlls.Count -eq 0) {
    Write-Error "*Test.dll not found in bin/Debug directories."
    exit 1
}

# ユニットテストを実行
& "$vstest" $dlls
exit $LASTEXITCODE