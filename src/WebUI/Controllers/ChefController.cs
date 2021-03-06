﻿using System.Linq;
using System.Web.Mvc;
using Omu.Awesome.Mvc;
using Core.Model;
using Core.Service;
using Infra.Builder;
using Infra.Dto;

namespace WebUI.Controllers
{
    public class ChefController : Cruder<Chef, ChefInput>
    {
        public ChefController(ICrudService<Chef> s, IBuilder<Chef, ChefInput> v)
            : base(s, v)
        {
        }
        public virtual ActionResult Search(string search, int? countryId, int page = 1, int ps = 5)
        {
            var src = s.Where(o => o.FirstName.StartsWith(search) || o.LastName.StartsWith(search), User.IsInRole("admin"));
            if (countryId != null) src = src.Where(o => o.CountryId == countryId);
            var rows = this.RenderView("rows", src.OrderBy(u => u.Id).Skip((page - 1) * ps).Take(ps));

            return Json(new { rows, more = src.Count() > page * ps });
        }
    }
}