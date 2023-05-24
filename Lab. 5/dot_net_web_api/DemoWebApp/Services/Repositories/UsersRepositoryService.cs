using DemoWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DemoWebApp.Services.Repositories
{
    public class ProjectsRepositoryService : IRepository<User>
    {
        private readonly ApplicationDbContext context;

        public ProjectsRepositoryService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public Project Create(User entity)
        {
            var entityEntry = context.Projects.Add(entity);
            context.SaveChanges();

            return entityEntry.Entity;
        }

        public void Delete(int id)
        {
            var entity = context.Projects.FirstOrDefault(project => project.Id == id);
            context.Remove(entity);
            context.SaveChanges();
        }

        public List<Project> Read()
        {
            return context.Projects.ToList();
        }

        public Project Read(int id)
        {
            return context.Projects.FirstOrDefault(project => project.Id == id);
        }

        public void Update(Project entity)
        {
            context.Projects.Update(entity);
            context.SaveChanges();
        }
    }
}