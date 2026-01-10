#!/bin/bash
# Kill process on specified port
# Usage: ./kill-port.sh <port>

PORT=$1

if [ -z "$PORT" ]; then
    echo "Usage: $0 <port>"
    exit 1
fi

echo "Checking for processes on port $PORT..."

# Find PIDs using the port on Windows
PIDS=$(netstat -ano | grep ":$PORT " | grep "LISTENING" | awk '{print $5}' | sort -u)

if [ -z "$PIDS" ]; then
    echo "No process found on port $PORT"
    exit 0
fi

for PID in $PIDS; do
    if [ "$PID" != "0" ]; then
        echo "Killing process $PID on port $PORT..."
        taskkill //F //PID $PID 2>/dev/null || echo "Failed to kill process $PID"
    fi
done

# Wait for port to be released
sleep 0.5

echo "Port $PORT cleanup complete"
