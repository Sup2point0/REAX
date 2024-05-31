from matplotlib import pyplot as plt

import helper


data = helper.load("data.json")[0]
collisions = data["collisions"]
reactions = data["reactions"]

fig, ax = plt.subplots()

ax.plot(range(len(collisions)), collisions)
ax.plot(range(len(reactions)), reactions)

plt.show()
