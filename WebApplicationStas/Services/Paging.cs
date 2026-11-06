namespace WebApplicationStas.Services;

public static class Paging
{
    public static (int Page, int PageSize) Normalize(int page, int pageSize)
        => (page < 1 ? 1 : page, pageSize < 1 ? 10 : pageSize);
}
