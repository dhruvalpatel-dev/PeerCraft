using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PeerCraft.Data;
using PeerCraft.Dtos;
using PeerCraft.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PeerCraft.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AccountController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (ModelState.IsValid)
            {
                // Check if username or email is already taken
                if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
                {
                    ModelState.AddModelError("Username", "Username is already taken.");
                    return View(registerDto);
                }

                if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
                {
                    ModelState.AddModelError("Email", "Email is already registered.");
                    return View(registerDto);
                }

                // Create a new user object
                var user = new User
                {
                    Username = registerDto.Username,
                    Email = registerDto.Email,
                    // Hash the password for security
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password)
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Redirect to the login page after successful registration
                return RedirectToAction("Login");
            }

            return View(registerDto);
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);

                // Check if user exists and password is correct
                if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(loginDto);
                }

                // Create claims for the authenticated user
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    // Allow the login to persist
                    IsPersistent = true,
                    // Set an expiration time for the cookie
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                };

                // Sign in the user. This creates the authentication cookie.
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Index", "Home");
            }

            return View(loginDto);
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Clear the authentication cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}