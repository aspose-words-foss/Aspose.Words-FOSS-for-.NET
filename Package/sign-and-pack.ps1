# Builds, signs and packs the Aspose.Words.FOSS NuGet package.
#
# Prerequisites:
#   - .NET 10 SDK.
#   - The Aspose code-signing certificate available in the CurrentUser\My store.
#     The certificate lives in the SSL.com eSigner cloud; install "eSigner CKA" and
#     sign in - the certificate then appears in the store with a cloud-backed key.
#     No key material or credentials are stored in this repository.
#
# The DLLs inside the package are deliberately NOT Authenticode-signed: the author
# signature on the package itself covers their integrity, and none of the sibling
# asposefoss packages sign their DLLs either.
#
# Usage:
#   powershell -File Package\sign-and-pack.ps1            # build + pack + sign + verify
#   powershell -File Package\sign-and-pack.ps1 -NoSign    # unsigned package (local testing)

param(
    [switch]$NoSign
)

$ErrorActionPreference = "Stop"

# Public identifier only: the fingerprint is printed in every published signature.
$CertSha256 = "0A46827552AB8684C75B19E7B6268E2AB5884589727691BD08618B3E49D905AA"
# RFC 3161 timestamping. The /legacy endpoint returns a TSA cert with an RSA key
# >= 2048 bits, which NuGet requires (NU3023).
$TimestamperNuGet = "http://ts.ssl.com/legacy"

$RepoRoot = Split-Path $PSScriptRoot -Parent
$PackageDir = $PSScriptRoot
$OutDir = Join-Path $PackageDir "out"
$Nuspec = Join-Path $PackageDir "Aspose.Words.FOSS.nuspec"
$Version = ([xml](Get-Content $Nuspec)).package.metadata.version

Write-Host "=== Aspose.Words.FOSS $Version" -ForegroundColor Cyan

# 1. Build (Release produces the merged single-file assemblies via ILRepack).
Write-Host "=== Building Release" -ForegroundColor Cyan
dotnet build (Join-Path $RepoRoot "Aspose.Words\Aspose.Words.csproj") -c Release
if ($LASTEXITCODE -ne 0) { throw "Build failed." }

# 2. Pack.
Write-Host "=== Packing" -ForegroundColor Cyan
$nupkg = Join-Path $OutDir "Aspose.Words.FOSS.$Version.nupkg"
if (Test-Path $nupkg) { Remove-Item $nupkg }
dotnet pack (Join-Path $RepoRoot "Aspose.Words\Aspose.Words.csproj") -c Release --no-build -o $OutDir `
    -p:NuspecFile="$Nuspec" -p:NuspecBasePath="$PackageDir"
if ($LASTEXITCODE -ne 0) { throw "Pack failed." }

# 3. Sign the package (nuget.org rejects unsigned packages for this account).
if (-not $NoSign) {
    Write-Host "=== Signing package" -ForegroundColor Cyan
    dotnet nuget sign $nupkg --certificate-fingerprint $CertSha256 --timestamper $TimestamperNuGet --hash-algorithm SHA256
    if ($LASTEXITCODE -ne 0) { throw "Package signing failed." }

    Write-Host "=== Verifying" -ForegroundColor Cyan
    dotnet nuget verify $nupkg
    if ($LASTEXITCODE -ne 0) { throw "Package verification failed." }
}

Write-Host "=== Done: $nupkg" -ForegroundColor Green
Write-Host "Push with: dotnet nuget push `"$nupkg`" --source https://api.nuget.org/v3/index.json --api-key <key>"
