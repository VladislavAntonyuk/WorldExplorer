# WorldExplorer

https://world-explorer.azurewebsites.net/

<a href="https://stand-with-ukraine.pp.ua"><img src="https://img.shields.io/badge/made_in-ukraine-ffd700.svg?labelColor=0057b7"></a>

[![Buy Me A Coffee](https://ik.imagekit.io/VladislavAntonyuk/vladislavantonyuk/misc/bmc-button.png)](https://www.buymeacoffee.com/vlad.antonyuk)

Explore the globe like never before with **World Explorer**. Our AI-powered app gives you in-depth insights about any place worldwide, provides a comprehensive description, and recommends local attractions with your personalized travel guide.

A journey is not just about reaching the destination; it's about the experiences, insights, and memories you create along the way. With the World Explorer app, you can add a personal travel guide to your pocket with a mere tap of a button. Leverage the power of AI to uncover the secrets of any place worldwide. From historic locations to modern sites, World Explorer provides a detailed description of your desired place, shaped by community insights and tailored to your interests. Discover the local cuisine, must-visit spots, cultural attractions, hidden gems, and more with our nearby recommendations feature. Venture into the unexplored and make your journey memorable with **World Explorer**.

## Conferences

### .NET Conf 2024 - Ukraine (English version)
[![YouTube Video Link](https://img.youtube.com/vi/eFUxMxLPWAo/0.jpg)](https://www.youtube.com/watch?v=eFUxMxLPWAo)

### .NET Conf 2024 - Ukraine
[![YouTube Video Link](https://img.youtube.com/vi/eFUxMxLPWAo/0.jpg)](https://www.youtube.com/watch?v=eFUxMxLPWAo)

### .NET Conf 2023 - Ukraine
[![YouTube Video Link](https://img.youtube.com/vi/sHVlg8Y6qlU/0.jpg)](https://www.youtube.com/watch?v=sHVlg8Y6qlU)

## Development

### WebApp

```bash
cd "src/Web/WorldExplorer.AppHost"
dotnet run -c Release
```

### MacCatalyst

Run on Device

```bash
dotnet build -t:Run -c Release -f net9.0-maccatalyst -r maccatalyst-arm64
```

### iOS

Run on Emulator iPhone 16 Pro Max (iOS 18.1)

```bash
dotnet build -t:Run -c Release -f net9.0-ios -p:_DeviceName=:v2:udid=86E951D8-DF6E-4CEF-9595-07D4E2D01367
```

Run on Device

```bash
dotnet build -t:Run -c Release -f net9.0-ios -r ios-arm64 -p:_DeviceName=IDENTIFIER
```

### Android

Generate jks

```bash
keytool -genkey -v -keystore world-explorer.jks -alias world-explorer -keyalg RSA -keysize 2048 -validity 10000
```

Sign aab
```bash
jarsigner -verbose -sigalg SHA256withRSA -digestalg SHA-256 -keystore world-explorer.jks com.vladislavantonyuk.worldexplorer.aab world-explorer  -storepass YOUR_PASSWORD
```


## Graph QL

```bash
dotnet tool install StrawberryShake.Tools -g --prerelease
dotnet graphql init https://localhost:5002/graphql/ -n WorldExplorerTravellersClient -p ./TravellersGraphQL
```
