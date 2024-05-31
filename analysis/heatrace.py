from collections import defaultdict

import numpy as np
from PIL import Image

import analysis.colourise as colourise


class Heatrace:
  '''Render multiple experiment runs in a heatmap.'''

  def __init__(self,
    data: list[dict],
    feature: str,
  ):
    config, *runs = data

    # Create lattice
    trace = defaultdict(lambda: 0)
    apexValue = 0

    for run in runs:
      for tick, value in enumerate(run[feature]):
        if value > apexValue:
          apexValue = value
        trace[(tick, value)] += 1

    apexFreq = max(trace.values())

    # Create heatmap
    shape = (config["target-ticks"] +1, apexValue +1, 3)
    lattice = np.full(shape, 16)

    for cord, freq in trace.items():
      intensity = (freq / apexFreq) ** 0.5
      col = colourise.DEFAULT[round(256 * intensity)]
      x, y = cord
      lattice[x][y] = np.array(col) * 255

    self.lattice = np.rot90(lattice)
    self.render = Image.fromarray(np.uint8(self.lattice), "RGB")

  def show(self):
    '''Display the heatmap.'''

    self.render.show()
