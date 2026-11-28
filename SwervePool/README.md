# Swerve Pool

A 3D eight-ball game with real spin physics. Unrelated to the rest of this repository; kept here for safekeeping.

- `swerve-pool.html` is the whole game: page, physics and rendering. It is the single source for both the web version and the Android app.
- `android/` wraps that page in a full-screen WebView and builds an installable APK.

## Build the APK

    cd android && ./build.sh

`build.sh` lists the tools it needs. Bump `android:versionCode` in `AndroidManifest.xml` for each new release.

## Signing key

`android/swervepool.keystore` (alias `swervepool`, passwords `swervepool`) signs the APK. Android only accepts an update signed with the same key, so keep this file. Anyone with this key can sign an app that installs over this one; do not reuse it for anything else.
