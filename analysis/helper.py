import json
import pathlib


ROOT = pathlib.Path(__file__).parent.parent.absolute()
LEVEL = "Reax3D"


def load(filename: str, source = LEVEL) -> dict:
  with open(ROOT / source / "data" / filename) as source:
    return json.load(source)
