using Ardalis.Specification;
using Diquis.Application.Common.Marker;
using Diquis.Application.Common.Wrapper;
using Diquis.Domain.Entities.Common;

namespace Diquis.Application.Common
{
    /// <summary>
    /// Service interface for asynchronous repository operations.
    /// </summary>
    public interface IRepositoryAsync : IScopedService
    {
        /// <summary>
        /// Gets a list of entities matching the specified specification.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <typeparam name="TId">The entity identifier type.</typeparam>
        /// <param name="specification">The specification to filter entities.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of entities.</returns>
        Task<IEnumerable<T>> GetListAsync<T, TId>(ISpecification<T>? specification = null, CancellationToken cancellationToken = default)
        where T : BaseEntity<TId>;

        /// <summary>
        /// Gets a list of DTOs matching the specified specification.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <typeparam name="TDto">The DTO type.</typeparam>
        /// <typeparam name="TId">The entity identifier type.</typeparam>
        /// <param name="specification">The specification to filter entities.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of DTOs.</returns>
        Task<IEnumerable<TDto>> GetListAsync<T, TDto, TId>(ISpecification<T>? specification = null, CancellationToken cancellationToken = default)
        where T : BaseEntity<TId>
        where TDto : IDto;

        /// <summary>
        /// Gets an entity by its identifier and optional specification.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <typeparam name="TId">The entity identifier type.</typeparam>
        /// <param name="id">The entity identifier.</param>
        /// <param name="specification">The specification to filter entities.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The entity.</returns>
        Task<T> GetByIdAsync<T, TId>(TId id, ISpecification<T>? specification = null, CancellationToken cancellationToken = default)
        where T : BaseEntity<TId>;

        /// <summary>
        /// Gets a DTO by entity identifier and optional specification.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <typeparam name="TDto">The DTO type.</typeparam>
        /// <typeparam name="TId">The entity identifier type.</typeparam>
        /// <param name="id">The entity identifier.</param>
        /// <param name="specification">The specification to filter entities.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The DTO.</returns>
        Task<TDto> GetByIdAsync<T, TDto, TId>(TId id, ISpecification<T>? specification = null, CancellationToken cancellationToken = default)
        where T : BaseEntity<TId>
        where TDto : IDto;

        /// <summary>
        /// Checks if any entities exist matching the specified specification.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <typeparam name="TId">The entity identifier type.</typeparam>
        /// <param name="specification">The specification to filter entities.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>True if any entities exist; otherwise, false.</returns>
        Task<bool> ExistsAsync<T, TId>(ISpecification<T>? specification = null, CancellationToken cancellationToken = default)
        where T : BaseEntity<TId>;

        /// <summary>
        /// Creates a new entity.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <typeparam name="TId">The entity identifier type.</typeparam>
        /// <param name="entity">The entity to create.</param>
        /// <returns>The created entity.</returns>
        Task<T> CreateAsync<T, TId>(T entity)
        where T : BaseEntity<TId>;

        /// <summary>
        /// Creates a range of entities.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <typeparam name="TId">The entity identifier type.</typeparam>
        /// <param name="entityList">The list of entities to create.</param>
        /// <returns>A list of created entity identifiers.</returns>
        Task<IList<TId>> CreateRangeAsync<T, TId>(IEnumerable<T> entityList)
        where T : BaseEntity<TId>;

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <typeparam name="TId">The entity identifier type.</typeparam>
        /// <param name="entity">The entity to update.</param>
        /// <returns>The updated entity.</returns>
        Task<T> UpdateAsync<T, TId>(T entity)
        where T : BaseEntity<TId>;

        /// <summary>
        /// Removes an entity.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <typeparam name="TId">The entity identifier type.</typeparam>
        /// <param name="entity">The entity to remove.</param>
        Task RemoveAsync<T, TId>(T entity)
        where T : BaseEntity<TId>;

        /// <summary>
        /// Removes an entity by its identifier.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <typeparam name="TId">The entity identifier type.</typeparam>
        /// <param name="entityId">The entity identifier.</param>
        /// <returns>The removed entity.</returns>
        Task<T> RemoveByIdAsync<T, TId>(TId entityId)
        where T : BaseEntity<TId>;

        /// <summary>
        /// Gets paginated results as DTOs based on the specified specification.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <typeparam name="TDto">The DTO type.</typeparam>
        /// <typeparam name="TId">The entity identifier type.</typeparam>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="specification">The specification to filter entities.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A paginated response containing DTOs.</returns>
        Task<PaginatedResponse<TDto>> GetPaginatedResultsAsync<T, TDto, TId>(int pageNumber, int pageSize, ISpecification<T>? specification = null, CancellationToken cancellationToken = default)
        where T : BaseEntity<TId>
        where TDto : IDto;

        /// <summary>
        /// Saves all changes made in the repository.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        Task<int> SaveChangesAsync();
    }
}
