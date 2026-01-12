import yaml
from enum import Enum
import smartbuildsim.cli.bim as bim_cli
from typer.testing import CliRunner

# 1. The Fix: Define how PyYAML should handle Enum objects
def enum_representer(dumper, data):
    # This tells PyYAML to treat the Enum member as its underlying value (e.g., 'temperature')
    return dumper.represent_scalar('tag:yaml.org,2002:str', str(data.value))

# 2. Register the fix globally for the SafeDumper
yaml.SafeDumper.add_multi_representer(Enum, enum_representer)

# 3. Trigger the smartbuildsim initialization via its internal CLI logic
print("Registering Enum representer and initializing schema...")
runner = CliRunner()
# This mimics: smartbuildsim bim init schema.yaml --scenario office-small
result = runner.invoke(bim_cli.app, ["init", "schema.yaml", "--scenario", "office-small"])

if result.exit_code == 0:
    print("Success! schema.yaml has been created.")
else:
    print(f"Error: {result.output}")