# Overview
This is a proof-of-concept Quazal Rendez-Vous emulator implementation of the online services for the PC version of the game **_25 To Life_** by Eidos Interactive.

Its aim is to enable P2P online multiplayer mode for _**25 To Life**_ without the use of any LAN emulators.

This project is a fork of [Quazal SDK implementation for Ghost Recon Online by Warranty Voider](https://github.com/zeroKilo/GROBackendWV) and reuses as much from it as possible.

Big thanks and shoutout to Warranty Voider who has done the prework of research and implementation of the whole Quazal protocol stack and made this project possible.

# Configuration
## Client
To redirect client's UDP traffic to the server, add the following line to `C:\Windows\System32\drivers\etc\hosts` file:
- localhost setup

`127.0.0.1 ttllivepc.quazal.net`
- LAN setup

`<LAN internal ip> ttllivepc.quazal.net`
- live environment setup

`<env domain> ttllivepc.quazal.net`

Forwarding port **4659 (UDP)** is required to connect to the backend online (does not affect LAN).

## Backend
Set your public address in `SecureServerAddress` tag of `TTLBackend/App.config` XML configuration before building the project. This address will be used by the authentication server to redirect traffic to the secure server after successful authentication.

Run the following commands from the root directory to trust `.resx` files:
```ps1
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass .\trust_resx.ps1
.\trust_resx.ps1
```

# Usage
To start the server application:

1) Run `TTLBackend.exe` (via Visual Studio project, `_runme.bat` script or directly)
2) Click `Start` button to start up the UDP server.
