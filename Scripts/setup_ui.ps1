# UACF UI Framework — Bootstrap via UACF API (PowerShell)
# Run with Unity Editor open and UACF server running (default port 7890)

$UACF_URL = if ($env:UACF_URL) { $env:UACF_URL } else { "http://localhost:7890" }

Write-Host "=== UACF UI Framework Bootstrap ==="
Write-Host "UACF URL: $UACF_URL"
Write-Host ""

# 1. Bootstrap
Write-Host "1. Running bootstrap (tokens + theme + prefabs)..."
try {
    $resp = Invoke-RestMethod -Uri "$UACF_URL/api/ui/setup/bootstrap" -Method Post -ContentType "application/json" -Body '{}'
    Write-Host "   OK: $($resp.total) items created"
} catch {
    Write-Host "   FAILED: $_"
    exit 1
}

# 2. Sample screen
Write-Host ""
Write-Host "2. Creating sample menu..."
$screenBody = @{ component = "UIVerticalLayout"; name = "MainMenu"; properties = @{ spacingToken = "lg"; paddingToken = "xl" } } | ConvertTo-Json
$screenResp = Invoke-RestMethod -Uri "$UACF_URL/api/ui/element/add" -Method Post -ContentType "application/json" -Body $screenBody
$parentId = $screenResp.created[0].instance_id

if ($parentId) {
    foreach ($label in @("Play", "Settings", "Quit")) {
        $btnBody = @{ parent = @{ instance_id = $parentId }; component = "UIButton"; name = "${label}Button"; properties = @{ labelText = $label; variant = "Filled" } } | ConvertTo-Json
        Invoke-RestMethod -Uri "$UACF_URL/api/ui/element/add" -Method Post -ContentType "application/json" -Body $btnBody | Out-Null
        Write-Host "   Added button: $label"
    }
}

Write-Host ""
Write-Host "=== Bootstrap complete ==="
