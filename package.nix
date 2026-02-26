{
  buildDotnetModule,
  dotnetCorePackages,
}:

buildDotnetModule {
  pname = "VOD-Manager";
  version = "0.1.0";

  src = ./.;

  projectFile = "VOD-Manager/VOD-Manager.csproj";

  dotnet-sdk = dotnetCorePackages.sdk_10_0;
  dotnet-runtime = dotnetCorePackages.runtime_10_0;

  nugetDeps = ./deps.json;

  executables = [ "VOD-Manager" ];

  meta = {
    description = "Automated YouTube VOD uploader and scheduler";
    homepage = "https://example.invalid";
    license = "MIT";
  };
}
