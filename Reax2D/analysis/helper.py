import json
import pathlib


ROOT = pathlib.Path(__file__).parent.parent.absolute()


def load(filename: str) -> dict:
  with open(ROOT / "data" / filename) as source:
    return json.load(source)
