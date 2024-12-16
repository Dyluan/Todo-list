using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.Json;

//création builder
var builder = WebApplication.CreateBuilder(args);

//rajouter l'interface dans le builder avant de build
builder.Services.AddSingleton<ITaskService>(new InMeMoryTaskService());

//permet à mon front react de s'y connecter
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => {
        policy.WithOrigins("http://localhost:3000")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

//on build après avoir rajouté ttes les dépendances
var app = builder.Build();

//activation de cors
app.UseCors();

//création de la liste contenant les futurs objets Todos
var todos = new List<Todo>();

//middleware qui log dans la console
app.Use(async (context, next) => {
    Console.WriteLine($"[{context.Request.Method} {context.Request.Path} {DateTime.UtcNow}] Started.");
    //toujours await next(context)
    await next(context);
    Console.WriteLine($"[{context.Request.Method} {context.Request.Path} {DateTime.UtcNow}] Finished.");
});

//get basique
app.MapGet("/todos", (ITaskService service) => service.GetTodos());

app.MapGet("/todos/{id}", Results<Ok<Todo>, NotFound> (string id, ITaskService service) => 
{
    //on appelle le service pour retrouver le Todo par id
    var targetTodo = service.GetTodoById(id);
    return targetTodo is null
        ? TypedResults.NotFound()
        : TypedResults.Ok(targetTodo);
});

//post basique
app.MapPost("/todos", (Todo task, ITaskService service) => {
    //on appelle le service
    service.AddTodo(task);
    return TypedResults.Created("/todos/{id}", task);
})
.AddEndpointFilter(async (context, next) => {
    //récupération de la requête
    var taskArgument = context.GetArgument<Todo>(0);

    //création dictionnaire contenant les possibles erreurs
    var errors = new Dictionary<string, string[]>();

    //permet de récupérer le service et donc la liste des _todos
    var service = context.HttpContext.RequestServices.GetRequiredService<ITaskService>();

    if (service.GetTodoById(taskArgument.Id) != null) {
        //si l'id existe déjà dans la liste, on rajoute le nom de l'erreur + un message au dictionnaire errors
        errors.Add(nameof(Todo.Id), ["Id must be unique"]);
    }
    if (errors.Count > 0) {
        //si le nbre d'erreurs du dic >0 
        return Results.ValidationProblem(errors);
    }
    //IMPORTANT retourner await next(context)
    return await next(context);
});

app.MapPut("/todos/{id}", (string id, Todo task, ITaskService service) => {

    service.ModifyTodo(id, task);
    return TypedResults.Ok();
});

//delete basique
app.MapDelete("/todos/{id}", (string id, ITaskService service) => 
{
    //on appelle le service
    service.DeleteTodoById(id);
    return TypedResults.NoContent();
});

//lance le service
app.Run();

//création d'un "objet" appelé record qui type les todos
public record Todo(string Id, string Name, bool IsCompleted);

//création interface facilitant la logique des get/post/..
interface ITaskService {
    Todo? GetTodoById(string id);

    List<Todo> GetTodos();

    void DeleteTodoById(string id);

    Todo AddTodo(Todo task);

    Todo ModifyTodo(string id, Todo task);
}

//création d'une classe héritant de l'interface + création de la logique 
class InMeMoryTaskService : ITaskService {
    private readonly List<Todo> _todos = [];
    private readonly string _filePath = "todos.json";

    public InMeMoryTaskService() {
        LoadTodosFromFile();
    }

    public Todo AddTodo(Todo task) {
        _todos.Add(task);
        SaveTodosToFile();
        return task;
    }

    public void DeleteTodoById(string id) {
        _todos.RemoveAll(t=> t.Id == id);
        SaveTodosToFile();
    }

    public Todo? GetTodoById(string id) {
        return _todos.SingleOrDefault(t=> id == t.Id);
    }

    public List<Todo> GetTodos() {
        return _todos;
    }

    public Todo ModifyTodo(string id, Todo task) {
        var targetTodo = _todos.FirstOrDefault(t => t.Id == id);

        if (targetTodo == null) {
            throw new ArgumentException($"No Todo found with Id {id}");
        }

        var updatedTodo = targetTodo with {IsCompleted = task.IsCompleted};

        int index = _todos.IndexOf(targetTodo);
        if (index >= 0) {
            _todos[index] = updatedTodo;
        } else {
            throw new InvalidOperationException($"Todo with Id {id} could not be updated");
        }

        SaveTodosToFile();
        return updatedTodo;
    }

    private void LoadTodosFromFile() {
        if (File.Exists(_filePath)) {
            var jsonData = File.ReadAllText(_filePath);
            var loadedTodos = JsonSerializer.Deserialize<List<Todo>>(jsonData);
            if (loadedTodos != null) {
                _todos.AddRange(loadedTodos);
            }
        }
    }

    private void SaveTodosToFile() {
        var jsonData = JsonSerializer.Serialize(_todos, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, jsonData);
    }
}