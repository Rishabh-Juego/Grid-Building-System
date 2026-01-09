# Grid-Building-System  
Project to make a Grid Building System for placing buildings in a grid

###### Scripts
Core Scripts:
shape data:
- `BuildingShapeUnit` - empty script for building shape unit, used for identifying building shape inside Wrapper.
  - using `BuildingShapeUnit` as a component on a GameObject will allow you to define the shape of the building in the grid without calculating x and y, this also helps when we want shapes beyond square and rectangle.
  - each `BuildingShapeUnit` represents one grid cell occupied by the building, it should be placed at the center of the grid position.
  - Gizmos are used to visualize the shape units in the editor.
- `BuildingModel`.cs - script for building model, gives access to building mesh inside Wrapper so we can rotate and position it correctly.
Grid:
- `BuildingGrid`.cs - main grid script for creating grid cells by defining width and height. Manages grid cell data when placing buildings.
- `BuildingGridCell`.cs - Cell data structure for individual grid cells.
Building:
- `Building`.cs - script for placed building on the grid, when a building is placed, we do not want to keep it directly, so this script acts as a common parent prefab for all buildings.
- `BuildingArt`.cs - Actual art of the building. This is empty and used for identification purposes.
- `BuildingData`.cs - scriptable object for building information, including the prefab of the building. Can be used to instantiate buildings on the grid.
- `Wrapper`.cs - script for handling building model rotation and position adjustments based on grid alignment.
- `AngledPosition`.cs - helper script for handling position adjustments when buildings are rotated at angles.
Placement:
- `BuildingPreview`.cs - script for previewing building placement on the grid. All colliders are disabled for this prefab and renderers are all using a configured material depending on placement.
- `BuildingSystem`.cs - main script for handling building placement logic, including raycasting, validating placement, and finalizing building placement on the grid.
  config:
- `GridBuildingConfig`.cs - configuration file for grid building system.
- `GridBuildingConfig2`.cs - In future we can have multiple configurations for different grid types. Currently not in use.


## Assumptions:
1. The grid is made of square cells.
2. Each building occupies one or more grid cells.
   1. No half occupation of grid cells, a building either fully occupies a cell or does not occupy it at all.
3. The grid origin (0,0) is at the bottom-left corner and starts at (0,0,0) position on world position.
4. The grid cells are indexed starting from (0,0) at the bottom-left corner.
5. Buildings can be placed only within the bounds of the grid.
6. Buildings cannot overlap with each other on the grid.
7. The grid size (number of rows and columns) is predefined and fixed in the scene.
   1. No dynamic resizing of the grid during runtime.
8. The building placement is done in a 2D plane (X, Z) with Y being the height.
9. Buildings can only be rotated in 90-degree increments along the Y-axis.
   1. If we want to rotate buildings with any value other than 90 degree increments, we may face errors in alignment with the grid cells.
10. I am using a Grid with Cell size of 5x5 units for Confirming no bias is left due to 1x1 or 2x2 which are common approaches.
11. The Grid is placed at world origin (0,0,0) and extends positively in the X and Z directions.
12. The collider is added to Layer "Grid"(6 in layerMask) for raycasting purposes.
13. All objects are aligned to world axes, no rotation happens except for buildings in Y axis. 
    1. scripts like Wrapper assume this, if we want to rotate the grid or collider, we need to adjust the scripts accordingly.


## How to Use the Grid Building System
#### Creating a Grid:
1. We have 2 ways to create a grid, either by using a plane at a certain height and calculating data based on this height or by creating a collider and using raycasting to get the grid position.
2. The Grid we make is visible in editor using Gizmo but not in the game build, so we can use a plane or a collider for raycasting.
3. Define the grid cell size in the `GridBuildingConfig`.cs.
4. Create a GameObject in the scene and add `BuildingGrid`.cs to it, this will be our main grid manager and we define the width and height of the grid in it.
5. If we are using a collider, the collider needs a layer that is defined in the `GridBuildingConfig`.cs for raycasting purposes.

#### Creating a Building:
1. We have a wd model which we will use as our building, lets call it "building art".
2. We need a prefab for the building, create an empty GameObject and add `BuildingModel`.cs to it, we will refer to this as the "Building Modal".
   1. The position of "Building Modal" should be at a intersection point of the grid. example for a grid of cell size 3, points like (0,0,0), (3,0,0), (0,0,3), (3,0,3) etc.
   2. The "Building Modal" should have a Wrapper GameObject as a child to hold the building art mesh.
      1. This Wrapper GameObject should be positioned in a way that allows easy rotation and alignment with the grid, for some buildings it might be at center of the building area and for others it might be an edge of the grid cell based on the building art.
         1. It can be placed in center of cell, but not necessary, as long as the building art continues to align with the grid cells occupied by the building shape units.
         2. We can also populate the wrapper script variables to help with position changes with specific rotation if needed.
      2. We rotate the wrapper to align the building art correctly with the grid when user chooses to rotate the building.
      3. Try to keep it so that after all possible rotations, the building shape units still align with the center of grid cells.
   3. Add the "building art" as a child of the Wrapper GameObject, so it is visually represented in the scene.
   4. Add multiple `BuildingShapeUnit`.cs components as children of the Wrapper to define the shape of the building on the grid.
      1. Each `BuildingShapeUnit` should be positioned at the center of the grid cell it occupies. So the art expects to know the grid cell size.
