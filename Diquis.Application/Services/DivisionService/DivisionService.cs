using AutoMapper;
using Diquis.Application.Services.DivisionService.DTOs;
using Diquis.Application.Common.Wrapper;
using Diquis.Application.Common;
using Diquis.Application.Utility;
using Diquis.Application.Services.DivisionService.Filters;
using Diquis.Application.Services.DivisionService.Specifications;
using Diquis.Domain.Entities.Football.Common;

namespace Diquis.Application.Services.DivisionService
{
    /// <summary>
    /// Service for managing football divisions, including CRUD operations and paginated queries.
    /// </summary>
    public class DivisionService : IDivisionService
    {
        private readonly IRepositoryAsync _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="DivisionService"/> class.
        /// </summary>
        /// <param name="repository">The asynchronous repository for data access.</param>
        /// <param name="mapper">The AutoMapper instance for DTO mapping.</param>
        public DivisionService(IRepositoryAsync repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper; 
        }

        /// <summary>
        /// Retrieves a full list of divisions, optionally filtered by a keyword.
        /// </summary>
        /// <param name="keyword">Optional. The keyword to filter divisions by name.</param>
        /// <returns>A response containing an enumerable of <see cref="DivisionDTO"/>.</returns>
        public async Task<Response<IEnumerable<DivisionDTO>>> GetDivisionsAsync(string keyword = "")
        {
            DivisionSearchList specification = new(keyword);
            IEnumerable<DivisionDTO> list = await _repository.GetListAsync<Division, DivisionDTO, Guid>(specification);
            return Response<IEnumerable<DivisionDTO>>.Success(list);
        }

        /// <summary>
        /// Retrieves a paginated list of divisions for Tanstack Table, supporting filtering and sorting.
        /// </summary>
        /// <param name="filter">The filter containing pagination, sorting, and keyword search options.</param>
        /// <returns>A paginated response containing <see cref="DivisionDTO"/> items.</returns>
        public async Task<PaginatedResponse<DivisionDTO>> GetDivisionsPaginatedAsync(DivisionTableFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                filter.PageNumber = 1;
            }

            string dynamicOrder = (filter.Sorting != null) ? NanoHelpers.GenerateOrderByString(filter) : "";
            DivisionSearchTable specification = new(filter?.Keyword, dynamicOrder);
            PaginatedResponse<DivisionDTO> pagedResponse = await _repository.GetPaginatedResultsAsync<Division, DivisionDTO, Guid>(filter.PageNumber, filter.PageSize, specification);
            return pagedResponse;
        }

        /// <summary>
        /// Retrieves a single division by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the division.</param>
        /// <returns>A response containing the <see cref="DivisionDTO"/> if found; otherwise, a failure response.</returns>
        public async Task<Response<DivisionDTO>> GetDivisionAsync(Guid id)
        {
            try
            {
                DivisionDTO dto = await _repository.GetByIdAsync<Division, DivisionDTO, Guid>(id);
                return Response<DivisionDTO>.Success(dto);
            }
            catch (Exception ex)
            {
                return Response<DivisionDTO>.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Creates a new division.
        /// </summary>
        /// <param name="request">The request DTO containing division details.</param>
        /// <returns>A response containing the unique identifier of the created division, or a failure message if the division already exists.</returns>
        public async Task<Response<Guid>> CreateDivisionAsync(CreateDivisionRequest request)
        {
            DivisionMatchName specification = new(request.Name);
            bool DivisionExists = await _repository.ExistsAsync<Division, Guid>(specification);
            if (DivisionExists)
            {
                return Response<Guid>.Fail("Division already exists");
            }

            Division newDivision = _mapper.Map(request, new Division());

            try
            {
                Division response = await _repository.CreateAsync<Division, Guid>(newDivision);
                _ = await _repository.SaveChangesAsync();
                return Response<Guid>.Success(response.Id);
            }
            catch (Exception ex)
            {
                return Response<Guid>.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Updates an existing division.
        /// </summary>
        /// <param name="request">The request DTO containing updated division details.</param>
        /// <param name="id">The unique identifier of the division to update.</param>
        /// <returns>A response containing the unique identifier of the updated division, or a failure message if not found.</returns>
        public async Task<Response<Guid>> UpdateDivisionAsync(UpdateDivisionRequest request, Guid id)
        {
            Division DivisionInDb = await _repository.GetByIdAsync<Division, Guid>(id);
            if (DivisionInDb == null)
            {
                return Response<Guid>.Fail("Not Found");
            }

            Division updatedDivision = _mapper.Map(request, DivisionInDb);

            try
            {
                Division response = await _repository.UpdateAsync<Division, Guid>(updatedDivision);
                _ = await _repository.SaveChangesAsync();
                return Response<Guid>.Success(response.Id);
            }
            catch (Exception ex)
            {
                return Response<Guid>.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a division by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the division to delete.</param>
        /// <returns>A response containing the unique identifier of the deleted division, or a failure message if an error occurs.</returns>
        public async Task<Response<Guid>> DeleteDivisionAsync(Guid id)
        {
            try
            {
                Division? Division = await _repository.RemoveByIdAsync<Division, Guid>(id);
                _ = await _repository.SaveChangesAsync();

                return Response<Guid>.Success(Division.Id);
            }
            catch (Exception ex)
            {
                return Response<Guid>.Fail(ex.Message);
            }
        }
    }
}

