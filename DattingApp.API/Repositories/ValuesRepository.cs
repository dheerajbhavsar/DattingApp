using DattingApp.API.DbContexts;
using DattingApp.API.Models;
using DattingApp.API.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace DattingApp.API.Data;

class ValuesRepository : IValuesRepository
{
    private readonly DataContext _dataContext;

    public ValuesRepository(DataContext dataContext) 
        => _dataContext = dataContext;

    public async Task<bool> AddAsync(Value value)
    {
        var valueFromDatabase = await GetValueByIdAsync(value.Id);
        if (valueFromDatabase is null)
        {
            await _dataContext.AddAsync(value);
            _dataContext.SaveChanges();
            return true;
        }
        return false;
    }

    public async Task<IEnumerable<Value>> GetAllValuesAsync() 
        => await (_dataContext?.Values)?.ToListAsync();

    public async Task<Value> GetValueByIdAsync(int id) 
        => await _dataContext?.Values?
            .FirstOrDefaultAsync(x => x.Id == id);
}