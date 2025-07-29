export type Pagination = {
    currentPage: number;
    pageSize: number;
    totalCount: number;
    totalPages: number;
}

export type PaginationResult<T> = {
    items: T[];
    metadata: Pagination;
}