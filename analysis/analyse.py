print("--- RUNNING! ---")

from matplotlib import pyplot as plt

import helper
from heatrace import Heatrace


data = helper.load("data.json")

trace = Heatrace(data, "collisions")
trace.show()
