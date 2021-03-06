import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';

import { PagedTransactionsParameters } from '../models/transactions/paged-transactions-parameters';
import { PagedTransactionsWithLinks } from '../models/transactions/paged-transactions-with-links';
import { TransactionForCreation } from '../models/transactions/transaction-for-creation';
import { Position } from '../models/positions/position';

@Injectable({
    providedIn: 'root'
})
export class DataService {
    public pagedTransactionsWithLinks: PagedTransactionsWithLinks;
    public position: Position;

    constructor(private http: HttpClient) { }

    public addTransaction(transaction: TransactionForCreation) {
        let token: string = localStorage.getItem('token');

        let headers = new HttpHeaders({
            'Content-Type': 'application/json',
            'Accept': 'application/json',
            'Authorization': `Bearer ${token}`
        });

        return this.http.post('api/transactions', transaction, {
            headers: headers
        }).pipe(
            map((response: any) => {
                return true;
            }));
    }

    public getPagedTransactionsWithLinks(resourceParams: PagedTransactionsParameters): Observable<boolean> {
        let token: string = localStorage.getItem('token');

        let headers = new HttpHeaders({
            'Content-Type': 'application/json',
            'Accept': 'application/vnd.trade.hateoas+json',
            'Authorization': `Bearer ${token}`
        });

        let params = new HttpParams()

        if (resourceParams.type != '') {
            params = params.set('Type', resourceParams.type.toString());
        }

        params = params.set('Order', resourceParams.order);
        params = params.set('PageNumber', resourceParams.pageNumber.toString());
        params = params.set('PageSize', resourceParams.pageSize.toString());

        if (resourceParams.rangeStart != null) {
            params = params.set('RangeStart', resourceParams.rangeStart.toString());
        }
        
        if (resourceParams.rangeEnd != null) {
            params = params.set('RangeEnd', resourceParams.rangeEnd.toString());
        }

        if (resourceParams.selection != null) {
            params = params.set('selection', resourceParams.selection);
        }
        
        return this.http.get('/api/transactions', {
            headers: headers,
            params: params
        }).pipe(
            map((data: any) => {
                this.pagedTransactionsWithLinks = data;
                return true;
            })
        );
    }

    public getPositionForSymbol(symbol: string): Observable<boolean> {
        let token: string = localStorage.getItem('token');

        let headers = new HttpHeaders({
            'Content-Type': 'application/json',
            'Accept': 'application/json',
            'Authorization': `Bearer ${token}`
        });

        return this.http.get(`api/positions/${symbol}`, {
            headers: headers
        }).pipe(
            map((data: any) => {
                this.position = data;
                return true;
            })
        )
    }
}