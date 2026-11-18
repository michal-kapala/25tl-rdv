# 25 To Life RDV
Quazal Rendez-Vous matchmaking server implementation for  _25 To Life_ game by Eidos Interactive (PC/PS2 support).

| Service | Protocol | Port |
|---|---|---|
| PS2 authentication | PRUDP/RMC | 30120 (UDP) |
| PC authentication | PRUDP/RMC | 30125 (UDP) |
| RDV (matchmaking) | PRUDP/RMC | 30126 (UDP) |
| Live API | HTTP | 2525 |

Live API exposes info on online players and lobbies.

PCSX2 is supported, the game clients do not support PC-PS2 crossplay in P2P traffic.

## Configuration

### Client
To redirect client's UDP traffic to the server, add the following line to `C:\Windows\System32\drivers\etc\hosts` file:
- localhost setup

```
127.0.0.1 ttllivepc.quazal.net
127.0.0.1 ttlliveps2.quazal.net
```

- LAN setup

```
<LAN internal ip> ttllivepc.quazal.net
<LAN internal ip> ttlliveps2.quazal.net
```

- live environment setup

```
<env domain> ttllivepc.quazal.net
<env domain> ttlliveps2.quazal.net
```

Forwarding the game port is recommended in public hosting scenarios (UDP 4659 for both PC and PS2).

### Backend
Set your public address in `SecureServerAddress` tag of `TTLBackend/App.config` XML configuration before building the project. This address will be used by the authentication server to redirect traffic to the RDV server after successful authentication.

Run the following commands from the root directory to trust `.resx` files:
```ps1
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass .\trust_resx.ps1
.\trust_resx.ps1
```

## Usage
To start the server application:

1) Run `TTLBackend.exe` (via Visual Studio project, `_runme.bat` script or directly)
2) Click `Start` button to start up all online services

## License
Due to interoperability with proprietary abandonware, this software is licensed under MIT with [Commons Clause](https://commonsclause.com/) (permissive with the exception of sale).

## Disclaimers
This project is not maintained by or affiliated with Eidos Interactive, Avalanche Software, Crystal Dynamics or Ritual Entertainment.

This project is a fork of Quazal SDK implementation for [Ghost Recon Online](https://github.com/zeroKilo/GROBackendWV) by Warranty Voider and reuses as much from it as possible.

Big thanks and shoutout to Warranty Voider who has done the prework of research and implementation of the whole Quazal protocol stack and made this project possible.

## Demos
### Development
[![25tl server dev demos](https://img.youtube.com/vi/iJT8YtnyyvE/0.jpg)](https://youtube.com/playlist?list=PLblnScJworYc_OSw2kYKyHIqnttqrdMkO)

### PC gameplay
[![pc gameplay](https://img.youtube.com/vi/LF5YM0TT_gQ/0.jpg)](https://www.youtube.com/watch?v=LF5YM0TT_gQ)

### PS2 gameplay
[![ps2 gameplay](https://img.youtube.com/vi/yeKc303MSWA/0.jpg)](https://www.youtube.com/watch?v=yeKc303MSWA)
