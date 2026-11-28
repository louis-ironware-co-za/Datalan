#!/bin/sh
# Builds SwervePool.apk without Gradle or Android Studio.
# Needs (Ubuntu): apt-get install aapt apksigner zipalign android-sdk-platform-23 dalvik-exchange
# plus a JDK, Node.js and Playwright (prep.js renders the launcher icon).
set -e
cd "$(dirname "$0")"
JAR=/usr/lib/android-sdk/platforms/android-23/android.jar
rm -rf obj && mkdir -p obj
node prep.js
aapt2 compile --dir res -o obj/res.zip
aapt2 link -o obj/base.apk -I "$JAR" --manifest AndroidManifest.xml -A assets \
  --min-sdk-version 24 --target-sdk-version 34 obj/res.zip
javac -source 8 -target 8 -cp "$JAR" -d obj/classes src/com/swervepool/app/MainActivity.java
dalvik-exchange --dex --min-sdk-version=24 --output=obj/classes.dex obj/classes
(cd obj && zip -j base.apk classes.dex)
zipalign -f -p 4 obj/base.apk obj/aligned.apk
apksigner sign --ks swervepool.keystore --ks-pass pass:swervepool --ks-key-alias swervepool \
  --out SwervePool.apk obj/aligned.apk
apksigner verify SwervePool.apk
echo "Built $(pwd)/SwervePool.apk"
