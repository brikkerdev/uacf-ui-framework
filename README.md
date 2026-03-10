# UACF UI Framework

Token-driven UI framework for Unity 6.3 with design tokens, component library, screen management, and UACF API integration for AI automation.

## Requirements

- Unity 6.3+
- Unity UI (com.unity.ugui)
- TextMeshPro (com.unity.textmeshpro)
- UACF (com.uacf.editor) — for Editor API integration

## Installation

Add to your project's `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.uacf.ui-framework": "https://github.com/brikkerdev/uacf-ui-framework.git?path=Packages/com.uacf.ui-framework",
    "com.uacf.editor": "https://github.com/brikkerdev/UACF.git?path=Packages/com.uacf.editor"
  }
}
```

Or clone this repository and add the package via file reference:

```json
{
  "dependencies": {
    "com.uacf.ui-framework": "file:../uacf-ui-framework/Packages/com.uacf.ui-framework"
  }
}
```

## Quick Start

1. Create a Theme: Assets > Create > UACF UI > Theme
2. Create a Color Palette: Assets > Create > UACF UI > Color Palette
3. Assign the palette to your theme
4. Create UI: GameObject > UACF UI > [component type]

## Features

- **Design Tokens** — ColorPalette, TypographySet, SpacingScale, ShapeSet, ElevationSet, IconSet
- **Style System** — UIStyle with state inheritance (normal, hovered, pressed, disabled)
- **Theming** — ThemeManager with runtime theme switching
- **Components** — UIText, UIImage, UIButton, UIPanel, UIScrollView, UIModal, and more
- **Screens** — UIScreen, UINavigator, TransitionPreset
- **UACF API** — Full control via HTTP API when UACF is installed

## UACF API Endpoints

When UACF is installed, all UI operations are available via HTTP API (curl, Postman, AI agents) without one-off scripts:

- **Theme:** GET/POST/PUT themes, create with custom colors
- **Tokens:** Update colors, typography, spacing in ColorPalette/TypographySet/SpacingScale
- **Screen:** Create screen (open scene, Canvas, root UIPanel)
- **Layout:** Create vertical/horizontal/grid layout
- **Elements:** Add, modify (name, properties, RectTransform), remove, reorder
- **Batch:** Chain operations with `parent: {"ref":"op_id"}`

## Documentation

See [Docs/UI_Framework.md](Docs/UI_Framework.md) for full specification.
