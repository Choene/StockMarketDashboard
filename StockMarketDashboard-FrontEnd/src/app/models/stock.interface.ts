export interface StockResponse {
  metaData: MetaData;
  timeSeriesDaily: { [key: string]: DailyData };
}

export interface MetaData {
  information: string;
  symbol: string;
  lastRefreshed: string;
  outputSize: string;
  timeZone: string;
}

export interface DailyData {
  open: string;
  high: string;
  low: string;
  close: string;
  volume: string;
}
