var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// In-memory storage for Notes
var notes = new List<Note>();
var nextId = 1;

// GET /api/notes
app.MapGet("/api/notes", () => notes);

// GET /api/notes/{id}
app.MapGet("/api/notes/{id}", (int id) =>
{
    var note = notes.FirstOrDefault(n => n.Id == id);
    return note is null ? Results.NotFound() : Results.Ok(note);
});

// POST /api/notes
app.MapPost("/api/notes", (CreateNoteRequest request) =>
{
    var note = new Note
    {
        Id = nextId++,
        Title = request.Title,
        Text = request.Text,
        CreatedAt = DateTime.UtcNow
    };
    notes.Add(note);
    return Results.Created($"/api/notes/{note.Id}", note);
});

// DELETE /api/notes/{id}
app.MapDelete("/api/notes/{id}", (int id) =>
{
    var note = notes.FirstOrDefault(n => n.Id == id);
    if (note is null) return Results.NotFound();
    notes.Remove(note);
    return Results.NoContent();
});

// Диагностические эндпоинты
app.MapGet("/health", () => new { status = "ok", time = DateTime.UtcNow });
app.MapGet("/version", () => new { appName = "IsLabApp", version = "0.1.0-lab4" });

app.Run();

// Models
record Note { public int Id { get; set; } public string Title { get; set; } = ""; public string Text { get; set; } = ""; public DateTime CreatedAt { get; set; } }
record CreateNoteRequest(string Title, string Text);