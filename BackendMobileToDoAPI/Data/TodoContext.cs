using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BackendMobileToDoAPI.Models;

    public class TodoContext : DbContext
    {
        public TodoContext (DbContextOptions<TodoContext> options)
            : base(options)
        {
        }

        public DbSet<BackendMobileToDoAPI.Models.TodoItem> TodoItem { get; set; }
    }
