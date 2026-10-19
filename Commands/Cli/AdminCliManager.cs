using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;
using FirstBackendApp.Data;
using FirstBackendApp.Entities;

namespace FirstBackendApp.Commands.Cli;

// Konsol orqali foydalanuvchilar va adminlarni boshqarish menejeri
public static class AdminCliManager
{
    public static async Task RunAsync(AppDbContext context)
    {
        var menuItems = new[]
        {
            "Yangi Admin yaratish",
            "Foydalanuvchini Admin qilish",
            "Foydalanuvchini Admindan olish (User qilish)",
            "Chiqish"
        };

        int selectedIndex = 0;

        while (true)
        {
            Console.Clear();
            Console.CursorVisible = false;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("========================================");
            Console.WriteLine("       FOYDALANUVCHILARNI BOSHQARISH    ");
            Console.WriteLine("========================================");
            Console.ResetColor();
            Console.WriteLine("(Tanlash uchun ↑ / ↓ strelkalarini bosing, tasdiqlash uchun Enter)\n");

            for (int i = 0; i < menuItems.Length; i++)
            {
                if (i == selectedIndex)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine($" > {menuItems[i]} ");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"   {menuItems[i]}");
                }
            }

            var keyInfo = Console.ReadKey(intercept: true);

            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = (selectedIndex == 0) ? menuItems.Length - 1 : selectedIndex - 1;
                    break;

                case ConsoleKey.DownArrow:
                    selectedIndex = (selectedIndex == menuItems.Length - 1) ? 0 : selectedIndex + 1;
                    break;

                case ConsoleKey.Enter:
                    Console.CursorVisible = true;
                    if (selectedIndex == 3)
                    {
                        Console.WriteLine("\nKonsol boshqaruvi yakunlandi.");
                        return;
                    }

                    await ExecuteActionAsync(selectedIndex, context);
                    break;
            }
        }
    }

    private static async Task ExecuteActionAsync(int index, AppDbContext context)
    {
        Console.Clear();
        switch (index)
        {
            case 0:
                await CreateAdminAsync(context);
                break;
            case 1:
                await PromoteToAdminAsync(context);
                break;
            case 2:
                await DemoteFromAdminAsync(context);
                break;
        }
    }

    // 1. Yangi Admin yaratish
    private static async Task CreateAdminAsync(AppDbContext context)
    {
        Console.WriteLine("--- YANGI ADMIN YARATISH ---");
        Console.Write("Username: ");
        var username = Console.ReadLine()?.Trim();

        Console.Write("Email: ");
        var email = Console.ReadLine()?.Trim();

        Console.Write("Parol: ");
        var password = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ShowError("Username va parol bo'sh bo'lishi mumkin emas!");
            return;
        }

        var exists = await context.Users.AnyAsync(u => u.Username == username);
        if (exists)
        {
            ShowError($"'{username}' nomli foydalanuvchi allaqachon mavjud!");
            return;
        }

        var admin = new User
        {
            Username = username,
            Email = email,
            Role = UserRole.Admin,
            PasswordHash = BC.HashPassword(password),
            CreatedAt = DateTime.UtcNow
        };

        context.Users.Add(admin);
        await context.SaveChangesAsync();

        ShowSuccess($"Admin '{username}' muvaffaqiyatli yaratildi!");
    }

    // 2. Foydalanuvchini Admin qilish
    private static async Task PromoteToAdminAsync(AppDbContext context)
    {
        Console.WriteLine("--- FOYDALANUVCHINI ADMIN QILISH ---");
        Console.Write("Foydalanuvchi username yoki emaili: ");
        var input = Console.ReadLine()?.Trim();

        var user = await context.Users.FirstOrDefaultAsync(u => u.Username == input || u.Email == input);
        if (user is null)
        {
            ShowError("Foydalanuvchi topilmadi!");
            return;
        }

        if (user.Role == UserRole.Admin)
        {
            ShowError($"'{user.Username}' allaqachon Admin!");
            return;
        }

        user.Role = UserRole.Admin;
        await context.SaveChangesAsync();

        ShowSuccess($"Foydalanuvchi '{user.Username}' muvaffaqiyatli ADMIN qilindi!");
    }

    // 3. Foydalanuvchidan Admin huquqini olish
    private static async Task DemoteFromAdminAsync(AppDbContext context)
    {
        Console.WriteLine("--- FOYDALANUVCHIDAN ADMIN HUQUQINI OLISH ---");
        Console.Write("Foydalanuvchi username yoki emaili: ");
        var input = Console.ReadLine()?.Trim();

        var user = await context.Users.FirstOrDefaultAsync(u => u.Username == input || u.Email == input);
        if (user is null)
        {
            ShowError("Foydalanuvchi topilmadi!");
            return;
        }

        if (user.Role == UserRole.User)
        {
            ShowError($"'{user.Username}' allaqachon oddiy User!");
            return;
        }

        user.Role = UserRole.User;
        await context.SaveChangesAsync();

        ShowSuccess($"'{user.Username}' admindan olindi va oddiy USER qilindi!");
    }

    private static void ShowSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n[Muvaffaqiyatli]: {message}");
        Console.ResetColor();
        Console.WriteLine("\nMenyuga qaytish uchun Enter bosing...");
        Console.ReadLine();
    }

    private static void ShowError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n[Xatolik]: {message}");
        Console.ResetColor();
        Console.WriteLine("\nMenyuga qaytish uchun Enter bosing...");
        Console.ReadLine();
    }
}