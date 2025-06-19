{
  description = "Flake providing a package for the SimpleStation14 Launcher.";

  inputs.nixpkgs.url = "github:NixOS/nixpkgs/release-25.05";
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
          version = "3.0.0";
          source = pkgs.fetchFromGitHub {
            owner = "Simple-Station";
            repo = "SimpleStationLauncher";
            tag = "v3.0.0";
            hash = "sha256-Qyr1otFKKY7wOCQucDT59dyk/PmqyKTKuwPEAU7mcR4=";
            fetchSubmodules = true;
          };
        };
      });

      formatter = forAllSystems (pkgs: pkgs.nixfmt-rfc-style);
    };
}
