using BackendMobileToDoAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendMobileToDoAPI.Interfaces
{
    public class ITodoRepository
    {
        bool DoesItemExists( string id );
        IEnumerable<TodoItem> All { get; }
        TodoItem Find( string id );
        void Insert( TodoItem item );
        void Update( TodoItem item );
        void Delete( string id );
    }
}
