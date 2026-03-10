# Changelog

## [1.0.1] - 2025-03-10

### Added

- **UITokenHandler**: Full implementation of UpdateColors, AddCustomColor, UpdateTypography, UpdateSpacing — modifies ScriptableObject assets
- **UIBatchHandler**: Batch operations with `parent: {"ref":"op_id"}` for chaining element/add, layout/create, screen/create
- **UIScreenHandler**: Create screen — open scene, create Canvas, root UIPanel (stretch), ThemeApplier
- **UIThemeHandler**: Create theme — new Theme + ColorPalette with custom colors
- **UIElementHandler**: Modify (name, properties, RectTransform), Remove, Reorder
- **UILayoutHandler**: Create layout (vertical/horizontal/grid) with parent resolution

### Changed

- All UI API handlers now fully functional — no stubs. All operations via HTTP API without one-off scripts.

## [1.0.0] - 2025-03-10

### Added

- Design Tokens: ColorPalette, TypographySet, SpacingScale, ShapeSet, ElevationSet, IconSet, Theme
- Style System: UIStyleState, UIStyle, StyleSheet, StyleResolver, StyleBinding, ResolvedStyle
- Theming: ThemeManager, ThemeApplier, ThemeListener, TokenReference, IThemeable
- Base Components: UIComponentBase, UIInteractableBase, MicroInteraction
- Animation: UITween, EasingFunctions, UIAnimator, TweenPreset
- Display Components: UIText, UIImage, UIIcon, UIDivider, UIBadge
- Layout Components: UIVerticalLayout, UIHorizontalLayout, UIGridLayout, UISpacer
- Input Components: UIButton, UIIconButton, UIToggle, UISlider, UIDropdown, UIInputField, UICheckbox
- Container Components: UIPanel, UICard, UIScrollView, UIList, UIListItem, UITabContainer
- Navigation Components: UIHeader, UIToolbar, UIBottomBar, UITabBar, UISidebar
- Overlay Components: UIOverlay, UIModal, UIDialog, UIToast, UITooltip
- Feedback Components: UIProgressBar, UISpinner, UIHealthBar
- Screen System: UIScreen, UIScreenRegistry, UINavigator, NavigationStack, TransitionPreset
- Utilities: SafeAreaHelper, AnchorPresets, UIExtensions, UIConstants
- UACF Integration: UI handlers for themes, tokens, screens, elements, styles, layout, batch
- Editor: UIMenuItems for GameObject and Assets creation
