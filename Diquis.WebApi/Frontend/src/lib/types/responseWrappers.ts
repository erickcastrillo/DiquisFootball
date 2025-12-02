// Generic response wrappers
export interface PaginatedResult<T = any> {
  data: T[];
  currentpage: number;
  totalPages: number;
  totalCount: number;
  pageSize: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface Result<T = any> {
  data: T;
  messages: string[];
  succeeded: boolean;
}
