using AutoMapper;
using WebApplicationStas.DTOs;
using WebApplicationStas.Exceptions;
using WebApplicationStas.Models;
using WebApplicationStas.Repositories;

namespace WebApplicationStas.Services;

public interface IReservationService
{
    Task<PagedResult<ReservationDto>> GetPagedAsync(int page, int pageSize);
    Task<ReservationDto?> GetByIdAsync(int id);
    Task<ReservationDto> ReserveAsync(CreateReservationDto dto);
    Task<ReservationDto?> ReleaseAsync(int reservationId);
}

public class ReservationService : IReservationService
{
    private readonly IRepository<ProductReservation> _reservations;
    private readonly IRepository<Product> _products;
    private readonly IRepository<User> _users;
    private readonly IMapper _mapper;

    public ReservationService(
        IRepository<ProductReservation> reservations,
        IRepository<Product> products,
        IRepository<User> users,
        IMapper mapper)
    {
        _reservations = reservations;
        _products = products;
        _users = users;
        _mapper = mapper;
    }

    public async Task<PagedResult<ReservationDto>> GetPagedAsync(int page, int pageSize)
    {
        (page, pageSize) = Paging.Normalize(page, pageSize);
        var items = await _reservations.GetPagedAsync(page, pageSize, r => r.Id, "User", "Product");
        var total = await _reservations.CountAsync();
        return new PagedResult<ReservationDto>(_mapper.Map<List<ReservationDto>>(items), total);
    }

    public async Task<ReservationDto?> GetByIdAsync(int id)
    {
        var reservation = await _reservations.GetByIdAsync(id, "User", "Product");
        return reservation == null ? null : _mapper.Map<ReservationDto>(reservation);
    }

    public async Task<ReservationDto> ReserveAsync(CreateReservationDto dto)
    {
        if (dto.Quantity <= 0)
            throw new BusinessRuleException("Количество для резервирования должно быть больше нуля.");

        var user = await _users.GetByIdAsync(dto.UserId);
        if (user == null)
            throw new NotFoundException($"Пользователь с id {dto.UserId} не найден.");

        var product = await _products.GetByIdAsync(dto.ProductId);
        if (product == null)
            throw new NotFoundException($"Товар с id {dto.ProductId} не найден.");

        if (product.StockQuantity < dto.Quantity)
            throw new BusinessRuleException(
                $"Недостаточно товара \"{product.Name}\" на складе: доступно {product.StockQuantity}, запрошено {dto.Quantity}.");

        product.StockQuantity -= dto.Quantity;

        var reservation = _mapper.Map<ProductReservation>(dto);
        _reservations.Add(reservation);

        await _reservations.SaveChangesAsync();

        var created = await _reservations.GetByIdAsync(reservation.Id, "User", "Product");
        return _mapper.Map<ReservationDto>(created!);
    }

    public async Task<ReservationDto?> ReleaseAsync(int reservationId)
    {
        var reservation = await _reservations.GetByIdAsync(reservationId);
        if (reservation == null || reservation.ReleasedAt != null)
            return null;

        var product = await _products.GetByIdAsync(reservation.ProductId);
        if (product == null)
            throw new NotFoundException($"Товар с id {reservation.ProductId} не найден.");

        reservation.ReleasedAt = DateTime.UtcNow;
        product.StockQuantity += reservation.Quantity;

        await _reservations.SaveChangesAsync();

        var updated = await _reservations.GetByIdAsync(reservationId, "User", "Product");
        return _mapper.Map<ReservationDto>(updated!);
    }
}
