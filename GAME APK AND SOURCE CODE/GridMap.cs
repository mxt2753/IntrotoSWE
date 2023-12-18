using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.IO;
using System.Linq;
using CodeMonkey.Utils;

public class GridMap<TGridObject>
{
	public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
	public class OnGridObjectChangedEventArgs : EventArgs
	{
		public int x;
		public int y;
	}

	[SerializeField] GameObject textObject;
	private int width;
	private int height;
	private float cellSize;

	private TGridObject[,] gridArray;
	private Vector3 originPosition;
	private TextMesh[,] debugTextArray;

	private static List<List<List<string>>> parsedFile = null;
	private static TGridObject[,] savedGrid;
	private static GridMap<TGridObject> savedGridMap;
	private static bool endUnlocked = false;

	public GridMap(float cellSize, Vector3 originPosition, Func<GridMap<TGridObject>, int, int, List<GridObject>, Vector3, TGridObject> createGridObject)
	{	
		List<List<List<GridObject>>> objectElements;

    this.cellSize = cellSize;
    this.originPosition = originPosition;

    if(parsedFile != null)
    {
  		ParseLevel(out this.width, out this.height, out objectElements);
    }
    else
    {
    	ParseLevel(out this.width, out this.height, out objectElements, levels.GetLevelName());
    }
		gridArray = new TGridObject[height, width];

		// Debug.Log("width: " + width + " height: " + height);
		// Debug.Log("X: " + gridArray.GetLength(0) + " Y: " + gridArray.GetLength(1));
		// Debug.Log("obj elem Y: " + objectElements.Count + " X: " + objectElements.ElementAt(0).Count);
		for(int row = 0; row < gridArray.GetLength(0); row++)
		{
			for(int col = 0; col < gridArray.GetLength(1); col++)
			{
				gridArray[row, col] = createGridObject(this, col, row, objectElements.ElementAt(row).ElementAt(col), new Vector3(col, row, 0) * cellSize + Vector3.one * cellSize * .5f);
			}
		}

		bool showDebug = false;
		if(showDebug)
		{
			debugTextArray = new TextMesh[width, height];

			for(int x = 0; x < gridArray.GetLength(0); x++)
			{
				for(int y = 0; y < gridArray.GetLength(1); y++)
				{
					debugTextArray[x,y] = UtilsClass.CreateWorldText(gridArray[x, y]?.ToString(), null, GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f, 20, Color.white, TextAnchor.MiddleCenter);
					Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
					Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
				}
			}
			Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
			Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);

			OnGridObjectChanged += (object sender, OnGridObjectChangedEventArgs eventArgs) =>
			{
				debugTextArray[eventArgs.x, eventArgs.y].text = gridArray[eventArgs.x, eventArgs.y]?.ToString();
			};
		}
	}

	public static void SetUnlockedEnd(bool newValue)
	{
		endUnlocked = newValue;
	}

	public static bool GetUnlockedEnd()
	{
		return endUnlocked;
	}

	public void SetUp()
	{
		// for(int row = 0; row < gridArray.GetLength(0); row++)
		// {
		// 	for(int col = 0; col < gridArray.GetLength(1); col++)
		// 	{
		// 		TGridObject curSpace = gridArray[row, col];
		// 		List<GridObject> objects = new List<GridObject>();
		// 		foreach(string objName in (curSpace as GridSpace).GetObjectNames())
		// 		{
    // 			Vector3 newPosition = GetWorldPosition(col, row) + new Vector3(cellSize, cellSize) *.5f;
    // 			switch(objName)
    // 			{
    // 				case "Player":
    // 					objects.Add(new Player(newPosition, cellSize, col, row, "Player"));
    // 					// Debug.Log("PLAYER X: " + y + " PLAYER Y: " + tempCounter);
    // 					break;
    // 				case "Floor":
    // 					objects.Add(new Floor(newPosition, cellSize, col, row, "Floor"));
    // 					break;
    // 				case "Wall":
    // 					objects.Add(new Wall(newPosition, cellSize, col, row, "Wall"));
    // 					break;
    // 				case "EnemyVN":
  	// 					objects.Add(new Enemy(newPosition, cellSize, col, row, 15, 3, 'v', 'n', "EnemyVN"));
    // 					break;
    // 				case "EnemyVS":
  	// 					objects.Add(new Enemy(newPosition, cellSize, col, row, 15, 3, 'v', 's', "EnemyVS"));
    // 					break;
    // 				case "EnemyHE":
  	// 					objects.Add(new Enemy(newPosition, cellSize, col, row, 15, 3, 'h', 'e', "EnemyHE"));
    // 					break;
    // 				case "EnemyHW":
  	// 					objects.Add(new Enemy(newPosition, cellSize, col, row, 15, 3, 'h', 'w', "EnemyHW"));
    // 					break; 
    // 				case "GoalItem":
    // 					objects.Add(new GoalItem(newPosition, cellSize, col, row, "GoalItem", "GoalItem"));
    // 					break;
    // 				case "LevelExit":
    // 					objects.Add(new LevelExit(newPosition, cellSize, col, row, "LevelExit"));
    // 					break;
    // 				case "HealthPotion":
    // 					objects.Add(new HealthPotion(newPosition, cellSize, col, row, "HealthPotion", "HealthPotion"));
    // 					break;
    // 			}
		// 		}
		// 	}
		// }
	}

	public void SetSavedGridMap(GridMap<TGridObject> gridArray)
	{
		savedGridMap = gridArray;
	}

	public void SetSavedGrid(TGridObject[,] gridArray)
	{
		savedGrid = gridArray;
	}

	public static TGridObject[,] GetSavedGrid()
	{
		return savedGrid;
	}

	public static GridMap<TGridObject> GetSavedGridMap()
	{
		return savedGridMap;
	}

	public void SetParsedFile(List<List<List<string>>> newParsedFile)
	{
		parsedFile = newParsedFile;
	}

	public static List<List<List<string>>> GetParsedFile()
	{
		return parsedFile;
	}

	public void ParseLevel(out int width, out int height, out List<List<List<GridObject>>> objectElements)
	{
		objectElements = new List<List<List<GridObject>>>();

		int rowCount = 0;
		for(int row = 0; row < parsedFile.Count; row++)
		{
			List<List<GridObject>> lineObjects = new List<List<GridObject>>();
			objectElements.Add(lineObjects);
			for(int col = 0; col < parsedFile.ElementAt(0).Count; col++)
			{
				List<GridObject> objects = new List<GridObject>();
				lineObjects.Add(objects);

				List<string> cols = parsedFile.ElementAt(row).ElementAt(col);
				foreach(string splitElement in cols)
				{
    			Vector3 newPosition = GetWorldPosition(col, rowCount) + new Vector3(cellSize, cellSize) *.5f;
    			switch(splitElement)
    			{
    				case "Player":
    					objects.Add(new Player(newPosition, cellSize, col, rowCount, "Player"));
    					// Debug.Log("PLAYER X: " + y + " PLAYER Y: " + tempCounter);
    					break;
    				case "Floor":
    					objects.Add(new Floor(newPosition, cellSize, col, rowCount, "Floor"));
    					break;
    				case "Wall":
    					objects.Add(new Wall(newPosition, cellSize, col, rowCount, "Wall"));
    					break;
    				case "EnemyVN":
  						objects.Add(new Enemy(newPosition, cellSize, col, rowCount, 15, 3, 'v', 'n', "EnemyVN"));
    					break;
    				case "EnemyVS":
  						objects.Add(new Enemy(newPosition, cellSize, col, rowCount, 15, 3, 'v', 's', "EnemyVS"));
    					break;
    				case "EnemyHE":
  						objects.Add(new Enemy(newPosition, cellSize, col, rowCount, 15, 3, 'h', 'e', "EnemyHE"));
    					break;
    				case "EnemyHW":
  						objects.Add(new Enemy(newPosition, cellSize, col, rowCount, 15, 3, 'h', 'w', "EnemyHW"));
    					break; 
    				case "GoalItem":
    					objects.Add(new GoalItem(newPosition, cellSize, col, rowCount, "GoalItem", "GoalItem"));
    					break;
    				case "LevelExit":
    					LevelExit exit = new LevelExit(newPosition, cellSize, col, rowCount, "LevelExit");
    					objects.Add(exit);
    					if(endUnlocked)
    					{
    						exit.UnlockExit();
    					}
    					break;
    				case "HealthPotion":
    					objects.Add(new HealthPotion(newPosition, cellSize, col, rowCount, "HealthPotion", "HealthPotion"));
    					break;
    			}
				}
			}
			rowCount += 1;
		}
		width = parsedFile.ElementAt(0).Count;
		height = parsedFile.Count;
		parsedFile = null;
	}

	public void ParseLevel(out int width, out int height, out List<List<List<GridObject>>> objectElements, string levelName)
	{
		TextAsset textFile = Resources.Load<TextAsset>(levelName);
		List<string> fileLines = new List<string>(textFile.text.Split('\n'));

		// foreach(string cur_line in fileLines)
		// {
		// 	Debug.Log("LINE: " + cur_line);
		// }

    objectElements = new List<List<List<GridObject>>>();
    List<List<List<string>>> stringElements = new List<List<List<string>>>();

    int max_rows = 0;
    int max_cols = 0;
    int rows = 0;
    int cols = 0;
    foreach(string currentLine in fileLines)
    {
    	// List<List<GridObject>> lineObjects = new List<List<GridObject>>();
    	List<List<string>> stringObjects = new List<List<string>>();
    	stringElements.Add(stringObjects);
    	// objectElements.Add(lineObjects);
    	string[] gridSpaceObjects = currentLine.Split(',');

    	cols = 0;
    	foreach(string element in gridSpaceObjects)
    	{
    		List<string> strObj = new List<string>();
    		stringObjects.Add(strObj);
    		// List<GridObject> objects = new List<GridObject>();
    		// lineObjects.Add(objects);

    		string currentElement = string.Copy(element);
    		currentElement = currentElement.Remove(0, 1);
    		currentElement = currentElement.Remove(currentElement.Length - 1);

    		string[] splitElements = currentElement.Split('|');

    		foreach(string splitElement in splitElements)
    		{
    			strObj.Add(splitElement);
    			// Vector3 newPosition = GetWorldPosition(cols, rows) + new Vector3(cellSize, cellSize) *.5f;
    			// switch(splitElement)
    			// {
    			// 	case "Player":
    			// 		objects.Add(new Player(newPosition, cellSize, cols, rows, "Player"));
    			// 		break;
    			// 	case "Floor":
    			// 		objects.Add(new Floor(newPosition, cellSize, cols, rows, "Floor"));
    			// 		break;
    			// 	case "Wall":
    			// 		objects.Add(new Wall(newPosition, cellSize, cols, rows, "Wall"));
    			// 		break;
    			// 	case "EnemyVN":
  				// 		objects.Add(new Enemy(newPosition, cellSize, cols, rows, 15, 3, 'v', 'n', "EnemyVN"));
    			// 		break;
    			// 	case "EnemyVS":
  				// 		objects.Add(new Enemy(newPosition, cellSize, cols, rows, 15, 3, 'v', 's', "EnemyVS"));
    			// 		break;
    			// 	case "EnemyHE":
  				// 		objects.Add(new Enemy(newPosition, cellSize, cols, rows, 15, 3, 'h', 'e', "EnemyHE"));
    			// 		break;
    			// 	case "EnemyHW":
  				// 		objects.Add(new Enemy(newPosition, cellSize, cols, rows, 15, 3, 'h', 'w', "EnemyHW"));
    			// 		break; 
    			// 	case "GoalItem":
    			// 		objects.Add(new GoalItem(newPosition, cellSize, cols, rows,"GoalItem", "GoalItem"));
    			// 		break;
    			// 	case "LevelExit":
    			// 		objects.Add(new LevelExit(newPosition, cellSize, cols, rows, "LevelExit"));
    			// 		break;
    			// 	case "HealthPotion":
    			// 		objects.Add(new HealthPotion(newPosition, cellSize, cols, rows, "HealthPotion", "HealthPotion"));
    			// 		break;
    			// }
    		}
    		cols += 1;
    		if(cols > max_cols)
    		{
    			max_cols = cols;
    		}
    	}
    	rows += 1;
    }

		if(rows > max_rows)
		{
			max_rows = rows;
		}

    width = max_cols;
    height = max_rows;

		// Debug.Log("X: " + stringElements.Count + " Y: " + stringElements.ElementAt(0).Count);
		// Debug.Log("cols: " + cols + " rows: " + rows);

		objectElements = new List<List<List<GridObject>>>();
		int tempCounter = 0;
		for(int x = rows - 1; x >= 0; x--)
		{
			List<List<string>> currentRow = stringElements.ElementAt(x);
			List<List<GridObject>> lineObjects = new List<List<GridObject>>();
			objectElements.Add(lineObjects);
			int counter = 0;
			for(int y = 0; y < cols; y++)
			{
				List<GridObject> objects = new List<GridObject>();
				lineObjects.Add(objects);
				List<string> nextRow = currentRow.ElementAt(y);

				foreach(string splitElement in nextRow)
				{
    			Vector3 newPosition = GetWorldPosition(y, tempCounter) + new Vector3(cellSize, cellSize) *.5f;
    			switch(splitElement)
    			{
    				case "Player":
    					objects.Add(new Player(newPosition, cellSize, y, tempCounter, "Player"));
    					// Debug.Log("PLAYER X: " + y + " PLAYER Y: " + tempCounter);
    					break;
    				case "Floor":
    					objects.Add(new Floor(newPosition, cellSize, y, tempCounter, "Floor"));
    					break;
    				case "Wall":
    					objects.Add(new Wall(newPosition, cellSize, y, tempCounter, "Wall"));
    					break;
    				case "EnemyVN":
  						objects.Add(new Enemy(newPosition, cellSize, y, tempCounter, 15, 3, 'v', 'n', "EnemyVN"));
    					break;
    				case "EnemyVS":
  						objects.Add(new Enemy(newPosition, cellSize, y, tempCounter, 15, 3, 'v', 's', "EnemyVS"));
    					break;
    				case "EnemyHE":
  						objects.Add(new Enemy(newPosition, cellSize, y, tempCounter, 15, 3, 'h', 'e', "EnemyHE"));
    					break;
    				case "EnemyHW":
  						objects.Add(new Enemy(newPosition, cellSize, y, tempCounter, 15, 3, 'h', 'w', "EnemyHW"));
    					break; 
    				case "GoalItem":
    					objects.Add(new GoalItem(newPosition, cellSize, y, tempCounter, "GoalItem", "GoalItem"));
    					break;
    				case "LevelExit":
    					objects.Add(new LevelExit(newPosition, cellSize, y, tempCounter, "LevelExit"));
    					break;
    				case "HealthPotion":
    					objects.Add(new HealthPotion(newPosition, cellSize, y, tempCounter, "HealthPotion", "HealthPotion"));
    					break;
    			}
    			counter += 1;
				}
			}
			tempCounter += 1;
		}
	}

	public int GetWidth()
	{
		return width;
	}

	public int GetHeight()
	{
		return height;
	}

	public float GetCellSize()
	{
		return cellSize;
	}

	public Vector3 GetWorldPosition(int x, int y)
	{
		return new Vector3(x, y) * cellSize + originPosition;
	}

	public void GetXY(Vector3 worldPosition, out int x, out int y)
	{
		x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
		y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
	}

	public void SetGridObject(int x, int y, TGridObject value)
	{
		if(x >= 0 && y >= 0 && x < width && y < height)
		{
			gridArray[y, x] = value;
			debugTextArray[y, x].text = gridArray[y, x].ToString();
			if(OnGridObjectChanged != null) OnGridObjectChanged(this, new OnGridObjectChangedEventArgs {x = x, y = y});
		}
	}

	public void TriggerGridObjectChanged(int x, int y)
	{
		if(OnGridObjectChanged != null) OnGridObjectChanged(this, new OnGridObjectChangedEventArgs {x = x, y = y});
	}

	public void SetGridObject(Vector3 worldPosition, TGridObject value)
	{
		int x, y;
		GetXY(worldPosition, out x, out y);
		SetGridObject(x, y, value);
	}

	public TGridObject GetGridObject(int x, int y)
	{
		if(x >= 0 && y >= 0 && x < width && y < height)
		{
			return gridArray[y,x];
		}
		else
		{
			return default(TGridObject);
		}
	}

	public TGridObject GetGridObject(Vector3 worldPosition)
	{
		int x, y;
		GetXY(worldPosition, out x, out y);
		return GetGridObject(x, y);
	}

	public TGridObject[,] GetGridArray()
	{
		return gridArray;
	} 
}
