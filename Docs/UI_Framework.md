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

### Theme API

| Метод | URL | Описание |
|-------|-----|----------|
| GET | `/api/ui/themes` | Список тем |
| POST | `/api/ui/theme/create` | Создать тему |
| PUT | `/api/ui/theme/apply` | Применить тему |
| GET | `/api/ui/theme/get` | Получить токены темы |

### Token API

| Метод | URL | Описание |
|-------|-----|----------|
| PUT | `/api/ui/tokens/colors` | Обновить цвета |
| POST | `/api/ui/tokens/colors/add-custom` | Добавить кастомный цвет |
| PUT | `/api/ui/tokens/typography` | Обновить типографику |
| PUT | `/api/ui/tokens/spacing` | Обновить отступы |

### Screen API

| Метод | URL | Описание |
|-------|-----|----------|
| POST | `/api/ui/screen/create` | Создать экран из шаблона |
| GET | `/api/ui/screen/hierarchy` | Иерархия экрана |

### Element API

| Метод | URL | Описание |
|-------|-----|----------|
| POST | `/api/ui/element/add` | Добавить UI-элемент |

### Другие

| Метод | URL | Описание |
|-------|-----|----------|
| GET | `/api/ui/components/list` | Список компонентов |
| POST | `/api/ui/batch` | Пакетные операции |

### Пример: создать экран

```bash
curl -X POST http://localhost:7890/api/ui/screen/create \
  -H "Content-Type: application/json" \
  -d '{
    "screen_id": "main_menu",
    "template": "HeaderContentTemplate",
    "name": "MainMenuScreen",
    "header": {"title": "Main Menu"}
  }'
```

### Пример: добавить элемент

```bash
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
  -d '{"theme_id": "default-dark", "target": "scene"}'
```

---

## 10. Примеры

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

Используйте `POST /api/ui/batch` для пакетного создания экрана с несколькими элементами. Примеры curl — в разделе 9.

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
