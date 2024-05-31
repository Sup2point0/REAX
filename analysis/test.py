print("--- RUNNING! ---")

import math
from random import randint

from matplotlib import pyplot as plt

import helper
from heatrace import Heatrace


ticks = 500

test = [
  {"target-ticks": ticks}
]

for i in range(1000):
  run = [randint(97, 103)]

  for j in range(1, ticks):
    run.append(round(
      run[-1] + randint(-200, 240) / 100
    ))

  test.append({"tests": run})

trace = Heatrace(test, "tests")
trace.show()

# plt.inferno()
# fig, ax = plt.subplots()
# im = ax.imshow(trace.lattice)
# plt.show()


print("--- DONE! ---")
