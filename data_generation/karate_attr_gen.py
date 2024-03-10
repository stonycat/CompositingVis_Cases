import networkx as nx
import random

# Load the Karate Club graph
G = nx.karate_club_graph()

# Generate random attributes for each node
for node in G.nodes:
    G.nodes[node]['Strength'] = random.randint(1, 100)
    G.nodes[node]['Agility'] = random.randint(1, 100)
    G.nodes[node]['Endurance'] = random.randint(1, 100)
    G.nodes[node]['Intelligence'] = random.randint(1, 100)

# Print the attributes for each node
# for node in G.nodes:
#     print(f"Node {node}: {G.nodes[node]}")

import matplotlib.pyplot as plt

# Get the attributes for each node
strength_values = [G.nodes[node]['Strength'] for node in G.nodes]
agility_values = [G.nodes[node]['Agility'] for node in G.nodes]
endurance_values = [G.nodes[node]['Endurance'] for node in G.nodes]
intelligence_values = [G.nodes[node]['Intelligence'] for node in G.nodes]

print(4)
print(len(G.nodes))
print("Strength")
for v in strength_values:
    print(v)
print("Agility")
for v in agility_values:
    print(v)
print("Endurance")
for v in endurance_values:
    print(v)
print("Intelligence")
for v in intelligence_values:
    print(v)

# Create a bar chart
plt.bar(G.nodes, strength_values, label='Strength')
plt.bar(G.nodes, agility_values, bottom=strength_values, label='Agility')
plt.bar(G.nodes, endurance_values, bottom=[i+j for i,j in zip(strength_values, agility_values)], label='Endurance')
plt.bar(G.nodes, intelligence_values, bottom=[i+j+k for i,j,k in zip(strength_values, agility_values, endurance_values)], label='Intelligence')

# Add labels and title
plt.xlabel('Node')
plt.ylabel('Attribute Value')
plt.title('Node Attributes in Karate Club Graph')
plt.legend()

# Show the chart
plt.show()