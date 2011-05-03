@echo off
mkdir "..\..\Build\Documentation\Api"
subst j: "..\..\Build\Documentation\Api"
"C:\Program Files\NDoc3\bin\NDoc3Console.exe" -project="..\..\VisualStudio\BashworkValidation.ndoc"
msbuild "..\..\VisualStudio\BashworkValidation.shfbproj"
doxygen "..\..\VisualStudio\BashworkValidation.Doxyfile"
