import { Link } from "../shared/link";

export interface TransactionWithLinks {
    transactionId: string;
    dateTime: Date;
    symbol: string;
    type: string;
    quantity: number;
    notional: number;
    tradePrice: number;
    links: Link[];
}