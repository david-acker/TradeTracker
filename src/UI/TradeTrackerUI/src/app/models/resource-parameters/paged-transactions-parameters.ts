export class PagedTransactionsParameters {
    public order: string = 'DateTime+desc';
    public pageNumber: number = 1;
    public pageSize: number = 10;
    public rangeStart?: string;
    public rangeEnd?: string;
}