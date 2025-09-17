#!/bin/bash
# Build and publish all Daisy plugins to the runtime plugins directory
# This script should be run from the repository root

set -e

echo "Building and publishing Daisy plugins..."

# Ensure we're in the repository root
if [ ! -f "Daisy-m4.sln" ]; then
    echo "Error: This script must be run from the repository root (where Daisy-m4.sln is located)"
    exit 1
fi

# Create plugins directory if it doesn't exist
mkdir -p src/Daisy/bin/Debug/net8.0/plugins

# Find and publish all plugin projects
PLUGIN_PROJECTS=$(find . -name "Daisy.Abilities.*.csproj" -o -name "Daisy.Receivers.*.csproj" -o -name "Daisy.Transmitters.*.csproj" -o -name "Daisy.Workflows.*.csproj" | grep -v obj)

echo "Found plugin projects:"
echo "$PLUGIN_PROJECTS"
echo ""

for proj in $PLUGIN_PROJECTS; do
    plugin_name=$(basename "$proj" .csproj)
    echo "Publishing $plugin_name..."
    dotnet publish "$proj" \
        --configuration Debug \
        --output "src/Daisy/bin/Debug/net8.0/plugins/$plugin_name" \
        --no-restore
done

echo ""
echo "Plugin publishing completed successfully!"
echo "Plugins directory: src/Daisy/bin/Debug/net8.0/plugins/"
ls -la src/Daisy/bin/Debug/net8.0/plugins/