

## Building the native library

```
mkdir -p InteropTestNative/obj && \
cd InteropTestNative/obj/ && \
cmake -GNinja .. -DCMAKE_INSTALL_PREFIX=../bin && \
ninja && \
ninja install
```

## Building the managed test

```
dotnet test \
    --nologo --configuration Release \
    --verbosity "normal" --logger "console;verbosity=detailed" \
    NativeBinding.Tests/NativeBinding.Tests.csproj
```