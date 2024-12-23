using _21T1020254.BusinessLayers;
using _21T1020254.DomainModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020254.Web.Models;
using System.Buffers;

namespace SV21T1020254.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMIN},{WebUserRoles.MANAGER}")]
    public class CustomerController : Controller
    {
        public const int PAGE_SIZE = 20;
        private const string CUSTOMER_SEARCH_CONDITION = "CustomerSearchCondition";

        public IActionResult Index()

        {
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(CUSTOMER_SEARCH_CONDITION);
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
            var data = CommonDataService.ListOfCustomers(out rowCount,condition.Page,condition.PageSize,condition.SearchValue??"");
            CustomerSearchResult model = new CustomerSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue =condition.SearchValue??"",
                RowCount = rowCount,
                Data =data

            };
            ApplicationContext.SetSessionData(CUSTOMER_SEARCH_CONDITION, condition);
            return View(model);
        }

        public IActionResult Create()
        {

            ViewBag.Title = "Bổ sung khách hàng";

            var data = new Customer()
            {
                CustomerID = 0,
                IsLocked = false
            };
            return View("Edit",data);
        }

        public IActionResult Edit( int id=0)
        {
            ViewBag.Title = "Cập nhận thông tin khách hàng";

            var data = CommonDataService.GetCustomer(id);

            if(data == null)
                return RedirectToAction("Index");
            return View(data);
        }


        [HttpPost]
        public IActionResult Save(Customer data)
        {
            ViewBag.Title = data.CustomerID ==0?"Bổ sung khách hàng":"Cập nhập thông tin khách hàng";
            //Todo:Kiem soat du lieu dau vao, nếu không hợp lệ thì ta tạo ra thông báo lỗi và lưu giữ nó trong ModelState
            //ModelState.addmodelError(key,message)

            if (string.IsNullOrWhiteSpace(data.CustomerName))
                ModelState.AddModelError(nameof(data.CustomerName), "Tên khách hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.ContactName))
                ModelState.AddModelError(nameof(data.ContactName), "Tên giao dịch không được để trống");
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập số điện thoại của khách hàng");
            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập địa chỉ email của khách hàng");
            if (string.IsNullOrWhiteSpace(data.Address))
                ModelState.AddModelError(nameof(data.Address), "Vui lòng nhập địa chỉ của khách hàng");
            if (string.IsNullOrWhiteSpace(data.Province))
               ModelState.AddModelError(nameof(data.Province), "Vui lòng chọn tỉnh thành của khách hàng");
            //Dựa vào ModelState để biết có tồn tại trường hợp lỗi nào không? Sử dụng thuộc tính ModelState.IsValid
            if (!ModelState.IsValid)
            {
                return View("Edit", data);//trả dữ liệu về cho View kèm theo các thông báo lỗi

            }

            if (data.CustomerID == 0)
            {
               int id= CommonDataService.AddCustomer(data);
                if (id <= 0)
                {
                    ModelState.AddModelError(nameof(data.Email), "Email đã tồn tại");
                    return View("Edit", data);
                }
            }
            else
            {
               bool result= CommonDataService.UpdateCustomer(data);
                if (!result)
                {
                    ModelState.AddModelError(nameof(data.Email), "Email đã tồn tại");
                    return View("Edit", data);
                }
            }

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id=0)
        {

            if(Request.Method == "POST")
            {
                CommonDataService.DeleteCustomer(id);
                return RedirectToAction("Index");
            }    
            var data = CommonDataService.GetCustomer(id);
            if(data==null)
                return RedirectToAction("Index");
            return View(data);
        }


    }
}
