# WorldExplorer

https://world-explorer.azurewebsites.net/

## Conference
[![YouTube Video Link](https://img.youtube.com/vi/sHVlg8Y6qlU/0.jpg)](https://www.youtube.com/watch?v=sHVlg8Y6qlU)

## Description

Explore the globe like never before with World Explorer. Our AI-powered app gives you in-depth insights about any place worldwide, provides a comprehensive description, and recommends local attractions with your personalized travel guide.

A journey is not just about reaching the destination; it's about the experiences, insights, and memories you create along the way. With the World Explorer app, you can add a personal travel guide to your pocket with a mere tap of a button. Leverage the power of AI to uncover the secrets of any place worldwide. From historic locations to modern sites, World Explorer provides a detailed description of your desired place, shaped by community insights and tailored to your interests. Discover the local cuisine, must-visit spots, cultural attractions, hidden gems, and more with our nearby recommendations feature. Venture into the unexplored and make your journey memorable with World Explorer.

## Development

### MacCatalyst

Run on Device

```bash
dotnet build -t:Run -c Release -f net8.0-maccatalyst -r maccatalyst-arm64
```

### iOS

Run on Emulator iPhone 15 Pro Max

```bash
dotnet build -t:Run -c Release -f net8.0-ios -p:_DeviceName=:v2:udid=7D850AFF-242B-41BC-A9A7-A0818D4CEF30
```

Run on Device

```bash
dotnet build -t:Run -c Release -f net8.0-ios -r ios-arm64 -p:_DeviceName=IDENTIFIER
```

### Android

Generate jks

```bash
keytool -genkey -v -keystore world-explorer.jks -alias world-explorer -keyalg RSA -keysize 2048 -validity 10000
```

Sign aab
```bash
jarsigner -verbose -sigalg SHA1withRSA -digestalg SHA1 -keystore world-explorer.jks com.vladislavantonyuk.worldexplorer.aab world-explorer  -storepass YOUR_PASSWORD
```
