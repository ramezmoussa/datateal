#!/bin/bash
set -e

KERNEL_PIP=/opt/venvs/kernel/bin/pip
REQUIREMENTS_FILE=/etc/duckhouse/kernel-requirements.txt

if [ -f "$REQUIREMENTS_FILE" ]; then
    echo "Installing kernel packages from $REQUIREMENTS_FILE"
    "$KERNEL_PIP" install --no-cache-dir -r "$REQUIREMENTS_FILE"
fi

if [ -n "$KERNEL_PACKAGES" ]; then
    echo "Installing kernel packages from KERNEL_PACKAGES env var"
    "$KERNEL_PIP" install --no-cache-dir $KERNEL_PACKAGES
fi

exec /opt/venvs/api/bin/duckhouse-runtime
