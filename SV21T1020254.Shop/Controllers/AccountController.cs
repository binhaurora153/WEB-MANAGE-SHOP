﻿using _21T1020254.BusinessLayers;
using _21T1020254.DomainModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace SV21T1020254.Shop.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            ViewBag.Username = username;
            //kiểm tra thông tin đâu vào
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Error", "Nhập đầy đủ tên mà mật khẩu");
                return View();
            }

            var userAccount = UserAccountService.Authorize(UserTypes.Customer, username, password);

            //TODO:Kiểm tra username và password có đúng không?
            if (userAccount==null)
            {
                ModelState.AddModelError("Error", "Đăng nhập thất bại");
                return View();
            }


            //Đăng nhập thành công
            //1. Tạo ra thông tin định danh người dùng
            WebUserData userData = new WebUserData()
            {
                UserId = userAccount.UserId,
                UserName = userAccount.UserName,
                DisplayName = userAccount.DisplayName,
                Photo = userAccount.Photo,
                Roles = userAccount.RoleNames.Split(',').ToList() 
            };
            //2.Ghi nhận trang thái nhập
          await  HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userData.CreatePrincipal());

            //3.Quay về trang chủ
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();

           await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        // Xử lý đổi mật khẩu
        [HttpPost]
        public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            var username = User.FindFirstValue(nameof(WebUserData.UserName));

            // Kiểm tra mật khẩu mới và xác nhận mật khẩu
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("Password", "Mật khẩu mới và xác nhận mật khẩu không khớp.");
                return View();
            }

            if (string.IsNullOrEmpty(username))
            {
                ModelState.AddModelError("", "Không tìm thấy tên người dùng.");
                return View();
            }

            // Xác thực người dùng
            var userType = UserTypes.Customer;
            var userAccount = UserAccountService.Authorize(userType, username, oldPassword);
            if (userAccount == null)
            {
                ModelState.AddModelError("OldPassword", "Mật khẩu cũ không đúng.");
                return View();
            }

            // Thay đổi mật khẩu
            bool isPasswordChanged = UserAccountService.ChangePassword(userType, username, oldPassword, newPassword);
            if (isPasswordChanged)
            {
                ViewBag.Message = "Đổi mật khẩu thành công!";
                return View();
                /*return View("Login");*/
            }
            else
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi khi đổi mật khẩu. Vui lòng thử lại.");
                return View();
            }
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
        public IActionResult AccessDenined()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            // Tạo đối tượng Model mặc định
            var data = new Customer()
            {
                CustomerID = 0, // Gán mặc định nếu chưa có
                Province = "",  // Province mặc định là chuỗi rỗng
                IsLocked = false
            };

            // Truyền model vào View
            return View(data);
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Register(
            string email, string password, string confirmPassword,
            string customerName, string contactName, string province,
            string address, string phone)
        {
            // Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(customerName))
            {
                ModelState.AddModelError("Error", "Vui lòng nhập đầy đủ thông tin bắt buộc.");
                return View(new Customer());
            }

            if (password != confirmPassword)
            {
                ModelState.AddModelError("Error", "Mật khẩu và xác nhận mật khẩu không khớp.");
                return View(new Customer());
            }

            // Kiểm tra email đã tồn tại
            bool emailExists = UserAccountService.CheckEmailExists(email);
            if (emailExists)
            {
                ModelState.AddModelError("Error", "Email này đã tồn tại. Vui lòng sử dụng email khác.");
                return View(new Customer());
            }

            // Thực hiện đăng ký
            var userType = UserTypes.Customer;
            bool isRegistered = UserAccountService.Register(userType, email, password, customerName, contactName, province, address, phone);

            if (isRegistered)
            {
                ViewBag.Message = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }
            else
            {
                ModelState.AddModelError("Error", "Đã xảy ra lỗi khi đăng ký. Vui lòng thử lại.");
                return View(new Customer());
            }
        }

        //
        [HttpGet]
        public IActionResult UserInfo()
        {
            var customerIdClaim = User.FindFirstValue(nameof(WebUserData.UserId)); // lấy CustomerID từ Claims
            if (string.IsNullOrEmpty(customerIdClaim))
            {
                return RedirectToAction("Login");
            }

            int customerId = int.Parse(customerIdClaim); // chuyển sang kiểu int
            var customer = UserAccountService.GetCustomerInfo(customerId);
            if (customer == null)
            {
                return RedirectToAction("Login");
            }

            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateUserInfo(Customer model)
        {
            if (!ModelState.IsValid)
            {
                return View("UserInfo", model);
            }

            var result = UserAccountService.UpdateCustomerInfo(model);
            if (result)
            {
                ViewBag.Message = "Cập nhật thông tin thành công!";
            }
            else
            {
                ModelState.AddModelError("", "Cập nhật thất bại. Vui lòng thử lại.");
            }

            return View("UserInfo", model);
        }


    }

}



