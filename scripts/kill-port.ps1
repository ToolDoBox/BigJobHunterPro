param(
    [Parameter(Mandatory=$true)]
    [int]$Port
)

Write-Host "Checking for processes on port $Port..." -ForegroundColor Yellow

# Find process using the port
$connections = netstat -ano | Select-String ":$Port\s" | Select-String "LISTENING"

if ($connections) {
    foreach ($connection in $connections) {
        $processId = $connection -replace '.*\s(\d+)\s*$', '$1'
        if ($processId -match '^\d+$') {
            Write-Host "Killing process $processId on port $Port..." -ForegroundColor Red
            try {
                Stop-Process -Id $processId -Force -ErrorAction Stop
                Write-Host "Process $processId terminated successfully." -ForegroundColor Green
            } catch {
                Write-Host "Failed to kill process $processId : $_" -ForegroundColor Red
            }
        }
    }
    # Give it a moment to release the port
    Start-Sleep -Milliseconds 500
} else {
    Write-Host "No process found on port $Port" -ForegroundColor Green
}
