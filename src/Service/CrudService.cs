﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Omu.ValueInjecter;
using Core.Model;
using Core.Repository;
using Core.Service;

namespace Service
{
    public class CrudService<T> : ICrudService<T> where T : DelEntity, new()
    {
        protected IRepo<T> repo;

        public CrudService(IRepo<T> repo)
        {
            this.repo = repo;
        }

        public IEnumerable<T> GetAll()
        {
            return repo.GetAll();
        }

        public int Count()
        {
            return repo.Count();
        }

        public T Get(int id)
        {
            return repo.Get(id);
        }

        public virtual int Create(T e)
        {
            repo.Insert(e);
            repo.Save();
            return e.Id;
        }

        public virtual void Save(T e)
        {
            var o = repo.Get(e.Id);
            o.InjectFrom(e);
            repo.Save();
        }

        public virtual void Delete(int id)
        {
            repo.Delete(repo.Get(id));
            repo.Save();
        }

        public void Restore(int id)
        {
            repo.Restore(repo.Get(id));
            repo.Save();
        }

        public IEnumerable<T> Where(Expression<Func<T, bool>> predicate, bool showDeleted = false)
        {
            return repo.Where(predicate, showDeleted);
        }
    }
}