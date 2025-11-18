using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Diquis.Application.Common;
using Diquis.Application.Common.Marker;
using Diquis.Application.Common.Wrapper;
using Diquis.Domain.Entities.Common;
using Diquis.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Diquis.Infrastructure.Persistence.Repository
{
    // Repository class
    // -- this class should be used by all application services
    // -- it provides abstraction from _context, it can return DTO-mapped lists with pagination
    // -- use ISpecification (Ardalis Specification) to pass query criteria, include statements, and sort expressions. 
    // -- returning mapped DTOs is handled by Automapper's projectTo() method for better performance when handling related entities (https://dev.to/cloudx/entity-framework-core-simplify-your-queries-with-automapper-3m8k)

    /// <summary>
    /// Provides an asynchronous repository implementation for CRUD and query operations using Entity Framework Core and AutoMapper.
    /// </summary>
    public class RepositoryAsync : IRepositoryAsync
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryAsync"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        public RepositoryAsync(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        #region [-- GET --]

        /// <inheritdoc/>
        public async Task<IEnumerable<T>> GetListAsync<T, TId>(ISpecification<T> specification = null, CancellationToken cancellationToken = default) where T : BaseEntity<TId>
        {
            IQueryable<T> query;
            if (specification == null)
            {
                query = _context.Set<T>().AsQueryable();
            }
            else
            {
                query = SpecificationEvaluator.Default.GetQuery(
                  query: _context.Set<T>().AsQueryable(),
                  specification: specification);
            }
            List<T> result = await query.ToListAsync(cancellationToken);
            return result;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TDto>> GetListAsync<T, TDto, TId>(ISpecification<T> specification = null, CancellationToken cancellationToken = default) where T : BaseEntity<TId>
            where TDto : IDto
        {
            IQueryable<T> query;
            if (specification == null)
            {
                query = _context.Set<T>().AsQueryable();
            }
            else
            {
                query = SpecificationEvaluator.Default.GetQuery(
                  query: _context.Set<T>().AsQueryable(),
                  specification: specification);
            }

            List<TDto> result = await query
                .ProjectTo<TDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return result;
        }

        /// <inheritdoc/>
        public async Task<T> GetByIdAsync<T, TId>(TId id, ISpecification<T> specification = null, CancellationToken cancellationToken = default) where T : BaseEntity<TId>
        {
            IQueryable<T> query;
            if (specification == null)
            {
                query = _context.Set<T>().AsQueryable();
            }
            else
            {
                query = SpecificationEvaluator.Default.GetQuery(
                  query: _context.Set<T>().AsQueryable(),
                  specification: specification);
            }

            T entity = await query.Where(x => x.Id.Equals(id)).FirstOrDefaultAsync(cancellationToken);

            if (entity != null)
            {
                return entity;
            }
            else
            {
                throw new Exception("Not Found");
            }
        }

        /// <inheritdoc/>
        public async Task<TDto> GetByIdAsync<T, TDto, TId>(TId id, ISpecification<T> specification = null, CancellationToken cancellationToken = default)
            where T : BaseEntity<TId>
            where TDto : IDto
        {
            IQueryable<T> query;
            if (specification == null)
            {
                query = _context.Set<T>().AsQueryable();
            }
            else
            {
                query = SpecificationEvaluator.Default.GetQuery(
                  query: _context.Set<T>().AsQueryable(),
                  specification: specification);
            }

            TDto result = await query
                .Where(x => x.Id.Equals(id))
                .ProjectTo<TDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken) ?? throw new Exception("Not Found");

            return result;
        }

        /// <inheritdoc/>
        public async Task<bool> ExistsAsync<T, TId>(ISpecification<T> specification = null, CancellationToken cancellationToken = default)
        where T : BaseEntity<TId>
        {
            IQueryable<T> query;
            if (specification == null)
            {
                query = _context.Set<T>().AsQueryable();
            }
            else
            {
                query = SpecificationEvaluator.Default.GetQuery(
                  query: _context.Set<T>().AsQueryable(),
                  specification: specification);
            }

            bool result = await query.AnyAsync(cancellationToken);
            return result;
        }

        #endregion [-- GET --]

        #region [-- CREATE --]

        /// <inheritdoc/>
        public async Task<T> CreateAsync<T, TId>(T entity)
        where T : BaseEntity<TId>
        {
            _ = await _context.Set<T>().AddAsync(entity);
            return entity;
        }

        /// <inheritdoc/>
        public async Task<IList<TId>> CreateRangeAsync<T, TId>(IEnumerable<T> entityList)
        where T : BaseEntity<TId>
        {
            await _context.Set<T>().AddRangeAsync(entityList);
            return entityList.Select(x => x.Id).ToList();
        }
        #endregion [-- CREATE --]

        #region [-- UPDATE --]

        /// <inheritdoc/>
        public async Task<T> UpdateAsync<T, TId>(T entity)
            where T : BaseEntity<TId>
        {
            if (_context.Entry(entity).State == EntityState.Unchanged)
            {
                throw new Exception("Nothing to update");
            }

            T entityInDb = await _context.Set<T>().FindAsync(entity.Id);
            if (entityInDb != null)
            {
                _context.Entry(entityInDb).CurrentValues.SetValues(entity);
                return entity;
            }
            else
            {
                throw new Exception("Not Found");
            }
        }
        #endregion [-- UPDATE --]

        #region [-- REMOVE --]

        /// <inheritdoc/>
        public Task RemoveAsync<T, TId>(T entity) where T : BaseEntity<TId>
        {
            _ = _context.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async Task<T> RemoveByIdAsync<T, TId>(TId entityId)
        where T : BaseEntity<TId>
        {
            T entity = await _context.Set<T>().FindAsync(entityId);
            if (entity == null)
            {
                throw new Exception("Not Found");
            }

            _ = _context.Set<T>().Remove(entity);

            return entity;
        }
        #endregion [-- REMOVE --]

        #region [-- PAGINATION --]

        /// <inheritdoc/>
        public async Task<PaginatedResponse<TDto>> GetPaginatedResultsAsync<T, TDto, TId>(int pageNumber, int pageSize, ISpecification<T> specification = null, CancellationToken cancellationToken = default)
            where T : BaseEntity<TId>
            where TDto : IDto
        {
            IQueryable<T> query;
            if (specification == null)
            {
                query = _context.Set<T>().AsQueryable().AsNoTracking();
            }
            else
            {
                query = SpecificationEvaluator.Default.GetQuery(
                  query: _context.Set<T>().AsQueryable().AsNoTracking(),
                  specification: specification);
            }

            List<TDto> pagedResult;
            int recordsTotal;
            try
            {
                recordsTotal = await query.CountAsync(cancellationToken);
                pagedResult = await query
                    .Skip((pageNumber - 1) * pageSize).Take(pageSize)
                    .ProjectTo<TDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return new PaginatedResponse<TDto>(pagedResult, recordsTotal, pageNumber, pageSize);
        }
        #endregion [-- PAGINATION --]

        #region [-- SAVE --]

        /// <inheritdoc/>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        #endregion [-- SAVE --]
    }
}
