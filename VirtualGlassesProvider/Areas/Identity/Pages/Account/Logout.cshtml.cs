﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VirtualGlassesProvider.Models;


namespace VirtualGlassesProvider.Areas.Identity.Pages.Account
{
    public sealed class LogoutModel(
        SignInManager<User> signInManager, 
        ILogger<LogoutModel> logger) : PageModel
    {
        private readonly SignInManager<User> _signInManager = signInManager;
        private readonly ILogger<LogoutModel> _logger = logger;


        public async Task<IActionResult> OnPost(string? returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                // This needs to be a redirect so that the browser performs a new
                // request and the identity for the user gets updated.
                return RedirectToPage();
            }
        }
    }
}
