#!/bin/bash

# Navigate to the script directory
cd "$(dirname "$0")"

# Determine if we should serve the docs
DOCFX_SERVE="--serve"
if [ "$CI" = "true" ] || [ "$CI" = "True" ]; then
    DOCFX_SERVE=""
fi

echo "Updating docfx..."
dotnet tool update docfx --tool-path packages --verbosity quiet

echo "Building documentation..."
# packages/docfx init -y -o Documentation
packages/docfx Documentation/docfx.json $DOCFX_SERVE
