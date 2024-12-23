using _21T1020254.BusinessLayers;
using _21T1020254.DomainModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020254.Web.Models;
using System.Globalization;

namespace SV21T1020254.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMIN}")]
    public class EmployeeController : Controller
    {
        public const int PAGE_SIZE = 18;
        private const string EMPLOYEE_SEARCH_CONDITION = "EmployeeSearchCondition";

        public IActionResult Index()

        {
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(EMPLOYEE_SEARCH_CONDITION);
            if (condition == null)
                condition = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            return View(condition);
        }

        public IActionResult Search(PaginationSearchInput condition)
        {
            int rowCount;
            var data = CommonDataService.ListOfEmployees(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
            EmployeeSearchResult model = new EmployeeSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data

            };
            ApplicationContext.SetSessionData(EMPLOYEE_SEARCH_CONDITION, condition);
            return View(model);
        }

        public IActionResult Create()
        {

            ViewBag.Title = "Bổ sung nhân viên";
            var data = new Employee()
            {
                EmployeeID = 0,
                IsWorking = false,
                Photo= "no-img.jpg"

            };
            return View("Edit", data);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhập thông tin nhân viên";
            var data = CommonDataService.GetEmployee(id);

            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }

        [HttpPost]
        public IActionResult Save(Employee data,string _birthDate , IFormFile?uploadPhoto)
        {

            ViewBag.Title = data.EmployeeID == 0 ? "Bổ sung nhân viên" : "Cập nhập thông tin nhân viên";
            //Todo:Kiem soat du lieu dau vao, nếu không hợp lệ thì ta tạo ra thông báo lỗi và lưu giữ nó trong ModelState
            //ModelState.addmodelError(key,message)

            if (string.IsNullOrWhiteSpace(data.FullName))
                ModelState.AddModelError(nameof(data.FullName), "Tên nhân viên không được để trống");
            if (string.IsNullOrWhiteSpace(_birthDate))
                ModelState.AddModelError(nameof(data.BirthDate), "Ngày sinh không được để trống");
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập số điện thoại của nhân viên");
            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập địa chỉ email của nhân viên");
            if (string.IsNullOrWhiteSpace(data.Address))
                ModelState.AddModelError(nameof(data.Address), "Vui lòng nhập địa chỉ của nhân viên");


            //xu li ngay sinh
            DateTime? d = _birthDate.ToDateTime();
            if (d != null)
            {
                if (d.Value.Year < 1753 || d.Value.Year > 9999)
                {
                    ModelState.AddModelError(nameof(data.BirthDate), "Ngày sinh phải nằm trong khoảng từ năm 1753 đến năm 9999.");
                }
                else
                {
                    data.BirthDate = d.Value;
                }
            }
            else
            {
                ModelState.AddModelError(nameof(data.BirthDate), "Ngày sinh nhập không hợp lệ.");
            }


            if (!ModelState.IsValid)
            {
                return View("Edit", data);//trả dữ liệu về cho View kèm theo các thông báo lỗi

            }


            //xu li anh
            if (uploadPhoto != null)
            {

                string fileName = $"{DateTime.Now.Ticks}-{uploadPhoto.FileName}";
                //string folder = @"D:\ThienBinh\SV21T1020254\SV21T1020254.Web\wwwroot\images\employees";
                string filePath = Path.Combine(ApplicationContext.WebRootPath,@"images\employees",fileName);
                using (var stream = new FileStream(filePath,FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);

                }
                data.Photo = fileName;
            }
            //Todo:Kiem soat du lieu dau vao
            if (data.EmployeeID == 0)
            {
               int id= CommonDataService.AddEmployee(data);
                if (id <= 0)
                {
                    ModelState.AddModelError(nameof(data.Email), "Email đã tồn tại");
                    return View("Edit", data);
                }
            }
            else
            {
               bool result= CommonDataService.UpdateEmployee(data);
                if (!result)
                {
                    ModelState.AddModelError(nameof(data.Email), "Email đã tồn tại");
                    return View("Edit", data);
                }
            }

            return RedirectToAction("Index");
        }
        //private DateTime? ToDateTime(string input, string formats = "d/M/yyyy;d-M-yyyy;d.M.yyyy")
        //{
        //    try
        //    {
        //        return DateTime.ParseExact(input, formats.Split(";"), CultureInfo.InvariantCulture);
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}
        //


        //




        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteEmployee(id);
                return RedirectToAction("Index");
            }
            var data = CommonDataService.GetEmployee(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }
    }
}
