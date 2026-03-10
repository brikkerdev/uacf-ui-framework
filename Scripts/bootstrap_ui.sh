#!/bin/bash
# UACF UI Framework — Bootstrap via UACF API
# Run with Unity Editor open and UACF server running (default port 7890)

UACF_URL="${UACF_URL:-http://localhost:7890}"

echo "=== UACF UI Framework Bootstrap ==="
echo "UACF URL: $UACF_URL"
echo ""

# 1. Bootstrap — create tokens, theme, all prefabs
echo "1. Running bootstrap (tokens + theme + prefabs)..."
RESP=$(curl -s -X POST "$UACF_URL/api/ui/setup/bootstrap" \
  -H "Content-Type: application/json" \
  -d '{}')

if echo "$RESP" | grep -q '"success":true'; then
  echo "   OK: $(echo "$RESP" | grep -o '"total":[0-9]*' | cut -d: -f2) items created"
else
  echo "   FAILED: $RESP"
  exit 1
fi

# 2. Create sample screen with buttons (optional)
echo ""
echo "2. Creating sample menu screen..."

# Create canvas/root first
SCREEN_RESP=$(curl -s -X POST "$UACF_URL/api/ui/element/add" \
  -H "Content-Type: application/json" \
  -d '{
    "component": "UIVerticalLayout",
    "name": "MainMenu",
    "properties": {"spacingToken": "lg", "paddingToken": "xl"}
  }')

PARENT_ID=$(echo "$SCREEN_RESP" | grep -o '"instance_id":[0-9]*' | head -1 | cut -d: -f2)

if [ -n "$PARENT_ID" ]; then
  echo "   Root layout created (instance_id: $PARENT_ID)"

  # Add buttons
  for label in "Play" "Settings" "Quit"; do
    curl -s -X POST "$UACF_URL/api/ui/element/add" \
      -H "Content-Type: application/json" \
      -d "{
        \"parent\": {\"instance_id\": $PARENT_ID},
        \"component\": \"UIButton\",
        \"name\": \"${label}Button\",
        \"properties\": {\"labelText\": \"$label\", \"variant\": \"Filled\"}
      }" > /dev/null
    echo "   Added button: $label"
  done
else
  echo "   Could not create root (UACF may need a scene open)"
fi

echo ""
echo "=== Bootstrap complete ==="
echo "Use GameObject > UACF UI > [component] to add UI elements."
echo "Or call POST /api/ui/element/add to add via API."
