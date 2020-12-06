using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using 资产管理系统.Models;

namespace 资产管理系统.Controllers
{
    [Authorize]
    public class PropertyController : Controller
    {
        Models.AssetDBEntities db = new Models.AssetDBEntities();


        #region 区域类别信息管理


        // GET: Property
        public ActionResult AreaTypeInfo()
        {
            return View();
        }

        /// <summary>
        /// 获取所有的区域类别信息
        /// </summary>
        /// <param name="AreaTypeName">模糊查询区域类别名称</param>
        /// <returns></returns>
        public ActionResult sel_AreaTypeInfo(string AreaTypeName)
        {
            var infos = from i in db.AreaTypeInfo select new { i.AreaTypeId, i.AreaTypeName, i.AreaTypeReMark };
            if (AreaTypeName != "") infos = infos.Where(s => s.AreaTypeName.IndexOf(AreaTypeName) >= 0);
            return Json(infos, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除区域类别信息
        /// </summary>
        /// <param name="AreaTypeId">区域类别信息id</param>
        /// <returns></returns>
        public int del_AreaTypeInfo(int AreaTypeId)
        {
            var info = (from i in db.AreaTypeInfo where i.AreaTypeId == AreaTypeId select i).Single();
            db.AreaTypeInfo.Remove(info);
            return db.SaveChanges();
        }

        /// <summary>
        /// 添加区域类别信息
        /// </summary>
        /// <param name="AreaTypeName">区域类别名称</param>
        /// <param name="AreaTypeReMark">区域类别备注</param>
        /// <returns></returns>
        public int add_AreaTypeInfo(string AreaTypeName, string AreaTypeReMark)
        {
            AreaTypeInfo info = new AreaTypeInfo();
            info.AreaTypeName = AreaTypeName;
            info.AreaTypeReMark = AreaTypeReMark;
            db.AreaTypeInfo.Add(info);
            return db.SaveChanges();
        }

        /// <summary>
        /// 修改区域类别信息
        /// </summary>
        /// <param name="AreaTypeId">区域类别id</param>
        /// <param name="AreaTypeName">区域类别名称</param>
        /// <param name="AreaTypeReMark">区域类别备注</param>
        /// <returns></returns>
        public int upd_AreaTypeInfo(int AreaTypeId, string AreaTypeName, string AreaTypeReMark)
        {
            var info = (from i in db.AreaTypeInfo where i.AreaTypeId == AreaTypeId select i).Single();
            info.AreaTypeName = AreaTypeName;
            info.AreaTypeReMark = AreaTypeReMark;
            db.Entry(info).State = System.Data.Entity.EntityState.Modified;
            return db.SaveChanges();
        }

        #endregion


        #region 区域信息管理


        public ActionResult AreaInfo()
        {
            return View();
        }

        /// <summary>
        /// 获取所有区域信息
        /// </summary>
        /// <param name="AreaName">模糊查询 区域名称</param>
        /// <param name="AreaTypeId">筛选区域类别id</param>
        /// <returns>返回区域信息集合</returns>
        public ActionResult sel_AreaInfo(string AreaName, int AreaTypeId)
        {
            var infos = from i in db.AreaInfo join s in db.AreaTypeInfo on i.AreaTypeId equals s.AreaTypeId select new { i.AreaId, i.AreaName, i.AreaReMark, s.AreaTypeId, s.AreaTypeName };
            if (AreaName != "") infos = infos.Where(s => s.AreaName.IndexOf(AreaName) >= 0);
            if (AreaTypeId != -1) infos = infos.Where(s => s.AreaTypeId == AreaTypeId);
            return Json(infos, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除区域信息
        /// </summary>
        /// <param name="AreaId">区域id</param>
        /// <returns>删除成功返回1</returns>
        public int del_AreaInfo(int AreaId)
        {
            var info = (from i in db.AreaInfo where i.AreaId == AreaId select i).Single();
            db.AreaInfo.Remove(info);
            return db.SaveChanges();
        }


        /// <summary>
        /// 添加区域信息
        /// </summary>
        /// <param name="AreaName">区域名称</param>
        /// <param name="AreaTypeId">区域类别id</param>
        /// <param name="AreaReMark">区域备注</param>
        /// <returns></returns>
        public int add_AreaInfo(string AreaName, int AreaTypeId, string AreaReMark)
        {
            AreaInfo info = new AreaInfo();
            info.AreaTypeId = AreaTypeId;
            info.AreaName = AreaName;
            info.AreaReMark = AreaReMark;
            db.AreaInfo.Add(info);
            return db.SaveChanges();
        }

        /// <summary>
        /// 修改区域信息
        /// </summary>
        /// <param name="AreaId">区域id</param>
        /// <param name="AreaTypeId">区域类别id</param>
        /// <param name="AreaName">区域名称</param>
        /// <param name="AreaReMark">区域备注</param>
        /// <returns></returns>
        public int upd_AreaInfo(int AreaId, int AreaTypeId, string AreaName, string AreaReMark)
        {
            AreaInfo info = new AreaInfo();
            info.AreaId = AreaId;
            info.AreaTypeId = AreaTypeId;
            info.AreaName = AreaName;
            info.AreaReMark = AreaReMark;
            db.Entry(info).State = System.Data.Entity.EntityState.Modified;
            return db.SaveChanges();
        }

        #endregion


    }
}