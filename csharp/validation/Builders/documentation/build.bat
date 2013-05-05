@echo off
mkdir "..\..\Build\Documentation\Api"
subst j: "..\..\Build\Documentation\Api"
"C:\Program Files\NDoc3\bin\NDoc3Console.exe" -project="..\..\VisualStudio\SharedAssemblies.ndoc"
msbuild "..\..\VisualStudio\SharedAssemblies.shfbproj"
doxygen "..\..\VisualStudio\SharedAssemblies.Doxyfile"
