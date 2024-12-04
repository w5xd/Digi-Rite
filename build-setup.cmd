beta-build-instructions.htm
pause "You will be offered 5 files to edit. As you File/Exit each one, another notepad will come up for you to edit."
notepad Digi-XDft\WsjtxBoost.props
notepad Digi-XDft\XDwsjtFt.props
notepad AssemblyVersionInfo.cs
notepad InstallDigiRite\Product.wxs
pause "Last is the 64 bit version of what you just edited"
notepad InstallDigiRiteX64\ProductX64.wxs
pause "We are going to build the various .sln files next"
pushd Digi-XDft
msbuild XDft.sln /t:Build /p:Configuration=Release /p:Platform=x86
msbuild XDft.sln /t:Build /p:Configuration=Release /p:Platform=x64
popd
msbuild DigiRite.sln -Restore /t:Build /p:Configuration=Release /p:Platform=x86
msbuild DigiRite.sln -Restore /t:Build /p:Configuration=Release /p:Platform=x64

