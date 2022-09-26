using DattingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DattingApp.API.DbContexts;
public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) :
     base(options)
    {
    }

    public DbSet<Value>? Values { get; set; }
}