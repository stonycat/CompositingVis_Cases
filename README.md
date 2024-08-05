<h2 align="center">CompositingVis: Exploring Interactions for Creating Composite Visualizations in Immersive Environments</h2>

<div align="center"><i>VIS 2024</i></div>

<p align="center">
  <a href="">Paper</a> |
  <a href="">Supplemental Video</a>
</p>

CompositingVis is a design space for embodied interactions that can be used to create composite visualization in immersive environments. This repository is the code of our case study to demonstrate the usage of the design space in our paper. The code is partially built on top of [IATK] (https://github.com/MaximeCordeil/IATK) (created by Cordeil Maxime et al.).

### Design Space
![figures/DesignSpace.png](figures/DesignSpace.png) 
How to use this design space? 
- Check the data relationships between two views (input)
- Define the composite visualization type (output)
- Determine the interaction target for each view and the order of manipulations
- Select and combine proper (and  multiple) 3D manipulation(s) in the design space for the output composite views
- Determine the parameterized decision conditions for the relative state of the views based on the specific situation (interaction device, user, task, etc.).

### Cases
- Juxtaposed View
- Integrated View
- Superimposed View
- Overloaded View
- Nested View

### Acknowledgements
We used several third-party toolkits and projects:
- [IATK] (https://github.com/MaximeCordeil/IATK)
- [Tilia](https://www.vrtk.io/tilia.html)
- [CSVHelper](https://joshclose.github.io/CsvHelper/)

