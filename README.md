# QuickEat

Press **V** to instantly eat the food you’re holding. QuickEat auto-confirms the eat dialog, blocks non-edible/big-craftables, and smooths out edge cases so you can chain-eat without opening menus.

---

## ✨ Features

* **One key eat**: Press **V** (configurable) to consume the currently held edible.
* **Auto-confirm**: Automatically selects *Yes* on the eat dialog when needed.
* **Safety checks**: Blocks big craftables and non-edible items.
* **Smooth chaining**: Cleans up internal waiting state after animations so the hotkey keeps working.
* **Multiplayer-friendly**: Only players with the mod get the hotkey behavior; others are unaffected.

## 📦 Requirements

* **Stardew Valley 1.6+**
* **SMAPI 4.0+**

## 🔧 Installation

1. Install **SMAPI**.
2. Download the **QuickEat** release `.zip`.
3. Extract so you have a folder:

   ```
   Stardew Valley/Mods/QuickEat/
     ├─ manifest.json
     └─ QuickEat.dll
   ```
4. Launch the game via SMAPI.

## 🎮 Usage

* Hold any **edible** food in the toolbar.
* Press **V** to eat it immediately.
* If the game would normally show a *Yes/No* prompt, QuickEat will auto-confirm *Yes*.

## ⚙️ Configuration (`config.json`)

Created on first run. Keys:

| Key                  | Type   | Default | Description                                          |
| -------------------- | ------ | ------: | ---------------------------------------------------- |
| `EatKey`             | string |     `V` | Hotkey to trigger eating the held food.              |
| `IgnoreWhenMenuOpen` | bool   |  `true` | Ignore the hotkey when menus are open.               |
| `RequirePlayerFree`  | bool   |  `true` | Ignore when the player cannot move (cutscene/event). |

> Tip: Edit `Stardew Valley/Mods/QuickEat/config.json` while the game is closed (or use Generic Mod Config Menu if you have it).

## 🔁 Compatibility & Multiplayer

* Works on **Stardew Valley 1.6+** and **SMAPI 4.0+**.
* In **multiplayer**, only players who have the mod installed can use the hotkey; others play normally.
* Mod avoids consuming non-edibles and big craftables.

## 🩹 Troubleshooting

* **Does nothing**: Make sure you’re holding an edible food (not a big craftable). Check SMAPI console for errors.
* **Hotkey works once, then stops**: This usually means an eat dialog stayed open—QuickEat auto-confirms and also cleans up after animation. If you still see it, please share your SMAPI log.

## 📄 License & Credits

* Author: **hankan1918**
* Thanks: SMAPI team and Stardew Valley modding community.
* License: Code: MIT License © hankan1918

## 📝 Changelog

* **1.0.0** – Initial release.

---

# (KOR) QuickEat

손에 든 **음식**을 **V 키**로 즉시 섭취합니다. 먹기 확인 창을 자동으로 *예* 처리하고, 비식용/장식(빅크래프터블)을 안전하게 걸러서 **연속 섭취**가 끊기지 않도록 보정합니다.

## ✨ 주요 기능

* **한 번에 먹기**: **V**(변경 가능)로 현재 들고 있는 음식을 즉시 섭취
* **자동 확인**: 필요 시 먹기 대화상자 *예* 자동 선택
* **안전 장치**: 비식용(-300)과 **bigCraftable**(장식/제작물) 차단
* **연속 섭취 보정**: 애니메이션 종료 후 내부 대기 상태를 정리해 단축키가 계속 동작
* **멀티 호환**: 모드를 설치한 플레이어만 단축키가 적용(다른 유저는 영향 없음)

## 📦 요구 사항

* **Stardew Valley 1.6+**
* **SMAPI 4.0+**

## 🔧 설치 방법

1. **SMAPI**를 설치합니다.
2. **QuickEat** 릴리스 `.zip`을 내려받습니다.
3. 압축을 풀어 아래 구조가 되도록 `Mods` 폴더에 넣습니다.

   ```
   Stardew Valley/Mods/QuickEat/
     ├─ manifest.json
     └─ QuickEat.dll
   ```
4. SMAPI로 게임을 실행합니다.

## 🎮 사용법

* 툴바에서 **음식**을 손에 듭니다.
* **V** 키를 누르면 즉시 섭취합니다.
* 먹기 확인 창이 필요한 경우 자동으로 *예*가 선택됩니다.

## ⚙️ 설정 (`config.json`)

처음 실행 시 생성됩니다.

| 키                    | 형식  |    기본값 | 설명                         |
| -------------------- | --- | -----: | -------------------------- |
| `EatKey`             | 문자열 |    `V` | 섭취 단축키                     |
| `IgnoreWhenMenuOpen` | 불리언 | `true` | 메뉴가 열려 있으면 무시              |
| `RequirePlayerFree`  | 불리언 | `true` | 플레이어가 움직일 수 없으면(컷신/이벤트) 무시 |

> 팁: `Mods/QuickEat/config.json`을 편집하거나, GMCM(설치 시)으로 바꿀 수 있습니다.

## 🔁 호환성 / 멀티플레이

* \*\*SDV 1.6+ / SMAPI 4.0+\*\*에서 동작합니다.
* **멀티**에서는 모드 설치자만 단축키 사용 가능하며, 미설치자는 영향이 없습니다.
* 비식용/장식 아이템은 섭취 시도에서 제외합니다.

## 🩹 문제 해결

* **아무 반응 없음**: 손에 든 것이 ‘음식’인지 확인(장식/제작물은 제외). SMAPI 콘솔 오류 확인.
* **한 번만 되고 멈춤**: 먹기 확인창이 남았을 수 있습니다. QuickEat이 자동으로 처리하지만 재현 시 로그를 공유해 주세요.

## 📄 라이선스 / 크레딧

* 제작: **hankan1918**
* 감사: SMAPI 팀, 모딩 커뮤니티
* 라이선스: 코드: MIT License © hankan1918

## 📝 변경 사항

* **1.0.0** – 첫 공개 버전.
