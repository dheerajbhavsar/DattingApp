using DattingApp.API.Models;

namespace DattingApp.API.Repositories.Contracts;
public interface IValuesRepository
{
    Task<IEnumerable<Value>> GetAllValuesAsync();
    Task<Value> GetValueByIdAsync(int id);
    Task<bool> AddAsync(Value value);
}