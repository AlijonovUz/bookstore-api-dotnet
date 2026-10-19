namespace FirstBackendApp.Services.Common;

// Hozirgi so'rov yuborgan foydalanuvchi ma'lumotlarini olish interfeysi
public interface ICurrentUserService
{ 
    // Joriy foydalanuvchi ID raqami
    int? UserId { get; } 

    // Foydalanuvchi Admin yoki yo'qligi
    bool IsAdmin { get; }
}