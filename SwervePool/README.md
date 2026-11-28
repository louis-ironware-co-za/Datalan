# Swerve Pool

A 3D cue-sports game with real spin physics. Unrelated to the rest of this repository; kept here for safekeeping.

- One game: UK 8-Ball, with reds, yellows and the black. Fouls give the opponent two visits.
- Play the computer at three levels, or two players on one device.
- `swerve-pool.html` is the whole game: page, physics, rules, rendering and sound. It is the single source for both the web version and the Android app. All art and sound are generated in code; there are no asset files.
- `android/` wraps that page in a full-screen WebView and builds an installable APK.

## Build the APK

    cd android && ./build.sh

`build.sh` lists the tools it needs. Bump `android:versionCode` in `AndroidManifest.xml` for each new release.

## Signing key

`android/swervepool.keystore` (alias `swervepool`, passwords `swervepool`) signs the APK. Android only accepts an update signed with the same key, so keep this file. Anyone with this key can sign an app that installs over this one; do not reuse it for anything else.
