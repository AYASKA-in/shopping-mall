param(
    [string]$DistDir = "$PSScriptRoot\dist",
    [string]$TemplateFile = "$PSScriptRoot\Product.template.wxs",
    [string]$OutputFile = "$PSScriptRoot\Product.wxs"
)

$ErrorActionPreference = "Stop"

function Get-RelativePath {
    param([string]$From, [string]$To)
    $fromUri = New-Object Uri(($From.TrimEnd('\') + '\'))
    $toUri = New-Object Uri($To)
    $rel = $fromUri.MakeRelativeUri($toUri).ToString()
    return [System.Uri]::UnescapeDataString($rel) -replace '/', '\'
}

function Sanitize-Id {
    param([string]$s)
    $s = $s -replace '[^a-zA-Z0-9_]', '_'
    if ($s -match '^[0-9]') { $s = "_$s" }
    return $s
}

function Generate-FileComponents {
    param($Files, $IdPrefix, $StripPrefix)
    $lines = @()
    foreach ($f in $Files) {
        $source = $f
        if ($StripPrefix -and $f.StartsWith($StripPrefix)) {
            $source = $f.Substring($StripPrefix.Length)
        }
        $id = Sanitize-Id "cmp_$IdPrefix$($source -replace '\\', '_' -replace '\.', '_')"
        $fid = Sanitize-Id "fil_$IdPrefix$($source -replace '\\', '_' -replace '\.', '_')"
        $fname = [System.IO.Path]::GetFileName($f)
        $ext = [System.IO.Path]::GetExtension($f).ToLower()
        $keyPath = if ($ext -eq '.exe') { ' KeyPath="yes"' } else { '' }
        $lines += "      <Component Id=`"$id`" Guid=`"*`">"
        $lines += "        <File Id=`"$fid`" Name=`"$fname`" Source=`"$f`"$keyPath />"
        $lines += "      </Component>"
    }
    return $lines -join "`r`n"
}

$client = @()
$server = @()
$cloud = @()

Get-ChildItem -Path $DistDir -Recurse -File | Where-Object {
    $_.Extension -notin '.pdb', '.xml' -or $_.Name -eq 'appsettings.json'
} | ForEach-Object {
    $rel = Get-RelativePath $DistDir $_.FullName
    $parts = $rel -split '\\'
    if ($parts[0] -eq 'Server') {
        $server += $rel
    } elseif ($parts[0] -eq 'CloudDashboard') {
        $cloud += $rel
    } else {
        $client += $rel
    }
}

$template = Get-Content $TemplateFile -Raw
$template = $template -replace '<!-- INSERT_CLIENT_FILES_HERE -->', (Generate-FileComponents $client "" "")
$template = $template -replace '<!-- INSERT_SERVER_FILES_HERE -->', (Generate-FileComponents $server "srv_" "")
$template = $template -replace '<!-- INSERT_CLOUD_FILES_HERE -->', (Generate-FileComponents $cloud "cld_" "")

Set-Content $OutputFile $template -NoNewline
Write-Host "Generated $OutputFile with $($client.Count) client, $($server.Count) server, $($cloud.Count) cloud files."
