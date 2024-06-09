print("--- RUNNING! ---")

from datetime import datetime

from matplotlib import pyplot as plt

import helper
from heatrace import Heatrace


exp = "exp-3-low"
particle = "SiSi"

data = helper.load(f"exp/{exp}.json")#[:31]

trace = Heatrace(data, f"particles.{particle}", apex = 40)
trace.resize(width = 600, height = 200)
trace.show()

# shard = str(datetime.now().strftime("%m-%d %H-%M"))
shard = f"{exp}-{particle}"
trace.render.save(
  helper.ROOT / "renders" / (shard + ".png")
)

print("--- DONE! ---")
