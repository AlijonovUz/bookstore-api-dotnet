using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using FirstBackendApp.Contracts.Common.Responses;

namespace FirstBackendApp.Extensions;

// IQueryable so'rovlari uchun pagination va shartli filtrlash kengaytmalari
public static class QueryableExtensions
{
    // Sahifalangan (paged) natijani hisoblash va qaytarish
    public static async Task<PagedResponse<T>> ToPagedListAsync<T>(
        this IQueryable<T> query, 
        int pageNumber, 
        int pageSize)
    {
        var page = pageNumber < 1 ? 1 : pageNumber;
        var size = pageSize is < 1 or > 20 ? 10 : pageSize;

        var totalCount = await query.CountAsync();

        var results = await query.Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return new PagedResponse<T>
        {
            Results = results,
            PageNumber = page,
            PageSize = size,
            TotalCount = totalCount
        };
    }

    // Shart rost (true) bo'lsagina filter shartini qo'shish
    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> query,
        bool condition,
        Expression<Func<T, bool>> predicate)
    {
        return condition ? query.Where(predicate) : query;
    }
}