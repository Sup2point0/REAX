from matplotlib import pyplot as plt

import helper


data = helper.load("data.json")
collisions = data[0]["collisions"]

fig, ax = plt.subplots()

ax.plot(range(len(collisions)), collisions)

plt.show()
