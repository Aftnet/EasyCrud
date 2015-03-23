$NugetPath = ".\.nuget\NuGet.exe";

$ProjectPaths = Get-ChildItem -Recurse -Path "*.csproj";
foreach($i in $ProjectPaths)
{
    $NuspecExists = (Get-ChildItem -Path $i.Directory -Filter "*.nuspec").Count -gt 0;
    if($NuspecExists)
    {
        &$NugetPath pack "$i" -IncludeReferencedProjects -Prop Configuration=Release
    }
}