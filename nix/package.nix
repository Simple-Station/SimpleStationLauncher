{
  lib,
  buildDotnetModule,
  dotnetCorePackages,
  fetchFromGitHub,
  wrapGAppsHook,
  iconConvTools,
  copyDesktopItems,
  makeDesktopItem,
  libX11,
  libICE,
  libSM,
  libXi,
  libXcursor,
  libXext,
  libXrandr,
  fontconfig,
  glew,
  SDL2,
  glfw,
  glibc,
  libGL,
  freetype,
  openal,
  fluidsynth,
  gtk3,
  pango,
  atk,
  cairo,
  zlib,
  glib,
  gdk-pixbuf,
  soundfont-fluid,
  # Path to set ROBUST_SOUNDFONT_OVERRIDE to, essentially the default soundfont used.
  soundfont-path ? "${soundfont-fluid}/share/soundfonts/FluidR3_GM2-2.sf2",

  version ? "development",
  source ? ../.,
}:
buildDotnetModule rec {
  pname = "simple-station-launcher";

  # Workaround to prevent buildDotnetModule from overriding assembly versions.
  name = "${pname}-${version}";

  src = source;

  buildType = "Release";
  selfContainedBuild = false;

  projectFile = [
    "SS14.Loader/SS14.Loader.csproj"
    "SS14.Launcher/SS14.Launcher.csproj"
  ];

  nugetDeps = ./deps.nix;

  passthru = {
    inherit version;
  };

  dotnet-sdk = dotnetCorePackages.sdk_8_0;
  dotnet-runtime = dotnetCorePackages.runtime_8_0;

  dotnetFlags = [
    "-p:FullRelease=true"
    "-p:RobustILLink=true"
    "-nologo"
  ];

  nativeBuildInputs = [
    wrapGAppsHook
    iconConvTools
    copyDesktopItems
  ];

  runtimeDeps = [
    # Required by the game.
    glfw
    SDL2
    glibc
    libGL
    openal
    freetype
    fluidsynth

    # Needed for file dialogs.
    gtk3
    pango
    cairo
    atk
    zlib
    glib
    gdk-pixbuf

    # Avalonia UI dependencies.
    libX11
    libICE
    libSM
    libXi
    libXcursor
    libXext
    libXrandr
    fontconfig
    glew

    # TODO: Figure out dependencies for CEF support.
  ];

  makeWrapperArgs = [ ''--set ROBUST_SOUNDFONT_OVERRIDE "${soundfont-path}"'' ];

  executables = [ "SS14.Launcher" ];

  desktopItems = [
    (makeDesktopItem {
      name = pname;
      exec = meta.mainProgram;
      icon = pname;
      desktopName = "SimpleStation14 Launcher";
      comment = meta.description;
      categories = [ "Game" ];
      startupWMClass = meta.mainProgram;
    })
  ];

  postInstall = ''
    mkdir -p $out/lib/${pname}/loader
    cp -r SS14.Loader/bin/${buildType}/*/*/* $out/lib/${pname}/loader/

    icoFileToHiColorTheme SS14.Launcher/Assets/icon.ico ${pname} $out
  '';

  dontWrapGApps = true;

  preFixup = ''
    makeWrapperArgs+=("''${gappsWrapperArgs[@]}")
  '';

  meta = with lib; {
    description = "Launcher for Simple Station 14, a 2D RPG about disasters in space.";
    homepage = "https://simplestation.org";
    license = licenses.mit;
    maintainers = [ ];
    platforms = [
      "x86_64-linux"
      "aarch64-linux"
    ];
    mainProgram = "SS14.Launcher";
  };
}
