$NugetPath = ".\.nuget\NuGet.exe";

$OutputDir = ".\GeneratedPackages";
Remove-Item  "$OutputDir" -Recurse -Force;
New-Item "$OutputDir" -type directory -Force;

$ProjectPaths = Get-ChildItem -Recurse -Path "*.csproj";
foreach($i in $ProjectPaths)
{
    $NuspecExists = (Get-ChildItem -Path $i.Directory -Filter "*.nuspec").Count -gt 0;
    if($NuspecExists)
    {
        &$NugetPath pack "$i" -IncludeReferencedProjects -Prop Configuration=Release -OutputDirectory "$OutputDir";
    }
}

$PackagesPaths = Get-ChildItem -Path "$OutputDir" -Filter "*.nupkg";
foreach($i in $PackagesPaths)
{
    &$NugetPath push $i.FullName;
}