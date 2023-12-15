using UnityEngine;
using System.Collections.Generic;
using System.Data;
public class SpriteController : MonoBehaviour
{
    public GameObject path; 
    public GameObject wall;
    //public GameObject brige;
    public GameObject walk;
    public GameObject exit;
    public GameObject start;
    public int rows = 17;
    public int cols = 17;
    private List<GameObject> instantiatedObjects = new List<GameObject>();
    private int[,] maze;

    void Start()
    {
        GenerateMaze();
        //PrintMaze();
    }

    public void GenerateMaze()
    {

        maze = new int[rows, cols];
        //populate maze
        for (int i = 0; i < rows; i++){
            for (int j = 0; j < cols; j++){
                if(i%2!=0 && j%2!=0)
                    maze[i, j] = 1;
                else
                    maze[i, j] = 0;
            }
        }
        
        //make random map
        /*for(int i=1;i<rows-1;i++){
            for(int j =1;j<cols-1;j++){
                int x = Random.Range(0, 4);
                maze[i,j]=x;
            }
        }*/
        //make connected map
        direction(2, 4, 4);
        direction(4, 8, 2);
        direction(8, 16,1);
        
        int startX = 0;
        int startY = 0;
        bool startCheck = true;
        while(startCheck){
            startX = Random.Range(1, cols-1);
            startY = Random.Range(1, cols-1);
            if(maze[startX,startY]!=0){
                maze[startX,startY]=999;
                startCheck = false;
            }
            
        }
        int exitX =0;
        int exitY =0;
        bool exitCheck= true;
        while(exitCheck){
            exitX = Random.Range(1, cols-1);
            exitY = Random.Range(1, cols-1);
            if(maze[exitX,exitY]!= 0 && maze[exitX,exitY]!=maze[startX,startY]){   
                maze[exitX,exitY]=111;
                exitCheck = false;
            }
        }
         // Perform BFS to find a valid path from start to exit
        bool pathFound = BFS(new Vector2Int(startX, startY), new Vector2Int(exitX, exitY));
        //Debug.Log("startX, startY " + startX+" "+ startY);
        //Debug.Log("exitX, exitY " + exitX+" "+ exitY);
        //Debug.Log("Path Found: " + pathFound);
        // Print the maze only if a valid path is found
        if (pathFound)
        {
            maze[exitX,exitY]=111;
            PrintMaze();
        }
 
    }
    void direction(int center, int range, int time){
        int direction1;
        int direction2;
        int direction3;
        direction1 = Random.Range(0,4);
        makeMaze(center,range,direction1, time);
        while(true){
            direction2 = Random.Range(0,4);
            if(direction2 != direction1){
                makeMaze(center,range,direction2, time);
                break;
            }
        }
        while(true){
            direction3 = Random.Range(0,4);
            if(direction3 != direction1 &&direction3 != direction2){
                makeMaze(center,range,direction3, time);
                break;
            }
        }
    }
    void makeMaze(int center, int range, int direction, int time){
        int location;
        if(direction == 0){
            while(true){
                location = Random.Range(1,center+1);
                if(location!=center && maze[center-1,location]!=0 && maze[center+1,location]!=0)
                    break;
            }
            
            maze[center,location]=1;
            if((range+range)<rows){
                for(int i = 0;i<time;i++){
                    for(int j =0;j<time;j++){
                        maze[center+range*j,location+range*i]=1;
                    }
                }
            }
            
            
                
        } 
        else if(direction == 1){
            while(true){
                location = Random.Range(1,center+1);
                if(location!=center && maze[location,center-1]!=0 && maze[location,center+1]!=0)
                    break;
            }

            maze[location,center]=1;
            if((range+range)<rows){
                for(int i = 0;i<time;i++){
                    for(int j =0;j<time;j++){
                        maze[location+range*j,center+range*i]=1;

                    }
                }
            }
        }
        else if(direction == 2){
            while(true){
                location = Random.Range(center,range);
                if(location!=center && maze[center-1,location]!=0 && maze[center+1,location]!=0)
                    break;
            }

            maze[center,location]=1;
            if((range+range)<rows){
                for(int i = 0;i<time;i++){
                    for(int j =0;j<time;j++){
                        maze[center+range*j,location+range*i]=1;

                    }
                }
            }
        }
        else if(direction == 3){
            while(true){
                location = Random.Range(center,range);
                if(location!=center && maze[location,center-1]!=0 && maze[location,center+1]!=0)
                    break;
            }

            maze[location,center]=1;
            if((range+range)<rows){
                for(int i = 0;i<time;i++){
                    for(int j =0;j<time;j++){
                        maze[location+range*j,center+range*i]=1;

                    }
                }
            }
        }
    }
    bool BFS(Vector2Int start, Vector2Int target)
    {
        bool[,] visited = new bool[rows, cols];
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> parentMap = new Dictionary<Vector2Int, Vector2Int>();

        queue.Enqueue(start);
        visited[start.x, start.y] = true;

        int[] rowMoves = { -1, 1, 0, 0 };
        int[] colMoves = { 0, 0, -1, 1 };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            if (current==target)
            {
                Debug.Log("Path Found: ");
                // Reconstruct the path from the target to the start
                ReconstructPath(parentMap, start, target);
                return true;
            }
            for (int i = 0; i < 4; i++)
            {
                int newRow = current.x + rowMoves[i];
                int newCol = current.y + colMoves[i];

                if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < cols && !visited[newRow, newCol] && (maze[newRow, newCol] == 1 || maze[newRow, newCol] == 111))
                {

                    Vector2Int neighbor = new Vector2Int(newRow, newCol);
                    queue.Enqueue(neighbor);
                    visited[newRow, newCol] = true;
                    parentMap[neighbor] = current; // Keep track of the parent for reconstructing the path
                    
                }
            }
            
        }
        
        // If the loop completes without finding a path, return false
        return false;
    }
    void ReconstructPath(Dictionary<Vector2Int, Vector2Int> parentMap, Vector2Int start, Vector2Int target)
    {
        Vector2Int current = target;
        while (current != start)
        {
            maze[current.x, current.y] = 222; // Mark the path in the maze
            current = parentMap[current];
        }
    }

    void PrintMaze()
    {
        ClearInstantiatedObjects();
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {

                if(maze[i,j] == 1){
                    GameObject pathObject = Instantiate(path);
                    pathObject.transform.position = new Vector2(i*0.40f-3f, j*0.40f-2.4f);
                    pathObject.transform.localScale = new Vector2(0.1455f,0.1455f);
                     // Add the instantiated object to the list
                    instantiatedObjects.Add(pathObject);

                }
                else if(maze[i,j] == 0){
                    GameObject wallObject = Instantiate(wall);
                    wallObject.transform.position = new Vector2(i*0.40f-3f, j*0.40f-2.4f);
                    wallObject.transform.localScale = new Vector2(0.1455f,0.1455f);
                     // Add the instantiated object to the list
                    instantiatedObjects.Add(wallObject);

                }

                else if(maze[i,j] == 222){
                    GameObject walkObject = Instantiate(walk);
                    walkObject.transform.position = new Vector2(i*0.40f-3f, j*0.40f-2.4f);
                    walkObject.transform.localScale = new Vector2(0.1455f,0.1455f);
                     // Add the instantiated object to the list
                    instantiatedObjects.Add(walkObject);

                }
                else if(maze[i,j] == 999){
                    GameObject startObject = Instantiate(start);
                    startObject.transform.position = new Vector2(i*0.40f-3f, j*0.40f-2.4f);
                    startObject.transform.localScale = new Vector2(0.1455f,0.1455f);
                    // Add the instantiated object to the list
                    instantiatedObjects.Add(startObject);
                }
                
                else if(maze[i,j] == 111){
                    GameObject exitObject = Instantiate(exit);
                    exitObject.transform.position = new Vector2(i*0.40f-3f, j*0.40f-2.4f);
                    exitObject.transform.localScale = new Vector2(0.1455f,0.1455f);
                    // Add the instantiated object to the list
                    instantiatedObjects.Add(exitObject);
                }
            }
        }         
    }
    void ClearInstantiatedObjects()
    {
        // Destroy previously instantiated objects
        foreach (GameObject obj in instantiatedObjects)
        {
            Destroy(obj);
        }

        // Clear the list
        instantiatedObjects.Clear();
    }

}
