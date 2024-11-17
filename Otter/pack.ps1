# Ensure the project is built
dotnet build -c Release

# Pack the project using the nuspec file
nuget pack Otter.nuspec -OutputDirectory ./nupkgs
