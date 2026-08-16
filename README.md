# Hierarchy Path Tools

Unity Project 창에는 **Copy Path**가 있는데 Hierarchy에는 없어서 만들어 봤습니다.

Unity gives you **Copy Path** in the Project window but nothing like it in the Hierarchy, so I made one.

---

## 한국어

### 기능

**세 가지 경로** — 명칭은 모듈러 아바타를 따랐습니다.

| | |
|---|---|
| **절대 경로** | 아바타 루트 기준. 애니메이션 클립이 쓰는 형태 |
| **상대 경로** | MA Merge Animator 또는 중첩 Animator 기준 |
| **씬 경로** | 씬 루트부터의 전체 경로 |

**상대 경로는 기준이 잡힐 때만 작동합니다.** 기준은 두 단계로 찾습니다.

1. 선택 항목의 상위에 **MA Merge Animator**가 있고 경로 모드가 상대 경로이면 그것을 기준으로 씁니다.
2. 만약 MA가 없으면 **가장 가까운 상위 Animator**를 기준으로 씁니다.


**절대 경로도 마찬가지로 기준이 필요합니다.**

1. 상위에 있는 아바타 디스크립터 또는 **Animator**를 기준으로 사용하며, 둘 다 없으면 절대 경로는 비활성화됩니다.

**실시간 표시** — 창을 띄워두면 Hierarchy에서 클릭하는 대로 세 경로가 한꺼번에 갱신됩니다. 

**중복 이름 경고** — 같은 부모 아래에 이름이 같은 오브젝트가 있으면 Unity 경로로는 구분이 안 돼 애니메이션이 깨집니다. 복사 시점과 창에서 경고합니다.

### 설치

VCC / ALCOM에 추가:

```

```

### 사용

| | |
|---|---|
| Hierarchy 우클릭 → Hierarchy Path | 절대 / 상대 / 씬 경로 복사 |
| Tools → Hierarchy Path | 창 열기 |
| 창 우측 상단 드롭다운 | 언어 (English / 日本語 / 한국어) |
| Edit → Shortcuts → Hierarchy Path | 단축키 지정 |

여러 개를 선택하고 우클릭하면 한 줄에 하나씩 복사됩니다. 창은 활성 항목만 보여줍니다.

**단축키는 기본값 없이 비워져 있습니다.** Edit → Shortcuts → Hierarchy Path에서 원하는 키를 직접 지정하세요. 네 가지를 지정할 수 있습니다.

| | |
|---|---|
| Copy Absolute Path | 절대 경로 복사 |
| Copy Relative Path | 상대 경로 복사 |
| Copy Scene Path | 씬 경로 복사 |
| Open Window | 창 열기 |

### 요구 사항

Unity 2022.3 이상.

VRChat SDK와 모듈러 아바타는 **둘 다 선택 사항**입니다.

- VRChat SDK 있음 → `VRCAvatarDescriptor`를 아바타 루트로 사용
- 없음 → 가장 가까운 `Animator`를 루트로 사용
- 둘 다 없음 → 절대 경로 비활성 (씬 경로는 그대로 동작)
- 모듈러 아바타 없음 → 상대 경로는 중첩 Animator 기준으로만 동작

---

## English

### Features

**Three path forms**, named after Modular Avatar's own terminology:

| | |
|---|---|
| **Absolute Path** | Anchored to the avatar root — the form animation clips use |
| **Relative Path** | Relative to an MA Merge Animator or a nested Animator |
| **Scene Path** | Full path from the scene root |

**The relative path appears only when a base can be found.** The base is resolved in two steps:

1. If an **MA Merge Animator** above the selection has its path mode set to relative, that is the base.
2. If there is no MA, the **nearest Animator above the selection** is used as the base.

**The absolute path needs a base too.**

1. It uses the Avatar Descriptor or Animator above the selection; with neither present, the absolute path is disabled.

**Live display** — keep the window open and all three paths update as you click around the Hierarchy. Each row has its own copy button.

**Duplicate name warning** — When two objects under the same parent share a name, Unity paths cannot tell them apart and animations break silently. You get warned when copying and in the window.

### Install

Add to VCC / ALCOM:

```

```

### Usage

| | |
|---|---|
| Hierarchy right-click → Hierarchy Path | Copy absolute / relative / scene path |
| Tools → Hierarchy Path | Open window |
| Dropdown in the window's toolbar | Language (English / 日本語 / 한국어) |
| Edit → Shortcuts → Hierarchy Path | Assign shortcuts |

Multi-select and right-click to copy one path per line. The window shows the active object only.

**Shortcuts ship unassigned**, so nothing can collide with keys your other tools already use. Pick your own under Edit → Shortcuts → Hierarchy Path; four commands are registered:

| | |
|---|---|
| Copy Absolute Path | Copy the absolute path |
| Copy Relative Path | Copy the relative path |
| Copy Scene Path | Copy the scene path |
| Open Window | Open the window |

### Requirements

Unity 2022.3 or newer.

Both the VRChat SDK and Modular Avatar are **optional**.

- With the VRChat SDK → `VRCAvatarDescriptor` is used as the avatar root
- Without it → the nearest `Animator` is used instead
- With neither → the absolute path is disabled (the scene path still works)
- Without Modular Avatar → the relative path falls back to nested Animators only

---

## License

MIT
