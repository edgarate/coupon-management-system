﻿using System.Web.Mvc;
using System.Web.UI;
using Core;
using Core.Model;
using Core.Service;
using Infra.Builder;
using Infra.Dto;

namespace WebUI.Controllers
{
    /// <summary>
    /// generic crud controller for entities where there is difference between the edit and create view
    /// </summary>
    /// <typeparam name="TEntity"> the entity</typeparam>
    /// <typeparam name="TCreateInput">create viewmodel</typeparam>
    /// <typeparam name="TEditInput">edit viewmodel</typeparam>
    public class Crudere<TEntity, TCreateInput, TEditInput> : BaseController
        where TCreateInput : new()
        where TEditInput : Input, new()
        where TEntity : DelEntity, new()
    {
        protected readonly ICrudService<TEntity> s;
        private readonly IBuilder<TEntity, TCreateInput> v;
        private readonly IBuilder<TEntity, TEditInput> ve;

        protected virtual string EditView
        {
            get { return "edit"; }
        }

        public Crudere(ICrudService<TEntity> s, IBuilder<TEntity, TCreateInput> v, IBuilder<TEntity, TEditInput> ve)
        {
            this.s = s;
            this.v = v;
            this.ve = ve;
        }

        public virtual ActionResult Index()
        {
            return View("cruds");
        }

        public ActionResult Row(int id)
        {
            return View("rows", new[] { s.Get(id) });
        }

        public ActionResult Create()
        {
            return View(v.BuildInput(new TEntity()));
        }

        [HttpPost]
        public virtual ActionResult Create(TCreateInput o)
        {
            if (!ModelState.IsValid)
                return View(v.RebuildInput(o));
            return Json(new { Id = s.Create(v.BuildEntity(o)) });
        }

        [OutputCache(Location = OutputCacheLocation.None)]//for ie8
        public ActionResult Edit(int id)
        {
            var o = s.Get(id);
            if (o == null) throw new sArtException("this entity doesn't exist anymore");
            return View(EditView, ve.BuildInput(o));
        }

        [HttpPost]
        public virtual ActionResult Edit(TEditInput input)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(EditView, ve.RebuildInput(input, input.Id));
                s.Save(ve.BuildEntity(input, input.Id));
            }
            catch (sArtException ex)
            {
                return Content(ex.Message);
            }
            return Json(new { input.Id });
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            s.Delete(id);
            return Json(new { Id = id });
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Restore(int id)
        {
            s.Restore(id);
            return Json(new { Id = id });
        }
    }
}