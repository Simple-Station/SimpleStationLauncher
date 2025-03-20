{
  description = "Flake providing a package for the SimpleStation14 Launcher.";

  inputs.nixpkgs.url = "github:NixOS/nixpkgs/release-24.11";
  outputs =
    { self, nixpkgs, ... }:
    let
      forAllSystems =
        function:
        nixpkgs.lib.genAttrs [ "x86_64-linux" "aarch64-linux" ] (
          system: function (import nixpkgs { inherit system; })
        );
    in
    rec {
      packages = forAllSystems (pkgs: {
        default = packages.${pkgs.system}.simple-station-launcher-development;
        # Build via nix build -L 'git+file://PATH?submodules=1'
        simple-station-launcher-development = pkgs.callPackage ./nix/package.nix { };
        simple-station-launcher = pkgs.callPackage ./nix/package.nix {
          version = "1.3.1";
          source = pkgs.fetchFromGitHub {
            owner = "Simple-Station";
            repo = "SimpleStationLauncher";
            tag = "v1.3.1";
            hash = "sha256-ekSq0xjgOq+OCrYpzLXihxPT3RpRil/o1Eia9JCp5HQ=";
            fetchSubmodules = true;
          };
        };
      });

      formatter = forAllSystems (pkgs: pkgs.nixfmt-rfc-style);
    };
}
