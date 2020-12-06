using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using 资产管理系统.Models;

namespace 资产管理系统.Controllers
{
    [Authorize]
    public class SchoolManagerController : Controller
    {
        Models.AssetDBEntities db = new Models.AssetDBEntities();
        // GET: SchoolManager

        #region 部门管理

        public ActionResult DeptInfo()
        {
            return View();
        }

        /// <summary>
        /// 查询部门信息——模糊查询
        /// </summary>
        /// <param name="DeptName">部门名称</param>
        /// <param name="CampusID">校区编号</param>
        /// <returns></returns>
        public ActionResult Sel_DeptInfo(string DeptName, string CampusID)
        {
            var res = from i in db.DeptInfo join j in db.CampusInfo on i.CampusId equals j.CampusId select new { j.CampusId, j.CampusName, i.DeptId, i.DeptName, i.DeptReMark }; ;
            if (!DeptName.IsEmpty()) res = res.Where(s => s.DeptName.IndexOf(DeptName) >= 0);
            if (CampusID != "-1") res = res.Where(s => s.CampusId + "" == CampusID);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加部门信息
        /// </summary>
        /// <param name="CampusID">所在校区</param>
        /// <param name="DeptName">部门名称</param>
        /// <param name="DeptReMark">部门说明</param>
        /// <returns></returns>
        public int Add_DeptInfo(int CampusID, string DeptName, string DeptReMark)
        {
            var dept = from i in db.DeptInfo where i.DeptName == DeptName select i;
            if (dept.Count() > 0) return -1;

            DeptInfo info = new DeptInfo();
            info.CampusId = CampusID;
            info.DeptName = DeptName;
            info.DeptReMark = DeptReMark;

            db.DeptInfo.Add(info);
            return db.SaveChanges();
        }

        /// <summary>
        /// 通过部门id删除部门信息
        /// </summary>
        /// <param name="DeptID"></param>
        /// <returns></returns>
        public int Del_DeptInfo(int DeptID)
        {

            var dept = (from i in db.DeptInfo where i.DeptId == DeptID select i).Single();

            db.DeptInfo.Remove(dept);

            return db.SaveChanges();

        }


        public int Upd_DeptInfo(int DeptID, string DeptName, int CampusId, string DeptReMark)
        {
            var deptinfo = from i in db.DeptInfo where i.DeptId == DeptID select i;
            if (deptinfo.Count() > 0)
            {
                DeptInfo info = new DeptInfo();
                info.DeptId = DeptID;
                info.DeptName = DeptName;
                info.CampusId = CampusId;
                info.DeptReMark = DeptReMark;
                db.Entry(info).State = System.Data.Entity.EntityState.Modified;
                return db.SaveChanges();
            }
            else
            {
                return -1;
            }
        }

        #endregion

        #region 角色管理

        public ActionResult RoleInfo()
        {
            return View();
        }

        /// <summary>
        /// 获取角色信息
        /// </summary>
        /// <param name="RoleName">模糊查询</param>
        /// <returns></returns>
        public ActionResult Sel_RoleInfo(string RoleName)
        {
            var res = from i in db.RoleInfo select new { i.RoletId, i.RoleName, i.RoleReMark };
            if (!RoleName.IsEmpty()) res = res.Where(s => s.RoleName.IndexOf(RoleName) >= 0);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加角色信息
        /// </summary>
        /// <param name="RoleName">角色姓名</param>
        /// <param name="RoleReMark">角色说明</param>
        /// <returns>写入的数目</returns>
        public int Add_RoleInfo(string RoleName, string RoleReMark)
        {
            RoleInfo info = new RoleInfo();
            info.RoleName = RoleName;
            info.RoleReMark = RoleReMark;
            db.RoleInfo.Add(info);
            return db.SaveChanges();
        }

        /// <summary>
        /// 删除角色信息
        /// </summary>
        /// <param name="RoleId">角色id</param>
        /// <returns></returns>
        public int Del_RoleInfo(int RoleId)
        {
            var role = (from i in db.RoleInfo where i.RoletId == RoleId select i);
            if (role.Count() == 0)
            {
                return -1;
            }
            else
            {
                db.RoleInfo.Remove(role.Single());
                return db.SaveChanges();

            }
        }

        /// <summary>
        /// 修改角色信息
        /// </summary>
        /// <param name="RoleId">角色编号</param>
        /// <param name="RoleName">角色名称</param>
        /// <param name="RoleReMark">角色说明</param>
        /// <returns></returns>
        public int Upd_RoleInfo(int RoleId, string RoleName, string RoleReMark)
        {
            RoleInfo info = new RoleInfo();
            info.RoletId = RoleId;
            info.RoleName = RoleName;
            info.RoleReMark = RoleReMark;
            db.Entry(info).State = System.Data.Entity.EntityState.Modified;
            return db.SaveChanges();
        }
        #endregion

        #region 校区管理

        public ActionResult CampusInfo()
        {
            return View();
        }


        /// <summary>
        /// 查询校区信息
        /// </summary>
        /// <param name="namelike">模糊查询，值为空则查询所有</param>
        /// <returns></returns>
        public ActionResult sel_CampusInfo(string namelike)
        {
            if (namelike.IsEmpty())
            {
                var res = from i in db.CampusInfo select new { i.CampusId, i.CampusName, i.CampusReMark };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var res = from i in db.CampusInfo where i.CampusName.IndexOf(namelike) >= 0 select new { i.CampusId, i.CampusName, i.CampusReMark };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// 添加校区信息
        /// </summary>
        /// <param name="name">校区名称</param>
        /// <param name="remark">校区说明</param>
        /// <returns></returns>
        public int add_CampusInfo(string name, string remark)
        {
            CampusInfo info = new CampusInfo();
            info.CampusName = name;
            info.CampusReMark = remark;
            db.CampusInfo.Add(info);
            return db.SaveChanges();
        }


        /// <summary>
        /// 通过校区编号删除校区信息
        /// </summary>
        /// <param name="CampusID">校区编号</param>
        /// <returns></returns>
        public int del_CampusInfo_byID(int CampusID)
        {
            CampusInfo c = (from i in db.CampusInfo where i.CampusId == CampusID select i).Single();
            var deptinfos = from i in db.DeptInfo where i.CampusId == CampusID select i;
            if (deptinfos.Count() > 0)
            {
                return -1;
            }
            else
            {
                db.CampusInfo.Remove(c);
                int res = db.SaveChanges();
                return res;
            }
        }

        /// <summary>
        /// 更改校区信息
        /// </summary>
        /// <param name="id">要更改的校区编号</param>
        /// <param name="name">校区名</param>
        /// <param name="remark">校区说明</param>
        /// <returns></returns>
        public int upd_CampusInfo(int id, string name, string remark)
        {

            var c = from i in db.CampusInfo where i.CampusId == id select i;
            if (c.Count() > 0)
            {
                CampusInfo info = new CampusInfo();
                info.CampusId = id;
                info.CampusName = name;
                info.CampusReMark = remark;
                db.Entry(info).State = System.Data.Entity.EntityState.Modified;
                return db.SaveChanges();
            }
            else
            {
                return -1;
            }
        }
        #endregion

        #region 员工管理

        public ActionResult EmpolyInfo()
        {
            return View();
        }


        /// <summary>
        /// 获取所有员工信息 
        /// </summary>
        /// <param name="EmpolyName">模糊查询姓名</param>
        /// <param name="DeptId">部门id</param>
        /// <param name="RoleId">角色id</param>
        /// <param name="Gender">性别</param>
        /// <returns>员工信息集合</returns>
        public ActionResult Sel_EmpolyInfo(string EmpolyName, int DeptId, int RoleId, int Gender)
        {
            var res = from employ in db.EmpolyInfo
                      join dept in db.DeptInfo on employ.DeptId equals dept.DeptId
                      join role in db.RoleInfo on employ.RoletId equals role.RoletId
                      select new
                      {
                          employ.EmpolyId,
                          employ.EmpolyNum,
                          employ.EmpolyName,
                          employ.EmpolySex,
                          employ.EmpolyIdCard,
                          dept.DeptId,
                          dept.DeptName,
                          employ.EmpolyLevel,
                          role.RoletId,
                          role.RoleName,
                          employ.InductionDate,
                          employ.DepartureDate,
                          employ.EmpolyReMark
                      };
            if (!EmpolyName.IsEmpty()) res = res.Where(s => s.EmpolyName.IndexOf(EmpolyName) >= 0);
            if (DeptId != -1) res = res.Where(s => s.DeptId == DeptId);
            if (RoleId != -1) res = res.Where(s => s.RoletId == RoleId);
            if (Gender != -1) res = res.Where(s => s.EmpolySex == (Gender == 1 ? "男" : "女"));

            return Json(res, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 删除员工信息
        /// </summary>
        /// <param name="EmpolyId">员工id</param>
        /// <returns>成功返回1</returns>
        public int Del_EmpolyInfo(int EmpolyId)
        {
            var info = from i in db.EmpolyInfo where i.EmpolyId == EmpolyId select i;
            db.EmpolyInfo.Remove(info.Single());
            return db.SaveChanges();
        }

        /// <summary>
        /// 增加员工信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public int Add_EmpolyInfo(EmpolyInfo info)
        {
            db.EmpolyInfo.Add(info);
            return db.SaveChanges();

        }


        /// <summary>
        /// 修改员工信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public int Upd_EmpolyInfo(EmpolyInfo info)
        {
            try
            {

                EmpolyInfo emp = (from i in db.EmpolyInfo where i.EmpolyId == info.EmpolyId select i).Single();
                emp.DepartureDate = info.DepartureDate;
                emp.DeptId = info.DeptId;
                emp.EmpolyIdCard = info.EmpolyIdCard;
                emp.EmpolyLevel = info.EmpolyLevel;
                emp.EmpolyName = info.EmpolyName;
                emp.EmpolyNum = info.EmpolyNum;
                emp.EmpolyReMark = info.EmpolyReMark;
                emp.EmpolySex = info.EmpolySex;
                emp.InductionDate = info.InductionDate;
                emp.RoletId = info.RoletId;

                db.Entry(emp).State = System.Data.Entity.EntityState.Modified;
                return db.SaveChanges();
            }
            catch (Exception)
            {
                return -1;
            }

        }
        #endregion
    }
}