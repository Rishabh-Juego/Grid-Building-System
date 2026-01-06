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


### Flow
Creating a Building:
1. Define the grid size and cell size in `GridBuildingConfig`.cs.
2. Create a Building Modal prefab with `BuildingModel` and multiple `BuildingShapeUnit` components to define its shape.
   1. Each `BuildingShapeUnit` should be positioned at the center of the grid cell it occupies.
   2. The `BuildingModel` script should have a Wrapper which hold the building mesh and the `BuildingShapeUnit` components as children.
3. Use `BuildingData`.cs to store information about the building, including a reference to the modal prefab.

Placing a Building on the Grid:
1. Use `BuildingPreview`.cs to visualize the building placement on the grid.
   1. The preview should show the building at the mouse position, snapping to the nearest grid cell.
   2. The preview should change color to indicate whether the placement is valid (e.g., green for valid, red for invalid).
2. When the player confirms the placement, use `BuildingGrid`.cs to check if the building can be placed at the desired grid position.
3. If the placement is valid, instantiate the building prefab at the grid position and update the grid cells to mark them as occupied (`BuildingGrid.SetBuilding()`).
4. The new Building is placed on the grid and cannot overlap with existing buildings, We have a `Building` script on a prefab, the new building will be a child of this prefab.