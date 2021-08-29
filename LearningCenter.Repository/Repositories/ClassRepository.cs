using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using LearningCenter.Repository.Interfaces;
using LearningCenter.Repository.Models;

namespace LearningCenter.Repository.Repositories
{
    public class ClassRepository : IClassRepository
    {
        private IMemoryCache memoryCache;

        public ClassRepository(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        public List<ClassModel> Classes
        {
            get
            {
                List<ClassModel> results = new List<ClassModel>();

                if (!memoryCache.TryGetValue("Classes", out results))
                {
                    results = DatabaseAccessor.Instance.Class.Select(s => new ClassModel
                    {
                        Id = s.ClassId,
                        Name = s.ClassName,
                        Description = s.ClassDescription,
                        Price = s.ClassPrice
                    }).ToList();

                    memoryCache.Set("Classes", results,
                        new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromSeconds(30)));
                }

                return (List<ClassModel>)memoryCache.Get("Classes");
            }
        }

        public ClassModel Add(string name, string description, decimal price)
        {
            var add_class = DatabaseAccessor.Instance.Class
                .Add(new Database.Models.Class
                {
                    ClassName = name,
                    ClassDescription = description,
                    ClassPrice = price
                });

            if (add_class == null)
                return null;

            DatabaseAccessor.Instance.SaveChanges();

            if (memoryCache.TryGetValue("Classes", out List<ClassModel> temp))
            {
                memoryCache.Remove("Classes");
            }

            return new ClassModel
            {
                Id = add_class.Entity.ClassId,
                Name = add_class.Entity.ClassName,
                Description = add_class.Entity.ClassDescription,
                Price = add_class.Entity.ClassPrice
            };
        }

        public ClassModel Class(int class_id)
        {
            return DatabaseAccessor.Instance.Class
                .Where(x => x.ClassId == class_id)
                .Select(s => new ClassModel
                {
                    Id = s.ClassId,
                    Name = s.ClassName,
                    Description = s.ClassDescription,
                    Price = s.ClassPrice
                }).FirstOrDefault();
        }

        public bool Remove(int class_id)
        {
            var remove_class = DatabaseAccessor.Instance.Class.FirstOrDefault(x => x.ClassId == class_id);

            if (remove_class == null)
                return false;

            DatabaseAccessor.Instance.Class.Remove(remove_class);
            DatabaseAccessor.Instance.SaveChanges();

            if (memoryCache.TryGetValue("Classes", out List<ClassModel> temp))
            {
                memoryCache.Remove("Classes");
            }

            return true;
        }

        public ClassModel Update(int class_id, string name, string description, decimal price)
        {
            var update_class = DatabaseAccessor.Instance.Class.FirstOrDefault(x => x.ClassId == class_id);

            if (update_class == null)
                return null;

            update_class.ClassName = name;
            update_class.ClassDescription = description;
            update_class.ClassPrice = price;

            DatabaseAccessor.Instance.SaveChanges();

            if (memoryCache.TryGetValue("Classes", out List<ClassModel> temp))
            {
                memoryCache.Remove("Classes");
            }

            return new ClassModel
            {
                Id = update_class.ClassId,
                Name = update_class.ClassName,
                Description = update_class.ClassDescription,
                Price = update_class.ClassPrice
            };
        }
    }
}
