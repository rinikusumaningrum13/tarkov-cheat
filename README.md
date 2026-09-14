# tarkov-cheat

> Escape from Tarkov · cheat · esp · aimbot · wallhack · triggerbot

[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dot.net)
[![Cheat](https://img.shields.io/badge/type-cheat-red)]()
[![Game](https://img.shields.io/badge/game-Escape%20from%20Tarkov-orange)]()
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

**tarkov-cheat** is an external Escape from Tarkov cheat: ESP, aim assist, triggerbot, recoil, bhop, radar and misc.

tested on latest patch. for research and educational purposes only.

## features

- **ESP** — box, health bar, distance, name tags, skeleton, snaplines
- **Aim Assist** — configurable FOV circle, smooth factor, bone select, recoil compensation
- **TriggerBot** — reaction delay with jitter, burst mode, team check
- **Bunny Hop** — auto-jump on ground flag detect
- **Radar** — force enemy spotted on minimap
- **No Flash** — override flash duration to zero
- **Config** — JSON profiles under %APPDATA%, import/export, hotkeys

## build

requires .NET 10 SDK.

```
dotnet build
```

run:

```
.\build\bin\tarkov-cheat.Loader\net10.0\tarkovcheat.exe
```

Escape from Tarkov must be running before you start the loader.

## config

profiles auto-save to `%APPDATA%\tarkov-cheat\profiles\default.json`.

```json
{
  "aim": {
    "enabled": true,
    "fov": 5.0,
    "smooth": 3.5,
    "bone": "Head",
    "rcs": true
  },
  "esp": {
    "enabled": true,
    "box": true,
    "health": true,
    "skeleton": false,
    "distance": true
  },
  "trigger": {
    "enabled": false,
    "delayMs": 50,
    "jitter": 15,
    "burstCount": 1
  },
  "misc": {
    "bhop": false,
    "noFlash": false,
    "radar": false
  }
}
```

## keybinds

| key | action |
|---|---|
| INSERT | toggle menu |
| F1 | aim assist |
| F2 | esp |
| F3 | triggerbot |
| F4 | bhop |
| MOUSE5 | aim key (hold) |
| HOME | reload config |
| END | panic — clean exit |

## anti-cheat

targets **BattlEye**. no bypass included.
detection is expected without additional evasion layers.

## disclaimer

educational / research project for game hacking concepts.
not affiliated with the developers of Escape from Tarkov.
don't use in online matchmaking — you **will** get banned.


---

## Topics

![tarkov](https://img.shields.io/badge/tarkov-111827?style=flat-square) ![tarkov-cheat](https://img.shields.io/badge/tarkov%20cheat-111827?style=flat-square) ![escape-from-tarkov](https://img.shields.io/badge/escape%20from%20tarkov-111827?style=flat-square) ![eft](https://img.shields.io/badge/eft-111827?style=flat-square) ![cheat](https://img.shields.io/badge/cheat-111827?style=flat-square) ![hack](https://img.shields.io/badge/hack-111827?style=flat-square) ![esp](https://img.shields.io/badge/esp-111827?style=flat-square) ![radar](https://img.shields.io/badge/radar-111827?style=flat-square)

`tarkov` `tarkov-cheat` `escape-from-tarkov` `eft` `cheat` `hack` `esp` `radar` `loot` `game-hacking` `free` `csharp`

Search: tarkov-cheat · tarkov · cheat · esp · loot · radar · Escape from Tarkov cheat — loot ESP, extraction markers, player radar, item filter. EFT hack.

---

<sub>Escape from Tarkov cheat — loot ESP, extraction markers, player radar, item filter. EFT hack.</sub>
