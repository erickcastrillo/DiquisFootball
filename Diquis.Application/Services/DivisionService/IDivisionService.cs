using Diquis.Application.Common.Marker;
using Diquis.Application.Common.Wrapper;
using Diquis.Application.Services.DivisionService.DTOs;
using Diquis.Application.Services.DivisionService.Filters;

namespace Diquis.Application.Services.DivisionService;

/// <summary>
/// Represents the contract for division-related operations in the application.
/// </summary>
public interface IDivisionService : IScopedService
{
    /// <summary>
    /// Retrieves a list of divisions, optionally filtered by a keyword.
    /// </summary>
    /// <param name="keyword">The keyword to filter divisions by name. If empty, all divisions are returned.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/> with an enumerable of <see cref="DivisionDTO"/>.
    /// </returns>
    Task<Response<IEnumerable<DivisionDTO>>> GetDivisionsAsync(string keyword = "");

    /// <summary>
    /// Retrieves a paginated list of divisions based on the specified filter.
    /// </summary>
    /// <param name="filter">The filter containing pagination and search criteria.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a <see cref="PaginatedResponse{T}"/> of <see cref="DivisionDTO"/>.
    /// </returns>
    Task<PaginatedResponse<DivisionDTO>> GetDivisionsPaginatedAsync(DivisionTableFilter filter);

    /// <summary>
    /// Retrieves a specific division by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the division.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/> with the <see cref="DivisionDTO"/>.
    /// </returns>
    Task<Response<DivisionDTO>> GetDivisionAsync(Guid id);

    /// <summary>
    /// Creates a new division with the specified details.
    /// </summary>
    /// <param name="request">The request containing the details of the division to create.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/> with the unique identifier of the created division.
    /// </returns>
    Task<Response<Guid>> CreateDivisionAsync(CreateDivisionRequest request);

    /// <summary>
    /// Updates an existing division with the specified details.
    /// </summary>
    /// <param name="request">The request containing the updated details of the division.</param>
    /// <param name="id">The unique identifier of the division to update.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/> with the unique identifier of the updated division.
    /// </returns>
    Task<Response<Guid>> UpdateDivisionAsync(UpdateDivisionRequest request, Guid id);

    /// <summary>
    /// Deletes a division by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the division to delete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/> with the unique identifier of the deleted division.
    /// </returns>
    Task<Response<Guid>> DeleteDivisionAsync(Guid id);
}