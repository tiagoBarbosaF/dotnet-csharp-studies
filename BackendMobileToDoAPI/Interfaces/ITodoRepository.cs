using System.Collections.Generic;

using BackendMobileToDoAPI.Models;

namespace BackendMobileToDoAPI.Interfaces
{
    public interface ITodoRepository
    {
        bool DoesItemExists(string id);
        IEnumerable<TodoItem> All { get; }
        TodoItem Find(string id);
        void Insert(TodoItem item);
        void Update(TodoItem item);
        void Delete(string id);
    }
}
