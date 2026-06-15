using LioraApp.Models;
using LioraApp.Resources;
using LioraApp.Utilities;
using LioraApp.Utilities.Validation;
using LioraApp.ViewModels.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Localization;

namespace LioraApp.Areas.Identity.Controllers;

[Area("Identity")]
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IEmailSender _emailSender;
    private readonly IStringLocalizer<SharedResource> _localizer;
    private readonly IConfiguration _configuration;
    private readonly IPhoneNumberValidator _phoneNumberValidator;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailSender emailSender,
        IStringLocalizer<SharedResource> localizer,
        IConfiguration configuration,
        IPhoneNumberValidator phoneNumberValidator)
    {
        _userManager  = userManager;
        _signInManager = signInManager;
        _emailSender  = emailSender;
        _localizer = localizer;
        _configuration = configuration;
        _phoneNumberValidator = phoneNumberValidator;
    }

    // ─── LOGOUT ─────────────────────────────────────────────────
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }

    // ─── REGISTER ───────────────────────────────────────────────
    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterVM vm)
    {
        var phoneValidation = _phoneNumberValidator.ValidateAndFormat(vm.PhoneNumber, vm.PhoneCountryIso2, isRequired: true);
        if (!phoneValidation.IsValid)
        {
            ModelState.AddModelError(nameof(vm.PhoneNumber), phoneValidation.ErrorMessage!);
        }

        if (!ModelState.IsValid) return View(vm);

        var user = new ApplicationUser
        {
            FullName       = vm.FullName,
            Email          = vm.Email,
            UserName       = vm.Email,
            PhoneNumber    = phoneValidation.E164Number,
            CreatedAt      = DateTime.UtcNow,
            IsActive       = true,
        };

        var result = await _userManager.CreateAsync(user, vm.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return View(vm);
        }

        await _userManager.AddToRoleAsync(user, SD.Role_Customer);

        // Send email confirmation link
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var publicBaseUrl = _configuration["App:PublicBaseUrl"] ?? $"{Request.Scheme}://{Request.Host}";
        var link = Url.Action(nameof(ConfirmEmail), "Account",
                        new { area = "Identity", token, email = user.Email },
                        protocol: "https",
                        host: new Uri(publicBaseUrl).Host);

        var confirmEmailBody = $"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8" />
                <meta name="viewport" content="width=device-width, initial-scale=1.0" />
                <title>Confirm Your Email</title>
            </head>
            <body style="margin:0; padding:0; background-color:#fff8f3; font-family:'Segoe UI', Arial, sans-serif;">
                <table width="100%" cellpadding="0" cellspacing="0" style="padding:40px 0;">
                    <tr><td align="center">
                        <table width="560" cellpadding="0" cellspacing="0" style="background:#ffffff; border-radius:24px; overflow:hidden; box-shadow: 0 4px 24px rgba(127,84,70,0.10);">
                            <tr><td style="background: linear-gradient(135deg, #c49080 0%, #7f5446 100%); padding:32px 40px; text-align:center;">
                                <h1 style="margin:0; color:#ffffff; font-size:32px; font-weight:400; font-style:italic; letter-spacing:1px;">liora</h1>
                                <p style="margin:8px 0 0; color:rgba(255,255,255,0.8); font-size:14px;">Welcome to the family!</p>
                            </td></tr>
                            <tr><td style="padding:40px 40px 32px; text-align:center;">
                                <h2 style="margin:0 0 12px; color:#7f5446; font-size:22px; font-weight:600;">Confirm Your Email</h2>
                                <p style="margin:0 0 28px; color:#83746f; font-size:15px; line-height:1.7;">Thank you for joining Liora, {user.FullName}!<br/>Please confirm your email to activate your account.</p>
                                <a href="{link}" style="display:inline-block; padding:16px 40px; background:#7f5446; color:#ffffff; text-decoration:none; border-radius:50px; font-size:14px; font-weight:700; letter-spacing:1px; text-transform:uppercase;">Confirm My Email</a>
                            </td></tr>
                            <tr><td style="padding:0 40px;"><hr style="border:none; border-top:1px solid #e8e1db; margin:0;" /></td></tr>
                            <tr><td style="padding:20px 40px 28px;">
                                <p style="margin:0; color:#9ca3af; font-size:12px; text-align:center; line-height:1.7;">If you didn't create a Liora account, please ignore this email.<br/>© 2026 Liora. All rights reserved.</p>
                            </td></tr>
                        </table>
                    </td></tr>
                </table>
            </body>
            </html>
            """;

        await _emailSender.SendEmailAsync(user.Email!, "Liora — Confirm Your Email", confirmEmailBody);

        TempData["success"] = "Account created! Please check your email to confirm your account before signing in.";
        return RedirectToAction(nameof(Login));
    }

    // ─── CONFIRM EMAIL ──────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string token, string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null) return NotFound();

        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (result.Succeeded)
            TempData["success"] = "Email confirmed! You can now sign in.";
        else
            TempData["error"] = "Email confirmation failed. The link may have expired.";

        return RedirectToAction(nameof(Login));
    }

    // ─── LOGIN ──────────────────────────────────────────────────
    [HttpGet]
    public IActionResult Login() => View();

    [HttpGet]
    public IActionResult AccessDenied() => View();

    [HttpPost]
    [EnableRateLimiting("auth")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVM vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var user = await _userManager.FindByEmailAsync(vm.Email);

        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(vm);
        }

        if (!await _userManager.IsEmailConfirmedAsync(user))
        {
            ModelState.AddModelError(string.Empty, "Please confirm your email before signing in. Check your inbox.");
            return View(vm);
        }

        if (!user.IsActive)
        {
            ModelState.AddModelError(string.Empty, "Your account has been deactivated. Please contact support.");
            return View(vm);
        }

        var result = await _signInManager.PasswordSignInAsync(
            user, vm.Password,
            isPersistent: vm.RememberMe,
            lockoutOnFailure: true);

        if (result.Succeeded)
            return RedirectToAction("Index", "Home", new { area = "Customer" });

        if (result.IsLockedOut)
            ModelState.AddModelError(string.Empty, "Account locked due to too many failed attempts. Try again later.");
        else
            ModelState.AddModelError(string.Empty, "Invalid email or password.");

        return View(vm);
    }
}