3. Use `BuildingData`.cs scriptable objects to store information about the building, including a reference to the modal prefab.
4. We do not place the "Building Modal" but rather we instantiate it for preview amd placement is managed by someone else.
   1. When we want to place a building, we will instantiate the "Building Modal" prefab for preview and when we finalize a place, we will instantiate the "Building" with the model as a child of it representing the placed building.


#### Placing a Building on the Grid:
1. To place a building, we need 2 empty gameobject prefabs each with a script:
   1. `BuildingPreview`.cs - for previewing the building placement.
   2. `Building`.cs - for the actual placed building on the grid.
2. We Define a gameobject in the scene which has our main grid(`BuildingGrid`.cs) and add a script to handle building placement logic called `BuildingSystem`.cs.
   1. This script needs reference to the `BuildingGrid`.cs instance in the scene, the camera we are using for raycasting, and the `BuildingPreview`.cs and `Building`.cs prefabs.
3. When we click on the button corresponding to a building in the keyboard(hardcoded for now), we instantiate the `BuildingPreview`.cs prefab and set it up with the building data (prefab, shape units, etc.).
4. Use `BuildingPreview`.cs to visualize the building placement on the grid.
   1. The preview should show the building at the mouse position, snapping to the nearest grid cell.
   2. The preview should change color to indicate whether the placement is valid (e.g., green for valid, red for invalid).
5. When the player confirms the placement, use `BuildingGrid`.cs to check if the building can be placed at the desired grid position.
6. If the placement is valid, instantiate the building prefab at the grid position and update the grid cells to mark them as occupied (`BuildingGrid.SetBuilding()`).
7. The new Building is placed on the grid and cannot overlap with existing buildings. 
   1. The new building will be a child of `Building`.cs script prefab.

## Shaders
we are using "Shader Graph"(`com.unity.shadergraph`) to create shaders for grid using the sample "Procedural Patterns" from Unity which has "Grid" Node.
Assuming you have a fixed size grid, we now design a shader graph to let us make the grid visible to the user using a plane/quad in 3d objects.
**Create** -> **Shader Graph** -> **URP** -> **Unlit Shader Graph** -> name it ["GridLineShader"](Assets/Grid System/Art/Shaders/GridLineShader.shadergraph):
- Add "Grid" Node - This allows us to define the Grid size, the number of lines in x and y(tiling) and offset(if needed)
- output of "Grid" goes to "One Minus" node, allowing us to get all lines without any cell data.
- **Graph inspector** -> **Graph Setting** -> **Surface type** -> *Transparent* -> this allows us to set the mode to transparent
- Take the output of the "One Minus" node and attach it to the input("Base Color" and "Alpha(1)") for our "Fragment" Node.
    - This makes the shader output the grid and alpha ensures we ignore all black areas.
- For better understanding, in "Main Preview" Change to "Quad" to see the grid on a plane surface.
- Now we want some exposed values(visible in inspector) which will allow us to change the shader data.
    - Vector2 - `CellsCoveredInUnitScale` - (2, 2)
        - How many cells do we cover when we use a 3d "Plane" without any changes (2x2) when the cells are of 5 units of unity scale
        - How many cells do we cover when we use a 3d "Quad" without any changes (0.2x0.2) when the cells are of 5 units of unity scale
    - Color - `GridColor` - per material - White color with max alpha
    - Vector2 - `NumberOfCellsInEachGrid` - (1, 1)
        - How many cells we want in a cell - *This feels redundant if your cell size is bigger then one unit of unity space.*
    - float - `Thickness` - Mode is Slider(to avoid any random values) - min: 0.01, max:0.1, default:0.1
- Now that we have 4 exposed variables to set our data, we also need the scale of the object to identify the range (size of grid which determines the cell count in the shader), so we add another node called "Object" which helps us get the scale of the object.
    - We are not using "Transform" node as it is used to change coordinate space and get a vector 3 in a new coordinate space and not provide us with the actual transform component in a game object.
- using "Split", get the x and z scale and using "Vector 2" node, make it into a vector 2 number which can then be multiplied("Multiply") by our 'CellsCoveredInUnitScale' to find the number of cells our shader needs to show.
    - So 'CellsCoveredInUnitScale' tells us the number of cells we are covering in local scale of (1,1,1), multiplying this number to the actual scale of the object will tell us how many cells we can show on the plane/quad.
- In most cases, earlier part would have sufficed, but Let's say we want to show each grid as a set of AxB grid, or we want multiple grids to make a single grid cell, to achieve this, we defined a 'NumberOfCellsInEachGrid' variable. For this we "multiply" the output of previous node with our 'NumberOfCellsInEachGrid' blackboard variable and then we can pass this graph's output to *tiling* input channel in the "Grid" Node
    - This ensures that if we want to visually divide the cells to show more finer(smaller) grid, we can keep a value greater than one, and cells appear smaller than our calculated grid, similarly for less than 1 value, the cells may be bigger than the cells we calculated.
- Now, we want to set the thickness and color:
    - so for thickness, thickness of line is opposite of the cell size, so in "Grid" node, we need the difference between 1 and thickness to get the cell thickness which equates to line thickness. To do this, we use "One Minus" node with 'Thickness' and push this to "Grid" node's *Size* channel.
    - For Color, want the color to be the 'GridColor' which is then passed to "Multiply" node along with the output of the "Grid"->"One Minus"'s output channel, this gives the same output but colored in 'GridColor' which can then be passed to "Fragment" node's *Base Color* channel.

Following this, I have added feature to allow us to highlight specific cells on the grid using another shader graph called "GridHightableShader"(Assets/Grid System/Art/Shaders/GridHightableShader.shadergraph). 
Now the system can be considered complete for basic grid building functionality.


