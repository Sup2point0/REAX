import json

with open("../data/data.json", "r+") as source:
  data = json.load(source)
  source.seek(0)
  source.truncate()
  json.dump(data, source, indent = None)
