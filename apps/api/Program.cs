using BangaloreTaxi.Api;
using BangaloreTaxi.Api.Hosting;

var builder = WebApplication.CreateBuilder(args);
builder.AddApiFoundation();

var app = builder.Build();
app.UseApiFoundation();
app.Run();

public partial class Program;
