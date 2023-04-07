using BCInsight.BAL.Repository;
using BCInsight.Code;
using BCInsight.DAL;
using BCInsight.Models;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Office2010.Excel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;

namespace BCInsight.Controllers
{
    public class SiteController : BaseController
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        ISite _site;

        public SiteController(ISite site)
        {
            _site = site;
        }

        public ActionResult Index()
        {
            List<SiteViewModel> siteList = new List<SiteViewModel>();
            try
            {
                var sites = _site.GetAll().Where(x => x.IsDeleted == false).Select(x => new SiteViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Address = x.Address,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    IsClosed = x.IsClosed == null ? false : x.IsClosed.Value,
                    ClosedOn = x.ClosedOn,
                    Remarks = x.Remarks,
                    CreatedBy = x.CreatedBy,
                    CreatedOn = x.CreatedOn,
                    ModifiedBy = x.ModifiedBy,
                    ModifiedOn = x.ModifiedOn,
                    DeletedBy = x.DeletedBy,
                    DeletedOn = x.DeletedOn,
                    IsDeleted = x.IsDeleted
                }).OrderByDescending(x => x.CreatedOn).ToList() ?? new List<SiteViewModel>();

                siteList = sites;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                TempData["errorMessage"] = "Error occured please contact your administrator.";
            }
            return View(siteList);
        }

        public ActionResult AddSite(int? Id)
        {
            SiteViewModel sitemodel = new SiteViewModel();
            try
            {
                if (Id == null || Id <= 0)
                {
                    return View(sitemodel);
                }
                var data = _site.FindBy(x => x.Id == Id).FirstOrDefault();
                if (data == null)
                {
                    TempData["errorMessage"] = "Site details not found.";
                    return RedirectToAction("Index");
                }
                sitemodel.Id = data.Id;
                sitemodel.Name = data.Name;
                sitemodel.Address = data.Address;
                sitemodel.Latitude = data.Latitude;
                sitemodel.Longitude = data.Longitude;
                sitemodel.Remarks = data.Remarks;
                sitemodel.IsClosed = data.IsClosed == null ? false : data.IsClosed.Value;
                sitemodel.ClosedOn = data.ClosedOn;
                sitemodel.CreatedBy = data.CreatedBy;
                sitemodel.CreatedOn = data.CreatedOn;
                sitemodel.ModifiedBy = data.ModifiedBy;
                sitemodel.ModifiedOn = data.ModifiedOn;
                sitemodel.DeletedBy = data.DeletedBy;
                sitemodel.DeletedOn = data.DeletedOn;
                sitemodel.IsDeleted = false;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                TempData["errorMessage"] = "Error occured please contact your administrator.";
            }
            return View(sitemodel);
        }

        [HttpPost]
        public ActionResult AddSite(SiteViewModel model)
        {
            try
            {
                using (var context = new admin_csmariseEntities())
                {
                    if (model.Id > 0)
                    {
                        var siteData = context.Site.Where(x => x.Id == model.Id && x.IsDeleted == false).FirstOrDefault();
                        if (siteData == null)
                        {
                            TempData["errorMessage"] = "Error occured please try after some time";
                            return RedirectToAction("Index");
                        }

                        var existSite = context.Site.Where(x => x.Name.Replace(" ", "").ToLower() == model.Name.Replace(" ", "").ToLower() && x.Id != model.Id && x.IsDeleted == false).FirstOrDefault();
                        if (existSite != null)
                        {
                            TempData["errorMessage"] = "Site name is already exist";
                            ModelState.AddModelError("Name", "Site name is already exist.");
                            return View("AddSite", new { Id = model.Id });
                        }
                        siteData.Name = model.Name;
                        siteData.Address = model.Address;
                        siteData.Latitude = model.Latitude;
                        siteData.Longitude = model.Longitude;
                        siteData.Remarks = model.Remarks;

                        if (model.IsClosed)
                        {
                            siteData.IsClosed = true;
                            siteData.ClosedOn = DateTime.Now;
                        }
                        else
                            siteData.IsClosed = false;

                        siteData.ModifiedBy = LoginUserId();
                        siteData.ModifiedOn = DateTime.Now;

                        context.Site.AddOrUpdate();
                        int isSave = context.SaveChanges();
                        if (isSave <= 0)
                        {
                            TempData["errorMessage"] = "Unable to save record, please try again";
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        var existSite = context.Site.Where(x => x.Name.Replace(" ", "").ToLower() == model.Name.Replace(" ", "").ToLower() && x.IsDeleted == false).FirstOrDefault();
                        if (existSite != null)
                        {
                            ModelState.AddModelError("Name", "Site name is already exist.");
                            return View(model);
                        }
                        DAL.Site siteData = new DAL.Site();
                        siteData.Name = model.Name;
                        siteData.Address = model.Address;
                        siteData.Latitude = model.Latitude;
                        siteData.Longitude = model.Longitude;
                        siteData.Remarks = model.Remarks;
                        siteData.IsClosed = false;
                        siteData.IsDeleted = false;
                        siteData.CreatedBy = LoginUserId();
                        siteData.CreatedOn = DateTime.Now;
                        context.Site.Add(siteData);
                        int isSave = context.SaveChanges();
                        if (isSave <= 0)
                        {
                            TempData["errorMessage"] = "Unable to save record, please try again";
                            return RedirectToAction("Index");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                TempData["errorMessage"] = "Error occured please contact your administrator.";
            }
            return RedirectToAction("Index");
        }

        public ActionResult DeleteSite(int Id)
        {
            try
            {
                var site = _site.FindBy(c => c.Id == Id).FirstOrDefault();
                if (site != null)
                {
                    site.IsDeleted = true;
                    site.DeletedBy = LoginUserId();
                    site.DeletedOn = DateTime.Now;
                    site.ModifiedBy = LoginUserId();
                    site.ModifiedOn = DateTime.Now;

                    _site.Edit(site);
                    _site.Save();
                }
                else
                    TempData["errorMessage"] = "Site record not found.";
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                TempData["errorMessage"] = "Error occured please contact your administrator.";
            }
            return RedirectToAction("Index");
        }
    }
}