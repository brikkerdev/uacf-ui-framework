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
    "com.uacf.ui-framework": "file:com.uacf.ui-framework",
    "com.uacf.editor": "https://github.com/brikkerdev/UACF.git?path=Packages/com.uacf.editor"
  }
}
```

## Quick Start

1. Create a Theme: Assets > Create > UACF UI > Theme
2. Create a Color Palette: Assets > Create > UACF UI > Color Palette
3. Assign the palette to your theme
4. Create UI: GameObject > UACF UI > [component type]

## Features

- **Design Tokens**: ColorPalette, TypographySet, SpacingScale, ShapeSet, ElevationSet, IconSet
- **Style System**: UIStyle with state inheritance (normal, hovered, pressed, disabled)
- **Theming**: ThemeManager with runtime theme switching
- **Components**: UIText, UIImage, UIButton, UIPanel, UIScrollView, and more
- **Screens**: UIScreen, UINavigator, TransitionPreset
- **UACF API**: Full control via HTTP API (curl) when UACF is installed

## UACF API Endpoints

When UACF is installed, the following endpoints are available:

**Theme:** `GET /api/ui/themes`, `POST /api/ui/theme/create`, `PUT /api/ui/theme/apply`, `GET /api/ui/theme/get`

**Tokens:** `PUT /api/ui/tokens/colors`, `POST /api/ui/tokens/colors/add-custom`, `PUT /api/ui/tokens/typography`, `PUT /api/ui/tokens/spacing`

**Screen:** `POST /api/ui/screen/create`, `GET /api/ui/screen/hierarchy`

**Layout:** `POST /api/ui/layout/create` — create vertical/horizontal/grid layout

**Elements:** `POST /api/ui/element/add`, `PUT /api/ui/element/modify`, `DELETE /api/ui/element/remove`, `POST /api/ui/element/reorder`

**Batch:** `POST /api/ui/batch` — batch operations with `parent: {"ref":"op_id"}` for chaining

**Other:** `GET /api/ui/components/list` — list available components

All operations are available via HTTP API (curl, Postman, AI agents) without one-off scripts.

## Documentation

See [Docs/UI_Framework.md](../../Docs/UI_Framework.md) for full specification.
