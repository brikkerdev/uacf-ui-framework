# UACF UI Framework — Документация

Token-driven UI фреймворк для Unity 6.3 с системой дизайн-токенов, библиотекой компонентов, управлением экранами и интеграцией с UACF для автоматизации через AI-агентов.

---

## Содержание

1. [Начало работы](#1-начало-работы)
2. [Основные концепции](#2-основные-концепции)
3. [Design Tokens](#3-design-tokens)
4. [Система стилей](#4-система-стилей)
5. [Темизация](#5-тематизация)
6. [Компоненты](#6-компоненты)
7. [Экраны и навигация](#7-экраны-и-навигация)
8. [Анимация](#8-анимация)
9. [UACF API](#9-uacf-api)
10. [Примеры](#10-примеры)

---

## 1. Начало работы

### Требования

- Unity 6.3+
- Unity UI (com.unity.ugui)
- TextMeshPro (com.unity.textmeshpro)
- UACF (com.uacf.editor) — опционально, для API-автоматизации

### Установка

Добавьте в `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.uacf.ui-framework": "https://github.com/brikkerdev/uacf-ui-framework.git?path=Packages/com.uacf.ui-framework",
    "com.uacf.editor": "https://github.com/brikkerdev/UACF.git?path=Packages/com.uacf.editor"
  }
}
```

### Быстрый старт

1. **Создайте тему** — Assets > Create > UACF UI > Theme
2. **Создайте палитру цветов** — Assets > Create > UACF UI > Color Palette
3. **Назначьте палитру теме** — перетащите палитру в поле Color Palette темы
4. **Создайте UI** — GameObject > UACF UI > [тип компонента]

### Принципы фреймворка

- **Token-driven** — цвета, шрифты, отступы хранятся в ScriptableObjects, не хардкодятся в компонентах
- **Prefab-based** — каждый компонент — готовый префаб с привязкой к теме
- **Composable** — экраны собираются из компонентов как конструктор
- **Theme-switchable** — смена темы в runtime обновляет весь UI
- **Automation-first** — при наличии UACF всё управляется через HTTP API

---

## 2. Основные концепции

### Архитектура

```
Theme (ScriptableObject)
├── ColorPalette      — цвета
├── TypographySet     — шрифты и размеры текста
├── SpacingScale      — отступы
├── ShapeSet          — скругления, формы
├── ElevationSet      — тени
└── IconSet           — иконки

UIStyle (ScriptableObject)
├── normal, hovered, pressed, disabled
└── наследование от parent-стиля

UIComponentBase → StyleBinding → Theme
     ↓
ResolvedStyle (конкретные значения из токенов)
```

### Поток данных

1. **Theme** содержит токены (цвета, шрифты, отступы)
2. **UIStyle** ссылается на токены через ключи (`"primary"`, `"body1"`, `"md"`)
3. **StyleBinding** на компоненте связывает его со стилем
4. **StyleResolver** разрешает токены в конкретные значения через активную тему
5. **ThemeManager** — синглтон, хранит активную тему и уведомляет компоненты при смене

---

## 3. Design Tokens

Токены — централизованные визуальные значения. Изменение токена обновляет все компоненты, которые его используют.

### ColorPalette

Хранит цвета по семантическим ключам.

**Семантические цвета:**
- `primary`, `primaryVariant` — основной цвет бренда
- `secondary`, `secondaryVariant` — вторичный акцент
- `background`, `surface`, `surfaceVariant` — фоны
- `error`, `warning`, `success`, `info` — статусы
- `onPrimary`, `onSecondary`, `onBackground`, `onSurface` — текст поверх цветов
- `divider`, `disabled`, `overlay`, `shadow` — утилитарные

**Кастомные цвета** — добавляйте через `customColors` (список NamedColor).

**Создание:** Assets > Create > UACF UI > Color Palette

```csharp
// Получение цвета из палитры
Color c = colorPalette.GetColor("primary");
colorPalette.TryGetColor("customGold", out Color gold);
```

### TypographySet

Коллекция пресетов текста (шрифт, размер, стиль).

**Стандартные пресеты:** `h1`, `h2`, `h3`, `h4`, `subtitle1`, `subtitle2`, `body1`, `body2`, `caption`, `overline`, `button`

**Создание:** Assets > Create > UACF UI > Typography Set

```csharp
// Применение пресета к TextMeshPro
typographySet.ApplyTo(tmpText, "body1");
TypographyPreset preset = typographySet.GetPreset("h1");
```

### SpacingScale

Шкала отступов в пикселях.

**Стандартные значения:** `none` (0), `xxs` (2), `xs` (4), `sm` (8), `md` (16), `lg` (24), `xl` (32), `xxl` (48), `xxxl` (64)

**Создание:** Assets > Create > UACF UI > Spacing Scale

```csharp
float spacing = spacingScale.GetSpacing("md");
RectOffset padding = spacingScale.GetPadding("md"); // все стороны
RectOffset custom = spacingScale.GetPadding("sm", "lg"); // horizontal, vertical
```

### ShapeSet

Пресеты форм (скругления, обводки).

**Стандартные:** `none`, `small` (4px), `medium` (8px), `large` (16px), `extraLarge` (24px), `circle`

**Создание:** Assets > Create > UACF UI > Shape Set

### ElevationSet

Пресеты теней (смещение, размытие, прозрачность).

**Стандартные:** `none`, `low`, `mid`, `high`

**Создание:** Assets > Create > UACF UI > Elevation Set

### Theme

Объединяет все токены в одну тему.

**Создание:** Assets > Create > UACF UI > Theme

Назначьте палитру, типографику, отступы, формы и elevation. Theme предоставляет методы `GetColor()`, `GetTypography()`, `GetSpacing()`, `GetShape()`, `GetElevation()`.

---

## 4. Система стилей

### UIStyleState

Описывает визуальное состояние компонента (normal, hovered, pressed, disabled). Каждое свойство — ссылка на токен или прямое значение:

- `backgroundColorToken`, `textColorToken`, `typographyToken`
- `shapeToken`, `elevationToken`, `paddingToken`
- `borderColorToken`, `borderWidth`
- `opacity`, `preferredSize`, `scale`

### UIStyle

ScriptableObject с состояниями и наследованием.

- `normal`, `hovered`, `pressed`, `disabled`, `focused`, `selected`
- `parent` — наследование: не заданные свойства берутся из parent
- `transitionDuration`, `transitionCurve` — анимация перехода между состояниями

**Создание:** Assets > Create > UACF UI > Style

### StyleSheet

Коллекция стилей. Используется в Theme как `defaultStyles` для компонентов.

**Создание:** Assets > Create > UACF UI > Style Sheet

### StyleBinding

Компонент на каждом UI-элементе. Связывает `UIComponentBase` со стилем.

- `style` — ссылка на UIStyle (или null)
- `styleKey` — ключ для поиска в Theme.defaultStyles
- `useThemeDefault` — если style = null, использовать default из темы

При смене темы или состояния StyleBinding применяет соответствующий ResolvedStyle к компоненту.

### StyleResolver

Статический класс для разрешения токенов в конкретные значения.

```csharp
ResolvedStyle resolved = StyleResolver.Resolve(style, state, theme);
// resolved.backgroundColor, resolved.textColor, resolved.padding, и т.д.
```

---

## 5. Темизация

### ThemeManager

Синглтон, управляет активной темой.

```csharp
ThemeManager.ActiveTheme;           // текущая тема
ThemeManager.SetTheme(theme);       // сменить тему
ThemeManager.SetTheme("theme-id");  // по ID
ThemeManager.GetColor("primary");   // цвет из активной темы
ThemeManager.OnThemeChanged += OnThemeChanged;
```

При смене темы все зарегистрированные `IThemeable` получают `OnThemeChanged()`.

### ThemeApplier

Компонент на корневом Canvas или UIScreen. При смене темы применяет её ко всем дочерним IThemeable.

- `overrideTheme` — если задана, используется вместо глобальной темы (для модалок, sidebar)
- `applyToChildren` — применять к дочерним элементам

### ThemeListener

Лёгкий компонент для обычных Unity UI элементов (Image, TMP_Text) без UIComponentBase.

- `targetImage` + `imageColorToken`
- `targetText` + `textColorToken`

При смене темы обновляет цвет из ThemeManager.

### TokenReference

Сериализуемая ссылка на токен в теме. Используется в компонентах вместо хардкода.

```csharp
TokenReference<Color> colorRef;  // key = "primary"
Color c = colorRef.Resolve(theme);
```

---

## 6. Компоненты

Все компоненты наследуют `UIComponentBase` и поддерживают `StyleBinding`. Интерактивные — от `UIInteractableBase` (hover, press, disabled).

### Создание через меню

**GameObject > UACF UI >** [категория] > [компонент]

### Display

| Компонент | Описание |
|-----------|----------|
| **UIText** | Текст с поддержкой типографики и цветовых токенов. `typographyToken`, `colorToken`, `SetText()` |
| **UIImage** | Изображение с `colorToken`, `SetSprite()`, `SetColor()` |
| **UIIcon** | Иконка из IconSet. `iconKey`, `colorToken`, `SetIcon()`, `SetSize()` |
| **UIDivider** | Горизонтальный/вертикальный разделитель. `colorToken`, `thickness` |
| **UIBadge** | Бейдж с числом. `SetValue()`, `maxDisplay`, `showZero` |

### Input

| Компонент | Описание |
|-----------|----------|
| **UIButton** | Кнопка. Варианты: Filled, Outlined, Text, Tonal. `SetLabel()`, `SetIcon()`, `onClick` |
| **UIIconButton** | Кнопка-иконка. `SetIcon()`, форма Circle/Square/Rounded |
| **UIToggle** | Переключатель. `SetOn()`, `onValueChanged` |
| **UISlider** | Ползунок. `SetValue()`, `SetRange()`, `onValueChanged` |
| **UIDropdown** | Выпадающий список. `SetOptions()`, `SetSelected()`, `onSelectionChanged` |
| **UIInputField** | Поле ввода. `SetText()`, `SetError()`, `SetLabel()`, `onValueChanged` |
| **UICheckbox** | Чекбокс. `SetChecked()`, `onValueChanged` |

### Layout

| Компонент | Описание |
|-----------|----------|
| **UIVerticalLayout** | Вертикальный layout. `spacingToken`, `paddingToken`, `SetSpacing()`, `SetPadding()` |
| **UIHorizontalLayout** | Горизонтальный layout. Аналогично |
| **UIGridLayout** | Сетка. `cellSize`, `spacingToken`, `SetColumns()`, `SetRows()` |
| **UISpacer** | Гибкий отступ. `sizeToken`, `flexible` |

### Containers

| Компонент | Описание |
|-----------|----------|
| **UIPanel** | Контейнер с фоном. `backgroundColorToken`, `shapeToken`, `elevationToken`, `paddingToken` |
| **UICard** | Карточка с заголовком, контентом и actions. `SetTitle()`, `SetSubtitle()`, `GetContentSlot()` |
| **UIScrollView** | Прокручиваемый контейнер. `GetContent()`, `ScrollTo()` |
| **UIList** | Список с элементами. `SetItems()`, `AddItem()`, `UIListItem` как шаблон |
| **UIListItem** | Элемент списка. Title, subtitle, leading/trailing icons |
| **UITabContainer** | Контейнер с вкладками. `AddTab()`, `SelectTab()` |

### Navigation

| Компонент | Описание |
|-----------|----------|
| **UIHeader** | Шапка экрана. Заголовок, кнопка back |
| **UIToolbar** | Панель инструментов |
| **UIBottomBar** | Нижняя панель навигации |
| **UITabBar** | Вкладки |
| **UISidebar** | Боковая панель |

### Overlay

| Компонент | Описание |
|-----------|----------|
| **UIOverlay** | Полупрозрачный фон для модалок. `Show()`, `Hide()`, `onTap` |
| **UIModal** | Модальное окно |
| **UIDialog** | Диалог с кнопками |
| **UIToast** | Всплывающее уведомление |
| **UITooltip** | Подсказка при наведении |

### Feedback

| Компонент | Описание |
|-----------|----------|
| **UIProgressBar** | Полоса прогресса. `SetValue()`, `SetIndeterminate()` |
| **UISpinner** | Индикатор загрузки |
| **UIHealthBar** | Полоска здоровья. `SetHealth()`, `TakeDamage()`, `Heal()` |

---

## 7. Экраны и навигация

### UIScreen

Базовый класс экрана. Наследуйте для создания своих экранов.

**Жизненный цикл:**
- `OnScreenCreated()` — один раз при создании
- `OnScreenShow(data)` — перед показом
- `OnScreenShown()` — после анимации показа
- `OnScreenHide()`, `OnScreenHidden()` — при скрытии
- `OnScreenFocus()`, `OnScreenBlur()` — при переключении на другой экран
- `OnBackPressed()` — обработка кнопки «Назад»

```csharp
screen.Show(data: someData);
screen.Hide();
```

### UIScreenRegistry

ScriptableObject — реестр экранов. Создайте asset, добавьте ScreenEntry (screenId, prefab, preload, singleton).

### UINavigator

Stack-based навигация.

```csharp
await navigator.Push("inventory", data);
await navigator.Pop();
await navigator.Replace("main_menu");
navigator.PopToRoot();
navigator.CurrentScreen;
navigator.CanGoBack();
```

### TransitionPreset

Пресеты анимаций переходов: None, Fade, SlideLeft/Right/Up/Down, Scale, ScaleAndFade.

**Создание:** Assets > Create > UACF UI > Transition Preset

---

## 8. Анимация

### UITween

Встроенная система tweening (без DOTween).

```csharp
UITween.FadeTo(canvasGroup, 1f, 0.3f);
UITween.MoveTo(rectTransform, targetPos, 0.3f);
UITween.ScaleTo(rectTransform, Vector3.one, 0.3f);
UITween.ColorTo(graphic, color, 0.3f);
```

### TweenPreset

ScriptableObject с параметрами анимации: duration, delay, curve.

### UIAnimator

Компонент для применения анимаций к UI-элементу. Named animations (show, hide, bounce, shake).

### MicroInteraction

Микроанимации при hover/press: scale, цвет, звук.

### EasingFunctions

Библиотека функций сглаживания: Linear, EaseInQuad, EaseOutQuad, EaseInOutCubic, EaseOutBack, Spring и др.

---

## 9. UACF API

При установленном UACF доступны HTTP-эндпоинты для управления UI через AI-агентов.

**Базовый URL:** `http://localhost:7890` (порт UACF)

**Target format** (для parent, target): `{"instance_id": 123}`, `{"name": "Canvas"}`, `{"path": "Canvas/Panel"}`

### Bootstrap (первоначальная настройка)

Перед использованием API создайте токены и префабы. Unity Editor и UACF должны быть запущены.

| Метод | URL | Описание |
|-------|-----|----------|
| POST | `/api/ui/setup/bootstrap` | Полная настройка: токены (ColorPalette, TypographySet, SpacingScale, ShapeSet, ElevationSet), тема DefaultTheme, все префабы компонентов |
| POST | `/api/ui/setup/prefabs` | Только префабы компонентов |
| POST | `/api/ui/setup/tokens` | Только токены и тема |

### Theme API

| Метод | URL | Описание |
|-------|-----|----------|
| GET | `/api/ui/themes` | Список тем (id, name, path, is_active) |
| POST | `/api/ui/theme/create` | Создать тему |
| PUT | `/api/ui/theme/apply` | Применить тему (только в Play Mode) |
| GET | `/api/ui/theme/get` | Получить токены активной темы |

**theme/create body:** `{"theme_id":"monochrome","theme_name":"Monochrome","palette_id":"default","colors":{"primary":{"r":0,"g":0,"b":0,"a":1}}}`

**theme/apply body:** `{"theme_id":"default-dark"}`

### Token API

| Метод | URL | Описание |
|-------|-----|----------|
| PUT | `/api/ui/tokens/colors` | Обновить цвета палитры |
| POST | `/api/ui/tokens/colors/add-custom` | Добавить кастомный цвет |
| PUT | `/api/ui/tokens/typography` | Обновить типографику |
| PUT | `/api/ui/tokens/spacing` | Обновить отступы |

**tokens/colors body:** `{"palette_id":"default","colors":{"primary":{"r":0,"g":0,"b":0,"a":1}}}`

**tokens/colors/add-custom body:** `{"key":"myColor","color":{"r":0.5,"g":0.5,"b":0.5,"a":1}}`

**tokens/typography body:** `{"preset_id":"default","presets":{"h1":{"fontSize":32,"fontStyle":"Bold"}}}`

**tokens/spacing body:** `{"scale_id":"default","values":{"md":16,"lg":24}}`

### Screen API

| Метод | URL | Описание |
|-------|-----|----------|
| POST | `/api/ui/screen/create` | Создать экран (открыть сцену, Canvas, root UIPanel) |
| GET | `/api/ui/screen/hierarchy` | Иерархия экрана (screen_id в query) |
| POST | `/api/ui/screen/show` | Показать экран по screen_id, скрыть остальные |
| POST | `/api/ui/screen/from-spec` | Создать экран по JSON-спецификации (screen + layout + elements + theme) |

**screen/create body:** `{"screen_id":"main","name":"MainScreen","scene_path":"Assets/Scenes/SampleScene.unity"}` — scene_path опционален; если не указан, используется текущая сцена.

**screen/show body:** `{"screen_id":"main_menu","hide_others":true}` — hide_others (по умолчанию true) скрывает остальные экраны с UIScreenMarker.

**screen/from-spec body:** `{"scene_path":"Assets/Scenes/MyScene.unity","screen_id":"main_menu","name":"MainMenuScreen","theme_id":"default","canvas":{"referenceResolution":{"x":1920,"y":1080}},"operations":[{"id":"main","method":"POST","path":"/api/ui/layout/create","body":{"type":"vertical","name":"Main","parent":{"ref":"content"},"spacing":"md"}},{"id":"btn1","method":"POST","path":"/api/ui/element/add","body":{"parent":{"ref":"main"},"component":"UIButton","name":"PlayBtn","properties":{"labelText":"Play"}}}]}`. После screen/create доступны ref "root" (UIPanel) и "content" (Content внутри UIPanel). Layout обычно создаётся с parent: `{"ref":"content"}`.

### Layout API

| Метод | URL | Описание |
|-------|-----|----------|
| POST | `/api/ui/layout/create` | Создать layout (vertical, horizontal, grid) |

**layout/create body:** `{"type":"vertical","name":"Root","parent":{"name":"Canvas"}}` — type: `vertical`, `horizontal`, `grid`.

**Layout First — параметры layout/create:**
- `spacing` — число или токен ("sm", "md", "lg")
- `padding` — число (все стороны) или объект `{left, right, top, bottom}` (числа или токены)
- `childForceExpandWidth`, `childForceExpandHeight` — bool
- `childAlignment` — строка ("UpperLeft", "MiddleCenter" и т.д.)
- Для grid: `cellSize` — `{x, y}`, `constraint` — "Flexible"/"FixedColumnCount"/"FixedRowCount", `constraintCount` — int

### Element API

| Метод | URL | Описание |
|-------|-----|----------|
| POST | `/api/ui/element/add` | Добавить UI-элемент |
| PUT | `/api/ui/element/modify` | Изменить элемент (name, properties, rect) |
| DELETE | `/api/ui/element/remove` | Удалить элемент |
| POST | `/api/ui/element/reorder` | Изменить порядок (sibling_index) |

**element/add body:** `{"parent":{"instance_id":123},"component":"UIButton","name":"PlayButton","properties":{"labelText":"Play","variant":"Filled"}}` — component: UIText, UIImage, UIButton, UIPanel, UIVerticalLayout и др. (см. раздел 6). UIButton поддерживает variant: Filled, Outlined, Text, Tonal.

**properties поддерживает:**
- `labelText`, `variant` — UIButton
- `value`, `minValue`, `maxValue` — UISlider, UIProgressBar
- `isOn` — UIToggle, UICheckbox
- `options` — UIDropdown (массив строк)
- `currentHealth`, `maxHealth` — UIHealthBar
- `{"asset":"Assets/..."}` — ссылки на Sprite, Texture2D, Font, TMP_FontAsset

**Layout First — element/add поддерживает:**
- `rect` — `{anchorMin, anchorMax, offsetMin, offsetMax, sizeDelta}` — объекты с x, y
- `layout` — `{preferredWidth, preferredHeight, flexibleWidth, flexibleHeight, minWidth, minHeight, layoutPriority}` — для LayoutElement (при добавлении в layout-группу)
- `scrollRect` — для UIScrollView: `{horizontal, vertical, movementType, elasticity, scrollSensitivity}`
- `contentSizeFitter` — для контента ScrollView: `{horizontalFit, verticalFit}` — Unconstrained, PreferredSize, MinSize

**element/modify body:** `{"target":{"instance_id":123},"set":{"name":"X","properties":{},"layout":{}},"rect":{},"scrollRect":{},"contentSizeFitter":{}}`
- `set.layout` — `{preferredWidth, preferredHeight, flexibleWidth, flexibleHeight, minWidth, minHeight, layoutPriority}` — обновить LayoutElement
- `scrollRect` — `{horizontal, vertical, movementType, elasticity, scrollSensitivity}`
- `contentSizeFitter` — `{horizontalFit, verticalFit}`

**element/remove body:** `{"target":{"instance_id":123}}`

**element/reorder body:** `{"target":{"instance_id":123},"sibling_index":0}`

### Style API

| Метод | URL | Описание |
|-------|-----|----------|
| POST | `/api/ui/style/create` | Создать UIStyle. Body: `{"key":"my_style","parent":"base_style","normal":{"backgroundColorToken":"primary","textColorToken":"onPrimary"},"hovered":{...}}` |
| PUT | `/api/ui/style/apply` | Применить стиль к элементу. Body: `{"target":{"instance_id":123},"style":"my_style"}` (style — styleKey или asset path) |
| GET | `/api/ui/style/list` | Список всех UIStyle (key, path, has_parent, parent_key) |

### Canvas API

| Метод | URL | Описание |
|-------|-----|----------|
| PUT | `/api/ui/canvas/configure` | Настроить Canvas/CanvasScaler. Body: `{"target":{"name":"Canvas"},"renderMode":"Overlay","referenceResolution":{"x":1920,"y":1080},"matchWidthOrHeight":0.5,"scaleFactor":1}` |

**screen/create** поддерживает опциональный блок `canvas`: `{"renderMode":"Overlay","referenceResolution":{"x":1920,"y":1080},"matchWidthOrHeight":0.5,"scaleFactor":1}`

### Прочее

| Метод | URL | Описание |
|-------|-----|----------|
| GET | `/api/ui/components/list` | Список доступных компонентов (полный список + properties по типу) |
| POST | `/api/ui/batch` | Пакетные операции с цепочкой parent: `{"ref":"op_id"}` |

**batch body:** `{"operations":[{"id":"op1","method":"POST","path":"/api/ui/layout/create","body":{...}},{"id":"op2","method":"POST","path":"/api/ui/element/add","body":{"parent":{"ref":"op1"},...}}],"stop_on_error":true}`

**Поддерживаемые batch-операции:** `POST /api/ui/screen/create`, `POST /api/ui/layout/create`, `POST /api/ui/element/add`, `PUT /api/ui/element/modify`, `PUT /api/ui/theme/apply`.

### Пример: bootstrap

```bash
curl -X POST http://localhost:7890/api/ui/setup/bootstrap
```

Bootstrap создаёт токены (ColorPalette, TypographySet, SpacingScale, ShapeSet, ElevationSet), тему DefaultTheme, StyleSheet с базовыми UIStyle для UIButton, UIText, UIPanel и др., назначает theme.defaultStyles, и префабы всех компонентов.

### Пример: создать экран

```bash
curl -X POST http://localhost:7890/api/ui/screen/create \
  -H "Content-Type: application/json" \
  -d '{
    "screen_id": "main_menu",
    "name": "MainMenuScreen",
    "scene_path": "Assets/Scenes/SampleScene.unity"
  }'
```

### Пример: создать layout и добавить кнопку

```bash
# 1. Создать layout
curl -X POST http://localhost:7890/api/ui/layout/create \
  -H "Content-Type: application/json" \
  -d '{"type":"vertical","name":"Root","parent":{"name":"Canvas"}}'

# 2. Добавить кнопку (parent.instance_id из ответа шага 1)
curl -X POST http://localhost:7890/api/ui/element/add \
  -H "Content-Type: application/json" \
  -d '{
    "parent": {"instance_id": 15001},
    "component": "UIButton",
    "name": "PlayButton",
    "properties": {"labelText": "Play", "variant": "Filled"}
  }'
```

### Пример: применить тему

```bash
curl -X PUT http://localhost:7890/api/ui/theme/apply \
  -H "Content-Type: application/json" \
  -d '{"theme_id": "default-dark"}'
```

### Пример: batch с цепочкой

```bash
curl -X POST http://localhost:7890/api/ui/batch \
  -H "Content-Type: application/json" \
  -d '{
    "operations": [
      {"id": "layout1", "method": "POST", "path": "/api/ui/layout/create", "body": {"type": "vertical", "name": "Root", "parent": {"name": "Canvas"}}},
      {"id": "btn1", "method": "POST", "path": "/api/ui/element/add", "body": {"parent": {"ref": "layout1"}, "component": "UIButton", "name": "PlayButton", "properties": {"labelText": "Play"}}}
    ],
    "stop_on_error": true
  }'
```

### Пример: screen/from-spec (создать экран по спецификации)

```bash
curl -X POST http://localhost:7890/api/ui/screen/from-spec \
  -H "Content-Type: application/json" \
  -d '{
    "scene_path": "Assets/Scenes/SampleScene.unity",
    "screen_id": "main_menu",
    "name": "MainMenuScreen",
    "theme_id": "default",
    "canvas": {"referenceResolution": {"x": 1920, "y": 1080}},
    "operations": [
      {"id": "main", "method": "POST", "path": "/api/ui/layout/create", "body": {"type": "vertical", "name": "Main", "parent": {"ref": "content"}, "spacing": "md", "padding": "md"}},
      {"id": "btn1", "method": "POST", "path": "/api/ui/element/add", "body": {"parent": {"ref": "main"}, "component": "UIButton", "name": "PlayBtn", "properties": {"labelText": "Play", "variant": "Filled"}}},
      {"id": "btn2", "method": "POST", "path": "/api/ui/element/add", "body": {"parent": {"ref": "main"}, "component": "UIButton", "name": "SettingsBtn", "properties": {"labelText": "Settings", "variant": "Outlined"}}}
    ]
  }'
```

### Пример: screen/show (переключить экран)

```bash
curl -X POST http://localhost:7890/api/ui/screen/show \
  -H "Content-Type: application/json" \
  -d '{"screen_id": "main_menu", "hide_others": true}'
```

---

## 10. Layout First (веб-подход)

Верстка строится по принципу «сначала layout, потом элементы» — как в CSS Flexbox.

1. **Создать layout-контейнер** — `layout/create` с параметрами spacing, padding
2. **Добавить элементы** — `element/add` с parent = layout; элементы автоматически получают LayoutElement с размерами по умолчанию
3. **Переопределить при необходимости** — `layout` в body или `element/modify` с `set.layout`

```bash
# Layout First: создать секцию с отступами
curl -X POST http://localhost:7890/api/ui/layout/create \
  -H "Content-Type: application/json" \
  -d '{"type":"vertical","name":"Section","parent":{"name":"Content"},"spacing":"md","padding":{"left":"md","right":"md","top":"sm","bottom":"sm"}}'

# Добавить кнопку — получит preferredWidth:120, preferredHeight:40 автоматически
curl -X POST http://localhost:7890/api/ui/element/add \
  -H "Content-Type: application/json" \
  -d '{"parent":{"name":"Section"},"component":"UIButton","name":"OK","properties":{"labelText":"OK"}}'

# Кнопка с кастомным layout
curl -X POST http://localhost:7890/api/ui/element/add \
  -H "Content-Type: application/json" \
  -d '{"parent":{"name":"Section"},"component":"UIButton","name":"WideBtn","properties":{"labelText":"Wide"},"layout":{"preferredWidth":200,"flexibleWidth":1}}'
```

---

## 11. Примеры

### Создание темы вручную

1. Assets > Create > UACF UI > Color Palette
2. Настройте primary, secondary, background, surface
3. Assets > Create > UACF UI > Theme
4. Назначьте палитру и остальные токены
5. В коде: `ThemeManager.SetTheme(myTheme)`

### Простой экран с кнопками

1. Создайте Canvas с ThemeApplier
2. GameObject > UACF UI > Layout > Vertical Layout
3. В него добавьте: UIButton (Filled), UIButton (Outlined), UIButton (Text)
4. Назначьте стили или Theme.defaultStyles

### Экран с навигацией

1. Создайте UIScreenRegistry, добавьте экраны
2. Создайте UINavigator с ссылкой на registry
3. `navigator.Push("inventory")` — переход на экран инвентаря
4. `navigator.Pop()` — возврат

### Кастомный цвет в палитре

В ColorPalette добавьте в customColors:
- key: `"rarity_legendary"`
- color: золотой
- description: `"Legendary item"`

Используйте в компонентах: `colorToken = "rarity_legendary"`

### Полный сценарий через UACF API

1. Запустите Unity Editor и UACF.
2. Выполните `POST /api/ui/setup/bootstrap` для создания токенов и префабов.
3. Используйте `POST /api/ui/batch` для пакетного создания экрана с несколькими элементами (screen/create, layout/create, element/add с `parent: {"ref":"op_id"}`). Примеры curl — в разделе 9.

---

## Структура пакета

```
Packages/com.uacf.ui-framework/
├── Runtime/
│   ├── Tokens/          — ColorPalette, TypographySet, SpacingScale, ShapeSet, ElevationSet, IconSet, Theme
│   ├── Styles/          — UIStyle, UIStyleState, StyleSheet, StyleResolver, StyleBinding, ResolvedStyle
│   ├── Theming/         — ThemeManager, ThemeApplier, ThemeListener, TokenReference
│   ├── Components/      — Base, Display, Input, Layout, Containers, Navigation, Overlay, Feedback
│   ├── Screens/         — UIScreen, UIScreenRegistry, UINavigator, NavigationStack, TransitionPreset
│   ├── Animation/       — UIAnimator, UITween, TweenPreset, EasingFunctions
│   └── Utility/         — SafeAreaHelper, AnchorPresets, UIExtensions, UIConstants
├── Editor/
│   ├── UACF/            — Handlers для UACF API
│   └── Creation/        — UIMenuItems
├── Resources/
│   └── UACF_UI/         — DefaultTheme
└── Tests/
    └── Runtime/         — TokenResolutionTests
```

---

## Утилиты

### SafeAreaHelper

Адаптация под safe area (notch, закруглённые углы). Применяет `Screen.safeArea` к RectTransform.

### AnchorPresets

Статические методы для установки anchor:

```csharp
AnchorPresets.StretchFull(rectTransform);
AnchorPresets.TopStretch(rectTransform, 56f);
AnchorPresets.CenterMiddle(rectTransform, new Vector2(200, 100));
```

### UIExtensions

Extension-методы: `SetAlpha()`, `GetOrAddComponent<T>()`, `DestroyChildren()`, `GetScreenRect()` и др.
