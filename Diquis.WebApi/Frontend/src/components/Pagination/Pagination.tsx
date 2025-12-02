import React, { useCallback, useEffect, useMemo, useState } from 'react';
import { Link } from 'react-router-dom';
import clsx from 'clsx';

export type PageSize = {
  text: string;
  value: number;
};

type PaginationProps = {
  queryPageCount: number;
  queryPageIndex: number;
  queryPageSize: number;
  totalCount: number;
  isLoading: boolean;
  sizePerPageList: PageSize[];
  goToPage: (page: number) => void;
  setPageSize: (size: number) => void;
};

const Pagination = ({
  queryPageCount,
  queryPageIndex,
  queryPageSize,
  totalCount,
  isLoading,
  goToPage,
  setPageSize,
  sizePerPageList
}: PaginationProps) => {
  /**
   * pagination count , index
   */
  const [pageCount, setPageCount] = useState(queryPageCount);
  const [pageIndex, setPageIndex] = useState(queryPageIndex);

  useEffect(() => {
    setPageCount(queryPageCount);
    setPageIndex(queryPageIndex);
  }, [queryPageCount, queryPageIndex]);

  /**
   * get filter pages
   */
  const filterPages = useCallback(
    (visiblePages: number[], totalPages: number) => {
      return visiblePages.filter((page: number) => page <= pageCount);
    },
    [pageCount]
  );

  /**
   * handle visible pages
   */
  const getVisiblePages = useCallback(
    (page: number, total: number) => {
      if (total < 7) {
        return filterPages([1, 2, 3, 4, 5, 6], total);
      } else {
        if (page % 5 >= 0 && page > 4 && page + 2 < total) {
          return [1, page - 1, page, page + 1, total];
        } else if (page % 5 >= 0 && page > 4 && page + 2 >= total) {
          return [1, total - 3, total - 2, total - 1, total];
        } else {
          return [1, 2, 3, 4, 5, total];
        }
      }
    },
    [filterPages]
  );

  /**
   * handle page change
   * @param page - current page
   * @returns
   */
  const changePage = (page: number) => {
    const activePage = pageIndex + 1;

    if (page === activePage) {
      return;
    }

    const visiblePages = getVisiblePages(page, pageCount);
    setVisiblePages(filterPages(visiblePages, pageCount));

    goToPage(page - 1);
  };

  useEffect(() => {
    const visiblePages = getVisiblePages(0, pageCount);
    setVisiblePages(visiblePages);
  }, [pageCount, getVisiblePages]);

  const [visiblePages, setVisiblePages] = useState(
    getVisiblePages(0, pageCount)
  );
  const activePage = pageIndex + 1;

  /**
   * Displays information regarding pagination
   */
  const infoDisplay = useMemo(() => {
    const pageNumber = queryPageIndex + 1;

    const firstDisplay = queryPageSize * pageNumber - queryPageSize + 1;
    let lastDisplay = queryPageSize * pageNumber;
    const totalPages = Math.ceil(totalCount / queryPageSize);

    if (pageNumber == totalPages) {
      lastDisplay = totalCount;
    }

    if (totalCount == 0) {
      return 'No results';
    }

    return (
      'Showing ' +
      firstDisplay +
      ' to ' +
      lastDisplay +
      ' of ' +
      totalCount +
      ' entries'
    );
  }, [queryPageSize, queryPageIndex, totalCount]);

  return (
    <div className="d-lg-flex align-items-center justify-content-between pb-1 table-footer">
      <div>
        <span>{isLoading || infoDisplay}</span>
      </div>

      <div>
        {sizePerPageList.length > 0 && (
          <div className="d-inline-block me-3">
            <select
              value={queryPageSize}
              onChange={(e) => {
                setPageSize(Number(e.target.value));
              }}
              className="form-select d-inline-block w-auto"
            >
              {(sizePerPageList || []).map((pageSize, index) => {
                return (
                  <option key={index.toString()} value={pageSize.value}>
                    {pageSize.text}
                  </option>
                );
              })}
            </select>
          </div>
        )}

        <ul className="pagination pagination-rounded d-inline-flex align-item-center mb-0">
          <li
            key="prevpage"
            className={clsx('page-item', 'paginate_button', 'previous', {
              disabled: activePage === 1
            })}
            onClick={() => {
              if (activePage === 1) return;
              changePage(activePage - 1);
            }}
          >
            <Link to="#" className="page-link">
              <i className="mdi mdi-chevron-left"></i>
            </Link>
          </li>
          {(visiblePages || []).map((page, index, array) => {
            return array[index - 1] + 1 < page ? (
              <React.Fragment key={page.toString()}>
                <li className="page-item disabled d-none d-xl-inline-block">
                  <Link to="#" className="page-link">
                    ...
                  </Link>
                </li>
                <li
                  className={clsx('page-item', 'd-none', 'd-xl-inline-block', {
                    active: activePage === page
                  })}
                  onClick={(e) => changePage(page)}
                >
                  <Link to="#" className="page-link">
                    {page}
                  </Link>
                </li>
              </React.Fragment>
            ) : (
              <li
                key={page.toString()}
                className={clsx('page-item', 'd-none', 'd-xl-inline-block', {
                  active: activePage === page
                })}
                onClick={(e) => changePage(page)}
              >
                <Link to="#" className="page-link">
                  {page}
                </Link>
              </li>
            );
          })}
          <li
            key="nextpage"
            className={clsx('page-item', 'paginate_button', 'next', {
              disabled: activePage === queryPageCount
            })}
            onClick={() => {
              if (activePage === queryPageCount) return;
              changePage(activePage + 1);
            }}
          >
            <Link to="#" className="page-link">
              <i className="mdi mdi-chevron-right"></i>
            </Link>
          </li>
        </ul>
      </div>
    </div>
  );
};

export default Pagination;
