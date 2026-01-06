# Grid-Building-System  
Project to make a Grid Building System for placing buildings in a grid

###### Scripts
Model:
- `BuildingShapeUnit` - empty script for building shape unit, used for identifying building shape inside Wrapper.
  - using `BuildingShapeUnit` as a component on a GameObject will allow you to define the shape of the building in the grid without calculating x and y, this also helps when we want shapes beyond square and rectangle.
  - each `BuildingShapeUnit` represents one grid cell occupied by the building, it should be placed at the center of the grid position.
- `BuildingModel`.cs - script for building model, gives access to building mesh inside Wrapper.
Grid:
- `BuildingGrid`.cs - main grid script for managing grid cells and building placement.
- `BuildingGridCell`.cs - Cell data structure for individual grid cells.
Building Placement:
- `BuildingData`.cs - data structure for building information, including the prefab of the building. Can be used to instantiate buildings on the grid.
- `BuildingPreview`.cs - script for previewing building placement on the grid.
System:
- `GridBuildingConfig`.cs - configuration file for grid building system.
- `GridBuildingConfig2`.cs - In future we can have multiple configurations for different grid types.
- 

## Assumptions:
1. The grid is made of square cells.
2. Each building occupies one or more grid cells.
3. The grid origin (0,0) is at the bottom-left corner.
4. The grid cells are indexed starting from (0,0) at the bottom-left corner.
5. Buildings can be placed only within the bounds of the grid.
6. Buildings cannot overlap with each other on the grid.
7. The grid size (number of rows and columns) is predefined and fixed.
8. The building placement is done in a 2D plane (X, Z) with Y being the height.
9. Buildings can only be rotated in 90-degree increments along the Y-axis.
10. I am using a Grid with Cell size of 5x5 units for Confirming no bias is left due to 1x1 or 2x2 which are common approaches.
