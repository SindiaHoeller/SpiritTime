# SpiritTime
Dieses Projekt wird aktuell nicht mehr gewartet, da es als Showcase gedacht ist.

Es dient zur Veranschaulichung von verschiedenen Technologien und Pattern. Es wurde dabei Wert auf die Clean-Code Regeln gelegt, um möglichst gute Lesbarkeit und Wiederverwendbarkeit von Code und Komponenten zu gewährleisten.

## Verwendete Technologien
* .Net Core
  * Blazor ServerSide Frontend (SpiritTime.Frontend)
  * ASP.Net Core Web API  (SpiritTime.Backend)
    * Entity Framework (Code First)
    * mit Swagger
* Electron
* SASS

## Verwendete Architektur- & Designpattern
* N-Tier Layer
* UnitOfWork Pattern

## Benötigte Settings Files
Diese sind nicht im Github Projekt selbt enthalten, werden jedoch benötigt, wenn das Projekt ausgeführt werden soll.

Die entsprechenden Token / Connection-Strings usw. müssen dann natürlich noch ausgefüllt werden.

### SpiritTime.Frontend
appsettings.json
``` json
{
  "Settings": {
    "BackendBaseAddress": "http://localhost:5002/",
    "Version": "0.0.1"
  },
  "Shortcuts": {
    "NewTask": "CmdOrCtrl+I",
    "CurrentTask": "CmdOrCtrl+O",
    "StopCurrentTask": "CmdOrCtrl+H"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Trace",
      "System": "Information",
      "Microsoft": "Information"
    }
  },
  "Proxy": {
    "PacScript": "",
    "ProxyRules": "",
    "ProxyBypassRules": ""
  },
  "ProxyAuth": {
    "ProxyUrl": "",
    "ProxyUsername": "",
    "ProxyPassword": "",
    "ProxyDomain": "",
    "AuthType": null,
    "Enabled": false,
    "DefaultNetworkCred": false
  },
  "AllowedHosts": "*",
  "DetailedErrors": true
}
```
electron.manifest.json
```json
{
  "executable": "SpiritTime.Frontend",
  "splashscreen": {
    "imageFile": "Assets/icon_120x120.png"
  },
  "name": "Spirittime",
  "author": "Sindia Höller",
  "singleInstance": false,
  "build": {
    "appId": "com.SpiritTime.Frontend.app",
    "productName": "SpiritTime",
    "buildVersion": "0.0.1",
    "compression": "maximum",
    "directories": {
      "output": "../../../bin/Desktop"
    },
    "extraResources": [
      {
        "from": "./bin",
        "to": "bin",
        "filter": ["**/*"]
      }
    ],
    "files": [
      {
        "from": "./ElectronHostHook/node_modules",
        "to": "ElectronHostHook/node_modules",
        "filter": ["**/*"]
      },
      "**/*"
    ],
    "nsis": {
      "oneClick": false,
      "perMachine": true,
      "allowElevation": true,
      "allowToChangeInstallationDirectory": true,
      "include": "bin/ElectronResources/nsisExtra.nsh"
    },
    "win": {
      "target": "NSIS",
      "icon": "bin/Assets/icon.ico",
      "publish": [{
        "provider": "github",
        "owner": "",
        "repo": "",
        "token": ""
      }]
    },
    "linux": {
      "icon": "bin/Assets/icon_120x120.png"
    }
  }
}
```
### SpiritTime.Backend
appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "",
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "Settings": {
    "Color": "Dark"
  },
  "Jwt": {
    "SecurityKey": "",
    "ValidIssuer": "",
    "ValidAudience": ""
  },
  "SwaggerOptions": {
    "JsonRoute": "swagger/{documentName}/swagger.json",
    "Description": "SpiritTime API",
    "UIEndpoint":  "v1/swagger.json",
    "LandingPageRedirectUrl": "swagger/index.html"
  }
}
```