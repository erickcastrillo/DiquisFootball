import { SortingState } from '@tanstack/react-table';

export interface SearchParams {
  keyword: string;
  pageNumber: number;
  pageSize: number;
  sorting: SortingState;
}
