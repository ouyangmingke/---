using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using 资产管理系统.Models;

namespace 资产管理系统.Controllers
{
    [Authorize]
    public class Fixed_AssetsController : Controller
    {
        Models.AssetDBEntities db = new Models.AssetDBEntities();

        // GET: Fixed_Assets

        /// <summary>
        /// 明细资产使用管理
        /// </summary>
        /// <returns></returns>
        public ActionResult AssetDetail()
        {
            return View();
        }

        /// <summary>
        /// 固定资产使用登记
        /// </summary>
        /// <returns></returns>
        public ActionResult AssetsFixedRegister()
        {
            return View();
        }

        /// <summary>
        /// 固定资产统计
        /// </summary>
        /// <returns></returns>
        public ActionResult AssetsFixedStatistics()
        {
            return View();
        }

        /// <summary>
        /// 维修明细
        /// </summary>
        /// <returns></returns>
        public ActionResult RepairDetails()
        {
            return View();
        }

        #region//固定资产统计
        /// <summary>
        /// 固定资产统计
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAssetsFixedStatistics()
        {
            db.Configuration.ProxyCreationEnabled = false;

            var list = from t1 in db.AssetDetailRecordInfo
                       join t2 in db.AssetDetailInfo on t1.AssetDetailId equals t2.AssetDetailId
                       select new { t1.AreaId, AssetNum = t1.AssetNum > 0 ? t1.AssetNum : 0, t2.AssetId };
            var List_AreaId = from t1 in db.AreaInfo select t1.AreaId;
            var List_AssetId = from t1 in db.AssetInfo where t1.AssetTypeID == 1 select new { t1.AssetId, t1.AssetName };
            LinkedList<IDictionary<string, string>> ARR = new LinkedList<IDictionary<string, string>>();
            JavaScriptSerializer jss = new JavaScriptSerializer();
            foreach (var AssetId_Name in List_AssetId)
            {
                IDictionary<string, string> idc = new Dictionary<string, string>();
                var temp = from t in list
                           where t.AssetId == AssetId_Name.AssetId
                           select new { AssetNum = t.AssetNum > 0 ? t.AssetNum : 0, t.AreaId };
                idc.Add("AssetName", AssetId_Name.AssetName);

                if (temp.Count() == 0)
                {
                    for (int i = 0; i < List_AreaId.Count(); i++)
                    {
                        idc.Add(i.ToString(), "0");
                    }
                }
                else
                {
                    int f = 0;
                    foreach (var AreaId in List_AreaId)
                    {

                        var sumlist = from t in temp
                                      where t.AreaId == AreaId
                                      select t.AssetNum > 0 ? t.AssetNum : 0;
                        if (sumlist.Count() == 0)
                        {
                            idc.Add(f.ToString(), "0");
                        }
                        else
                        {
                            idc.Add(f.ToString(), sumlist.Sum().ToString());
                        }
                        f++;
                    }
                }
                ARR.AddLast(idc);
            }
            return Json(ARR, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region//图片上传
        /// <summary>
        /// 图片上传
        /// </summary>
        /// <returns></returns>
        public ActionResult ImgUpdate()
        {
            string result = string.Empty;
            //得到上传的图片对象												
            HttpPostedFileBase file = Request.Files[0];
            //得到上传图片的完整路径												
            string path = Server.MapPath("~/images/FileUpload/") + file.FileName;
            if (file.FileName.EndsWith(".jpg") || file.FileName.EndsWith(".png") || file.FileName.EndsWith(".gif"))
            {
                file.SaveAs(path);//图片保存												
                result = file.FileName;
            }
            else
            {
                result = "-1";
            }
            //JavaScriptSerializer jss = new JavaScriptSerializer();
            //string json = jss.Serialize(result);
            //context.Response.Write(json);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region//查看资产维修详情
        /// <summary>
        /// 查看资产维修详情
        /// </summary>
        /// <returns></returns>
        public ActionResult GetViewRepairDetails(int DamageRecordId)
        {
            //db.Configuration.ProxyCreationEnabled = false;

            var list = (from t1 in db.DamageRecordInfo
                        join t2 in db.EmpolyInfo on t1.EmpolyId equals t2.EmpolyId
                        join t3 in db.AssetDetailInfo on t1.AssetDetailId equals t3.AssetDetailId
                        join t4 in db.AssetInfo on t3.AssetId equals t4.AssetId
                        where t1.DamageRecordId == DamageRecordId
                        select new
                        {
                            t1.DamageRecordId,
                            t4.AssetName,
                            t1.ProblemImange,
                            t2.EmpolyName,
                            t1.Repairman,
                            t1.RepairDates,
                            t1.DamageDate,
                            t1.DamageCauses,
                            t1.ProblemDescription,
                            t1.RecordState,
                            t1.DamageRecordReMark
                        }).FirstOrDefault();


            byte[] File = list.ProblemImange;

            FileStream fs;
            FileInfo fi = new System.IO.FileInfo(Server.MapPath("~/images/serverimg/temp" + list.DamageRecordId + ".jpg"));
            fs = fi.OpenWrite();
            fs.Write(File, 0, File.Length);
            fs.Close();
            if (list != null)
            {
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(-1, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        #region//获取资产维修明细
        /// <summary>
        /// 获取资产维修明细
        /// </summary>
        /// <returns></returns>
        public ActionResult GetRepairDetails()
        {
            //db.Configuration.ProxyCreationEnabled = false;
            int limit = 5, beginIndex = 0;
            if (!string.IsNullOrEmpty(Request["limit"]))
                limit = int.Parse(Request["limit"].ToString());//每页显示的数量

            if (!string.IsNullOrEmpty(Request["offset"]))
                beginIndex = int.Parse(Request["offset"].ToString());// SQL语句起始索引

            var list = from t1 in db.DamageRecordInfo
                       join t2 in db.EmpolyInfo on t1.EmpolyId equals t2.EmpolyId
                       join t3 in db.AssetDetailInfo on t1.AssetDetailId equals t3.AssetDetailId
                       join t4 in db.AssetInfo on t3.AssetId equals t4.AssetId
                       select new
                       {
                           t1.DamageRecordId,
                           t4.AssetName,
                           t2.EmpolyName,
                           t1.DamageDate,
                           t1.DamageCauses,
                           t1.ProblemDescription,
                           t1.RecordState,
                           t1.DamageRecordReMark,
                           t3.AssetDetailUseState
                       };
            if (!string.IsNullOrEmpty(Request["AssetName"]))
            {
                string AssetName = Request["AssetName"];
                list = list.Where(t4 => t4.AssetName.Contains(AssetName));
            }
            if (!string.IsNullOrEmpty(Request["RecordState"]))
            {
                int RecordState = int.Parse(Request["RecordState"]);
                if (RecordState != -1)
                    list = list.Where(t1 => t1.RecordState == RecordState);
            }
            var data = new
            {
                total = list.Count(),
                rows = list.ToList().Skip((beginIndex)).Take(limit)
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region//添加资产维修明细
        /// <summary>
        /// 添加资产维修明细
        /// </summary>
        /// <returns></returns>
        public int AddDamageRecord(string Imange, DamageRecordInfo DRI)
        {
            //保存文件到SQL Server数据库中
            FileInfo fi = new FileInfo(Server.MapPath("~/images/FileUpload/") + Imange);
            FileStream fs = fi.OpenRead();
            byte[] bytes = new byte[fs.Length];
            fs.Read(bytes, 0, Convert.ToInt32(fs.Length));

            DRI.ProblemImange = bytes;

            db.Configuration.ProxyCreationEnabled = false;
            DRI.EmpolyId = int.Parse(Session["EmpolyId"].ToString());
            db.DamageRecordInfo.Add(DRI);   
            return db.SaveChanges();
        }
        #endregion

        #region//获取资产明细编号
        /// <summary>
        /// 获取资产明细编号
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAssetDetailId()
        {
            Session["EmpolyId"] = 1;
            if (Session["EmpolyId"] != null)
            {
                int EmpolyId = int.Parse(Session["EmpolyId"].ToString());
                //通过资产ID 查询资产明细编号 插入数据
                var upd = from DRI in db.AssetDetailRecordInfo
                          where DRI.EmpolyId == EmpolyId
                          select DRI.AssetDetailRecordId;

                if (upd.Count() > 0)
                {
                    return Json(upd, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(-1, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region//修改维修状态
        /// <summary>
        /// 修改维修状态
        /// </summary>
        /// <returns></returns>
        public int UpdateRepair(int DamageRecordId)
        {
            //通过资产ID 查询资产明细编号 插入数据
            var upd = (from DRI in db.DamageRecordInfo
                       where DRI.DamageRecordId == DamageRecordId
                       select DRI).FirstOrDefault();
            if (upd != null)
            {
                Session["EmpolyName"] = "cheshi";
                upd.RecordState = 1;
                upd.Repairman = Session["EmpolyName"].ToString();
                db.Entry(upd).State = System.Data.Entity.EntityState.Modified;
                return db.SaveChanges();
            }
            else return -1;

        }
        #endregion

        #region//获取固定资产使用登记
        /// <summary>
        /// 获取固定资产使用登记
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAssetsFixedRegister()
        {
            //db.Configuration.ProxyCreationEnabled = false;
            int limit = 5, beginIndex = 0;
            if (!string.IsNullOrEmpty(Request["limit"]))
                limit = int.Parse(Request["limit"].ToString());//每页显示的数量

            if (!string.IsNullOrEmpty(Request["offset"]))
                beginIndex = int.Parse(Request["offset"].ToString());// SQL语句起始索引

            var list = from t1 in db.AssetDetailRecordInfo
                       join t2 in db.EmpolyInfo on t1.EmpolyId equals t2.EmpolyId
                       join t3 in db.AreaInfo on t1.AreaId equals t3.AreaId
                       join t4 in db.AssetDetailInfo on t1.AssetDetailId equals t4.AssetDetailId
                       join t5 in db.AssetInfo on t4.AssetId equals t5.AssetId
                       select new
                       {
                           t1.AssetDetailRecordId,
                           t4.AssetDetailId,
                           t4.AssetDetailNum,
                           t2.EmpolyName,
                           t3.AreaName,
                           t1.AssetNum,
                           t1.AssetDetailRecordUseDate,
                           t1.AssetDetailRecordReturnDate,
                           t1.AssetDetailRecordReMark,
                           t5.AssetName

                       };
            if (!string.IsNullOrEmpty(Request["AssetName"]))
            {
                string AssetName = Request["AssetName"];
                list = list.Where(t5 => t5.AssetName.Contains(AssetName));
            }


            var data = new
            {
                total = list.Count(),
                rows = list.ToList().Skip((beginIndex)).Take(limit)
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region//修改归还时间
        /// <summary>
        /// 修改归还时间
        /// </summary>
        /// <returns></returns>
        public int UpdateReturnDate(int AssetDetailRecordId, int AssetDetailId, DateTime Endtime, string Mark)
        {
            int re = 0;
            var upad = from ad in db.AssetDetailInfo where ad.AssetDetailId == AssetDetailId select ad;
            if (upad.Count() != 0)
            {
                AssetDetailInfo adi = upad.FirstOrDefault();
                adi.AssetDetailReturnDate = Endtime;
                adi.AssetAreaReMark = Mark;
                db.Entry(adi).State = System.Data.Entity.EntityState.Modified;
            }
            re += db.SaveChanges();

            var upadr = from adr in db.AssetDetailRecordInfo where adr.AssetDetailRecordId == AssetDetailRecordId select adr;
            if (upadr.Count() != 0)
            {
                AssetDetailRecordInfo adri = upadr.FirstOrDefault();
                adri.AssetDetailRecordReturnDate = Endtime;
                adri.AssetDetailRecordReMark = Mark;
                db.Entry(adri).State = System.Data.Entity.EntityState.Modified;
            }
            re += db.SaveChanges();
            return re;
        }
        #endregion

        #region//获取资产使用明细
        /// <summary>
        /// 获取资产明细
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAssetDetail()
        {
            //db.Configuration.ProxyCreationEnabled = false;
            int limit = 5, beginIndex = 0;
            if (!string.IsNullOrEmpty(Request["limit"]))
                limit = int.Parse(Request["limit"].ToString());//每页显示的数量

            if (!string.IsNullOrEmpty(Request["offset"]))
                beginIndex = int.Parse(Request["offset"].ToString());// SQL语句起始索引

            var list = from t1 in db.AssetDetailRecordInfo
                       join tt in db.AssetDetailInfo on t1.AssetDetailId equals tt.AssetDetailId
                       join t2 in db.EmpolyInfo on t1.EmpolyId equals t2.EmpolyId into temp1
                       from left1 in temp1.DefaultIfEmpty()
                       join t3 in db.AreaInfo on t1.AreaId equals t3.AreaId into temp2
                       from left2 in temp2.DefaultIfEmpty()
                       join t4 in db.AssetInfo on tt.AssetId equals t4.AssetId into temp3
                       from left3 in temp3.DefaultIfEmpty()
                       select new
                       {
                           t1.AssetDetailId,
                           left3.AssetName,
                           tt.AssetDetailNum,
                           tt.AssetDetailUseState,
                           tt.AssetDetailUseDate,
                           tt.AssetDetailReturnDate,
                           tt.AssetDetailDumState,
                           left1.EmpolyName,
                           left2.AreaName,
                           t1.AssetDetailRecordReMark,
                           t1.EmpolyId,
                           t1.AreaId
                       };
            if (!string.IsNullOrEmpty(Request["AssetName"]))
            {
                string AssetName = Request["AssetName"];
                list = list.Where(left3 => left3.AssetName.Contains(AssetName));
            }
            if (!string.IsNullOrEmpty(Request["AssetDetailUseState"]))
            {
                int AssetDetailUseState = int.Parse(Request["AssetDetailUseState"]);
                if (AssetDetailUseState != -1)
                    list = list.Where(t1 => t1.AssetDetailUseState == AssetDetailUseState);
            }
            var data = new
            {
                total = list.Count(),
                rows = list.ToList().Skip((beginIndex)).Take(limit)
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region//添加资产使用明细
        /// <summary>
        /// 添加资产明细
        /// </summary>
        /// <returns></returns>
        public int AddAssetDetail(int AssetId, AssetDetailRecordInfo ADRinfo)
        {

            //通过资产ID 查询资产明细编号 插入数据
            ADRinfo.AssetDetailId = (from ADinfo in db.AssetDetailInfo
                                     where ADinfo.AssetId == AssetId
                                     select ADinfo.AssetDetailId).FirstOrDefault();

            db.AssetDetailRecordInfo.Add(ADRinfo);
            return db.SaveChanges();
        }
        #endregion

        #region//填充资产名称
        /// <summary>
        /// 填充资产名称
        /// </summary>
        /// <returns></returns>
        public ActionResult SetAssetName()
        {
            var list = from AssetName in db.AssetInfo select new { AssetName.AssetId, AssetName.AssetTypeID, AssetName.AssetName };
            if (!string.IsNullOrEmpty(Request["AssetTypeID"]))
            {
                int AssetTypeID = int.Parse(Request["AssetTypeID"]);
                list = list.Where(T => T.AssetTypeID == AssetTypeID);
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region//填充员工名称
        /// <summary>
        /// 填充员工名称
        /// </summary>
        /// <returns></returns>
        public ActionResult SetEmpolyName()
        {
            var list = from EmpolyName in db.EmpolyInfo select new { EmpolyName.EmpolyId, EmpolyName.EmpolyName };
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region//填充区域名称
        /// <summary>
        ///  填充区域名称
        /// </summary>
        /// <returns></returns>
        public ActionResult SetAreaName()
        {
            var list = from AreaName in db.AreaInfo select new { AreaName.AreaId, AreaName.AreaName };
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}