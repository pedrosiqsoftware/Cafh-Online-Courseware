﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lang;
using Org.Cafh.Courseware.Models.Courseware;
using Org.Cafh.Courseware.Models.Repositories;

namespace Org.Cafh.Courseware.Controllers
{
    public class CoursesController : Controller
    {

        private static CoursewareDataContext _context = new CoursewareDataContext();
        private static readonly LanguagesRepository _langs = new LanguagesRepository(ref _context);
        private static readonly CoursesRepository _courses = new CoursesRepository(ref _context);

        [HttpGet]
        public ActionResult SelectLanguage()
        {
            ViewBag.Langs = Langs();
            return View();
        }

        // -------------------------------------------------------------------------------------------------------------------------------- //
        // ACTION SELECT LANGUAGE                                                                                                           //
        // -------------------------------------------------------------------------------------------------------------------------------- //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SelectLanguage(LanguageSelectViewModel model)
        {

            if (ModelState.IsValid)
            {
                return RedirectToAction("Index", new {language = model.Language});
            }
            else
            {
                ModelState.AddModelError("", Global.LanguageSelectError);
            }
            return View(model);
        } // ACTION SELECT LANGUAGE ENDS -------------------------------------------------------------------------------------------------- //

        /// <summary>
        /// 
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index(Guid language)
        {
            var dbLangs = _courses.Get(language);
            var langs = dbLangs.Select(CourseViewModel.Transform).ToList();
            ViewBag.Language = language;
            return View(langs);
        } // METHOD INDEX ENDS ------------------------------------------------------------------------------------------------------------ // 



        /// <summary>
        /// 
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Create(Guid language)
        {
            var course = new CourseViewModel
            {
                Language = language,
                Id = Guid.NewGuid()
            };
            return View(course);
        } // METHOD CREATE ENDS ----------------------------------------------------------------------------------------------------------- //


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create(CourseViewModel model)
        {
            ActionResult result;
            if (ModelState.IsValid)
            {
               var c = _courses.Post(CourseViewModel.Transform(model));
                if (c != null && c.Id != Guid.Empty)
                {
                    result = RedirectToAction("Index", "Teachings", new {course = c.Id});
                } // IF ENDS
                else
                {
                    ModelState.AddModelError("", @Global.ErrorProcessingRequest);
                    result = View(model);
                } // ELSE ENDS
            } // IF ENDS
            else
            {
                result = View(model);
            }
            return result;
        } // METHOD CREATE ENDS ------------------------------------------------------------------------------------------------------------ //



        #region SELECTLISTS

        public JsonResult Langs()
        {
            var languages = _langs.Get();
            var languageItems =
                languages.Select(item => new SelectListItem
                {
                    Value = item.Id.ToString(),
                    Text = item.Title
                }).ToList();
            var deftlt = new SelectListItem
            {
                Value = "",
                Text = Global.Select,
                Selected = true
            };
            languageItems.Add(deftlt);
            languageItems.Reverse();
            return Json(languageItems, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}