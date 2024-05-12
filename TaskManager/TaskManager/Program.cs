using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public enum TaskStatus
{
    Incomplete,
    Completed
}

public class Task
{
    public string Description { get; }
    public TaskStatus Status { get; private set; }

    public Task(string description)
    {
        Description = description;
        Status = TaskStatus.Incomplete;
    }

    public void MarkAsComplete()
    {
        Status = TaskStatus.Completed;
    }

    public override string ToString()
    {
        string status = Status == TaskStatus.Completed ? "[x]" : "[ ]";
        return $"{status} {Description}";
    }
}

public class TodoList
{
    private List<Task> tasks;
    private readonly object lockObject = new object();

    public TodoList()
    {
        tasks = new List<Task>();
        LoadTasksFromFile("tasks.txt");
    }

    public void AddTask(string description)
    {
        if (!IsValidTaskDescription(description))
        {
            Console.WriteLine("Invalid task description. Please enter a valid description.");
            return;
        }

        lock (lockObject)
        {
            Task task = new Task(description);
            tasks.Add(task);
            SaveTasksToFile("tasks.txt");
        }
    }

    public void MarkTaskComplete(int taskIndex)
    {
        lock (lockObject)
        {
            if (taskIndex >= 0 && taskIndex < tasks.Count)
            {
                tasks[taskIndex].MarkAsComplete();
                SaveTasksToFile("tasks.txt");
            }
            else
            {
                Console.WriteLine("Invalid task index");
            }
        }
    }

    public void ViewTasks()
    {
        lock (lockObject)
        {
            if (tasks.Count == 0)
            {
                Console.WriteLine("No tasks in the list");
                return;
            }
            for (int i = 0; i < tasks.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {tasks[i]}");
            }
        }
    }

    public void DeleteTask(int taskIndex)
    {
        lock (lockObject)
        {
            if (taskIndex >= 0 && taskIndex < tasks.Count)
            {
                tasks.RemoveAt(taskIndex);
                SaveTasksToFile("tasks.txt");
                Console.WriteLine("Task deleted successfully");
            }
            else
            {
                Console.WriteLine("Invalid task index");
            }
        }
    }

    public void DeleteCompletedTasks()
    {
        lock (lockObject)
        {
            tasks.RemoveAll(task => task.Status == TaskStatus.Completed);
            SaveTasksToFile("tasks.txt");
            Console.WriteLine("Completed tasks deleted successfully");
        }
    }

    public void ClearAllTasks()
    {
        lock (lockObject)
        {
            tasks.Clear();
            SaveTasksToFile("tasks.txt");
            Console.WriteLine("All tasks cleared");
        }
    }

    private bool IsValidTaskDescription(string description)
    {
        string pattern = @"^[a-zA-Z0-9\s\-.,!?]+$";
        return Regex.IsMatch(description, pattern);
    }

    private void SaveTasksToFile(string filePath)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (Task task in tasks)
            {
                writer.WriteLine($"{task.Description},{task.Status}");
            }
        }
    }

    private void LoadTasksFromFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            tasks.Clear();
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(',');
                    string description = parts[0];
                    TaskStatus status = (TaskStatus)Enum.Parse(typeof(TaskStatus), parts[1]);
                    Task task = new Task(description);
                    if (status == TaskStatus.Completed)
                    {
                        task.MarkAsComplete();
                    }
                    tasks.Add(task);
                }
            }
        }
    }
}

public class Program
{
    public static void PrintHeader()
    {
        string header = @"
 __          __  _                                      
 \ \        / / | |                                      
  \ \  /\  / /__| | ___ ___  _ __ ___   ___  
   \ \/  \/ / _ \ |/ __/ _ \| '_ ` _ \ / _ \ 
    \  /\  /  __/ | (_| (_) | | | | | |  __/ 
     \/  \/ \___|_|\___\___/|_| |_| |_|\___|  

         Welcome to the To-Do List App                  
";
        Console.WriteLine(header);
    }

    public static void Main(string[] args)
    {
        PrintHeader();
        TodoList todoList = new TodoList();

        while (true)
        {
            Console.WriteLine("\nMain Menu");
            Console.WriteLine("1. Add Task");
            Console.WriteLine("2. Mark Task as Complete");
            Console.WriteLine("3. View Tasks");
            Console.WriteLine("4. Delete Task");
            Console.WriteLine("5. Delete Completed Tasks");
            Console.WriteLine("6. Clear All Tasks");
            Console.WriteLine("7. Exit");

            Console.Write("Enter your choice: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Enter task description: ");
                    string description = Console.ReadLine();
                    todoList.AddTask(description);
                    break;

                case "2":
                    todoList.ViewTasks();
                    Console.Write("Enter task index to mark as complete: ");
                    if (int.TryParse(Console.ReadLine(), out int taskIndex))
                    {
                        todoList.MarkTaskComplete(taskIndex - 1);
                    }
                    else
                    {
                        Console.WriteLine("Invalid input");
                    }
                    break;

                case "3":
                    todoList.ViewTasks();
                    break;

                case "4":
                    todoList.ViewTasks();
                    Console.Write("Enter task index to delete: ");
                    if (int.TryParse(Console.ReadLine(), out int deleteIndex))
                    {
                        todoList.DeleteTask(deleteIndex - 1);
                    }
                    else
                    {
                        Console.WriteLine("Invalid input");
                    }
                    break;

                case "5":
                    todoList.DeleteCompletedTasks();
                    break;

                case "6":
                    Console.Write("Are you sure you want to clear all tasks? (y/n): ");
                    string confirm = Console.ReadLine().ToLower();
                    if (confirm == "y")
                    {
                        todoList.ClearAllTasks();
                    }
                    break;

                case "7":
                    Console.WriteLine("Exiting the To-Do List App...");
                    return;

                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }
}
