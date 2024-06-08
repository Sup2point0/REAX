print("--- RUNNING! ---")

from datetime import datetime

from matplotlib import pyplot as plt

import helper
from heatrace import Heatrace


data = helper.load("exp-1-1.json")

trace = Heatrace(data, "particles.OSi")#, apex = 40)
trace.resize(height = 300)
trace.show()
trace.render.save(
  helper.ROOT / "renders" / (str(datetime.now().strftime("%m-%d %H-%M")) + ".png")
)

print("--- DONE! ---")
