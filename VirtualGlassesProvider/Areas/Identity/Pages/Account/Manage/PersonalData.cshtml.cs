﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VirtualGlassesProvider.Models;


namespace VirtualGlassesProvider.Areas.Identity.Pages.Account.Manage
{
    public sealed class PersonalDataModel(
        UserManager<User> userManager
        )
        : PageModel
    {
        private readonly UserManager<User> _userManager = userManager;


        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            return Page();
        }
    }
}
